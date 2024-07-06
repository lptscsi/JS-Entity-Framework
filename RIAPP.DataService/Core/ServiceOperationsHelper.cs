﻿using Microsoft.Extensions.DependencyInjection;
using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Resources;
using RIAPP.DataService.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    public class ServiceOperationsHelper<TService> : IServiceOperationsHelper<TService>, IEntityVersionHelper<TService>, IDisposable
        where TService : BaseDomainService
    {
        #region Fields

        /// <summary>
        /// Already created instances of DataManagers indexed by dbSetName (their lifetime should span the whole request execution for all entities)
        /// they are disposed after the request ended
        /// </summary>
        private readonly ConcurrentDictionary<string, object> _dataManagers;
        private readonly IDataHelper<TService> _dataHelper;
        private TService _domainService;

        #endregion

        public ServiceOperationsHelper(TService domainService, IDataHelper<TService> dataHelper)
        {
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
            _dataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
            _dataManagers = new ConcurrentDictionary<string, object>();
        }

        #region Private methods

        private void ApplyValue(object entity, RowInfo rowInfo, string fullName, Field fieldInfo, ValueChange val,
          bool isOriginal)
        {
            if (isOriginal)
            {
                if ((val.Flags & (int)ValueFlags.Setted) == (int)ValueFlags.Setted)
                {
                    _dataHelper.SetFieldValue(entity, fullName, fieldInfo, val.Orig);
                }
            }
            else
            {
                switch (rowInfo.ChangeType)
                {
                    case ChangeType.Deleted:
                        {
                            // for delete fill only original values
                            if ((val.Flags & (int)ValueFlags.Setted) == (int)ValueFlags.Setted)
                            {
                                _dataHelper.SetFieldValue(entity, fullName, fieldInfo, val.Orig);
                            }
                        }
                        break;
                    case ChangeType.Added:
                        {
                            if ((val.Flags & (int)ValueFlags.Changed) == (int)ValueFlags.Changed)
                            {
                                if (fieldInfo.isReadOnly && val.Val != null && !fieldInfo.allowClientDefault)
                                {
                                    throw new ValidationException(string.Format(ErrorStrings.ERR_PROPERTY_IS_READONLY,
                                        entity.GetType().Name, fieldInfo.fieldName));
                                }

                                if (!fieldInfo.isAutoGenerated && !fieldInfo.isNullable && val.Val == null)
                                {
                                    throw new ValidationException(string.Format(ErrorStrings.ERR_FIELD_IS_NOT_NULLABLE,
                                        entity.GetType().Name, fieldInfo.fieldName));
                                }

                                _dataHelper.SetFieldValue(entity, fullName, fieldInfo, val.Val);
                            }
                        }
                        break;
                    case ChangeType.Updated:
                        {
                            if ((val.Flags & (int)ValueFlags.Changed) == (int)ValueFlags.Changed)
                            {
                                if (fieldInfo.isReadOnly)
                                {
                                    throw new ValidationException(string.Format(ErrorStrings.ERR_PROPERTY_IS_READONLY,
                                        entity.GetType().Name, fieldInfo.fieldName));
                                }

                                if (!fieldInfo.isNullable && val.Val == null)
                                {
                                    throw new ValidationException(string.Format(ErrorStrings.ERR_FIELD_IS_NOT_NULLABLE,
                                        fieldInfo.fieldName));
                                }

                                _dataHelper.SetFieldValue(entity, fullName, fieldInfo, val.Val);
                            }
                            else if ((val.Flags & (int)ValueFlags.Setted) == (int)ValueFlags.Setted)
                            {
                                // when not changed then original value must be the same as current
                                if ((fieldInfo.isPrimaryKey > 0 || fieldInfo.fieldType == FieldType.RowTimeStamp || fieldInfo.isNeedOriginal) && val.Val != val.Orig)
                                {
                                    throw new ValidationException(string.Format(ErrorStrings.ERR_VAL_ORIGINAL_INVALID,
                                        fieldInfo.fieldName));
                                }

                                _dataHelper.SetFieldValue(entity, fullName, fieldInfo, val.Val);
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _domainService = null;
            try
            {
                IDisposable[] dataManagers = _dataManagers.Values
                    .Where(m => m is IDisposable)
                    .Select(m => (IDisposable)m)
                    .ToArray();
                Array.ForEach(dataManagers, m =>
                {
                    m.Dispose();
                });
            }
            finally
            {
                _dataManagers.Clear();
            }
        }

        #endregion

        #region IServiceOperationsHelper

        public object GetMethodOwner(string dbSetName, MethodInfoData methodData)
        {
            // if method is not on datta manager (aka handler) then it is defined on the Data Service
            if (!methodData.IsInDataManager)
            {
                return _domainService;
            }

            if (methodData.EntityType == null)
            {
                return _domainService;
            }

            if (string.IsNullOrEmpty(dbSetName))
            {
                throw new InvalidOperationException("For DataManamagers (aka handlers) the DbSetName argument must not be empty");
            }

            // just to initialize metadata (in case it is not initialized)
            RunTimeMetadata metadata = _domainService.GetMetadata();

            // different DbSets can have the same entity type, but the instances of data managers (aka handlers) should be different
            // so we use dbSet Name indexing for them
            object handlerInstance = _dataManagers.GetOrAdd(dbSetName, name =>
            {
                IServiceProvider sp = _domainService.ServiceContainer.ServiceProvider;
                return ActivatorUtilities.CreateInstance(sp, methodData.OwnerType);
            });
            return handlerInstance;
        }

        public async Task AfterExecuteChangeSet(ChangeSetRequest changeSet)
        {
            IEnumerable<IDataManager> dataManagers = _dataManagers.Values.Select(m => (IDataManager)m);
            foreach (IDataManager dataManager in dataManagers)
            {
                await dataManager.AfterExecuteChangeSet(changeSet);
            }
        }

        public async Task AfterChangeSetCommited(ChangeSetRequest changeSet, SubResultList refreshResult)
        {
            IEnumerable<IDataManager> dataManagers = _dataManagers.Values.Select(m => (IDataManager)m);
            foreach (IDataManager dataManager in dataManagers)
            {
                await dataManager.AfterChangeSetCommited(changeSet, refreshResult);
            }
        }

        public void ApplyValues(object entity, RowInfo rowInfo, string path, ValueChange[] values, bool isOriginal)
        {
            DbSetInfo dbSetInfo = rowInfo.GetDbSetInfo();
            Array.ForEach(values, val =>
            {
                string fullName = path + val.FieldName;
                Field fieldInfo = _dataHelper.GetFieldInfo(dbSetInfo, fullName);
                if (!fieldInfo.GetIsIncludeInResult())
                {
                    return;
                }
                //Server Side calculated fields are never set on entities from updates
                if (fieldInfo.fieldType == FieldType.ServerCalculated)
                {
                    return;
                }

                if (fieldInfo.fieldType == FieldType.Object && val.Nested != null)
                {
                    ApplyValues(entity, rowInfo, fullName + '.', val.Nested.ToArray(), isOriginal);
                }
                else
                {
                    ApplyValue(entity, rowInfo, fullName, fieldInfo, val, isOriginal);
                }
            });
        }

        public void UpdateEntityFromRowInfo(object entity, RowInfo rowInfo, bool isOriginal)
        {
            DbSetInfo dbSetInfo = rowInfo.GetDbSetInfo();
            ValuesList values = rowInfo.Values;
            ApplyValues(entity, rowInfo, "", values.ToArray(), isOriginal);

            if (!isOriginal && rowInfo.ChangeType == ChangeType.Added)
            {
                foreach (ParentChildNode pn in rowInfo.GetChangeState().ParentRows)
                {
                    if (!PropHelper.SetValue(entity, pn.Association.childToParentName, pn.ParentRow.GetChangeState().Entity, false))
                    {
                        throw new DomainServiceException(string.Format(ErrorStrings.ERR_CAN_NOT_SET_PARENT_FIELD,
                            pn.Association.childToParentName, dbSetInfo.GetEntityType().Name));
                    }
                }
            }
        }

        public void UpdateValuesFromEntity(object entity, string path, DbSetInfo dbSetInfo, ValueChange[] values)
        {
            Array.ForEach(values, val =>
            {
                string fullName = path + val.FieldName;
                Field fieldInfo = _dataHelper.GetFieldInfo(dbSetInfo, fullName);
                if (!fieldInfo.GetIsIncludeInResult())
                {
                    return;
                }

                if (fieldInfo.fieldType == FieldType.Object && val.Nested != null)
                {
                    UpdateValuesFromEntity(entity, fullName + '.', dbSetInfo, val.Nested.ToArray());
                }
                else
                {
                    val.Val = _dataHelper.SerializeField(entity, fullName, fieldInfo);
                    val.Flags = val.Flags | (int)ValueFlags.Refreshed;
                }
            });
        }

        public void CheckValuesChanges(RowInfo rowInfo, string path, ValueChange[] values)
        {
            DbSetInfo dbSetInfo = rowInfo.GetDbSetInfo();
            Array.ForEach(values, val =>
            {
                string fullName = path + val.FieldName;
                Field fieldInfo = _dataHelper.GetFieldInfo(dbSetInfo, fullName);
                if (!fieldInfo.GetIsIncludeInResult())
                {
                    return;
                }

                if (fieldInfo.fieldType == FieldType.Object && val.Nested != null)
                {
                    CheckValuesChanges(rowInfo, fullName + '.', val.Nested.ToArray());
                }
                else
                {
                    if (IsEntityValueChanged(rowInfo, fullName, fieldInfo, out string newVal))
                    {
                        val.Val = newVal;
                        val.Flags = val.Flags | (int)ValueFlags.Refreshed;
                    }
                }
            });
        }

        public void UpdateRowInfoFromEntity(object entity, RowInfo rowInfo)
        {
            DbSetInfo dbSetInfo = rowInfo.GetDbSetInfo();
            UpdateValuesFromEntity(entity, "", dbSetInfo, rowInfo.Values.ToArray());
            if (rowInfo.ChangeType == ChangeType.Added)
            {
                rowInfo.ServerKey = rowInfo.GetRowKeyAsString();
            }
        }

        public bool IsEntityValueChanged(RowInfo rowInfo, string fullName, Field fieldInfo, out string newVal)
        {
            EntityChangeState changeState = rowInfo.GetChangeState();
            string oldVal = null;
            newVal = _dataHelper.SerializeField(changeState.Entity, fullName, fieldInfo);
            if (changeState.OriginalEntity != null)
            {
                oldVal = _dataHelper.SerializeField(changeState.OriginalEntity, fullName, fieldInfo);
            }

            return newVal != oldVal;
        }

        public void UpdateRowInfoAfterUpdates(RowInfo rowInfo)
        {
            CheckValuesChanges(rowInfo, "", rowInfo.Values.ToArray());

            if (rowInfo.ChangeType == ChangeType.Added)
            {
                rowInfo.ServerKey = rowInfo.GetRowKeyAsString();
            }
        }

        #endregion

        #region IEntityVersionHelper

        public object GetOriginalEntity(RowInfo rowInfo)
        {
            if (rowInfo == null)
            {
                throw new DomainServiceException(ErrorStrings.ERR_METH_APPLY_INVALID);
            }
            return rowInfo.GetChangeState().OriginalEntity;
        }

        public T GetOriginalEntity<T>(RowInfo rowInfo)
            where T : class
        {
            return (T)GetOriginalEntity(rowInfo);
        }

        public object GetOriginalEntity(object entity, RowInfo rowInfo)
        {
            object dbEntity = Activator.CreateInstance(entity.GetType());
            UpdateEntityFromRowInfo(dbEntity, rowInfo, true);
            return dbEntity;
        }

        public object GetParentEntity(Type entityType, RowInfo rowInfo)
        {
            if (rowInfo == null)
            {
                throw new DomainServiceException(ErrorStrings.ERR_METH_APPLY_INVALID);
            }
            ParentChildNode[] parents = rowInfo.GetChangeState().ParentRows;
            if (parents.Length == 0)
            {
                return null;
            }

            return
                parents.Where(p => p.ParentRow.GetDbSetInfo().GetEntityType() == entityType)
                    .Select(p => p.ParentRow.GetChangeState().Entity)
                    .FirstOrDefault();
        }

        public T GetParentEntity<T>(RowInfo rowInfo)
            where T : class
        {
            return (T)GetParentEntity(typeof(T), rowInfo);
        }

        #endregion
    }
}