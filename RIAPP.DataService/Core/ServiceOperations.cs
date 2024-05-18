using Microsoft.Extensions.DependencyInjection;
using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Resources;
using RIAPP.DataService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    public class ServiceOperations<TService> : IServiceOperations<TService>, IDisposable
        where TService : BaseDomainService
    {
        #region Fields

        private TService _domainService;
        private readonly IServiceOperationsHelper<TService> _operations;
        private readonly IEntityVersionHelper<TService> _entityVersion;
        private readonly IDataHelper<TService> _dataHelper;
        private readonly IValidationHelper<TService> _validation;

        #endregion

        public ServiceOperations(
            TService domainService,
            IServiceOperationsHelper<TService> operations,
            IEntityVersionHelper<TService> entityVersion,
            IDataHelper<TService> dataHelper,
            IValidationHelper<TService> validation)
        {
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
            _operations = operations ?? throw new ArgumentNullException(nameof(operations));
            _entityVersion = entityVersion ?? throw new ArgumentNullException(nameof(entityVersion));
            _dataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
            _validation = validation ?? throw new ArgumentNullException(nameof(validation));
        }

        public void Dispose()
        {
            // NOOP
        }

        #region IServiceOperations

        public async Task InsertEntity(RunTimeMetadata metadata, RowInfo rowInfo)
        {
            DbSetInfo dbSetInfo = rowInfo.GetDbSetInfo();
            if (rowInfo.changeType != ChangeType.Added)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_REC_CHANGETYPE_INVALID,
                    dbSetInfo.GetEntityType().Name, rowInfo.changeType));
            }

            MethodInfoData methodData = metadata.GetOperationMethodInfo(dbSetInfo.dbSetName, MethodType.Insert);
            if (methodData == null)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_DB_INSERT_NOT_IMPLEMENTED,
                    dbSetInfo.GetEntityType().Name, GetType().Name));
            }

            object dbEntity = Activator.CreateInstance(dbSetInfo.GetEntityType());
            _operations.UpdateEntityFromRowInfo(dbEntity, rowInfo, false);
            rowInfo.GetChangeState().Entity = dbEntity;
            object instance = _operations.GetMethodOwner(dbSetInfo.dbSetName, methodData);
            object res = methodData.MethodInfo.Invoke(instance, new[] { dbEntity });
            if (res is Task)
            {
                await (res as Task);
            }
        }

        public async Task UpdateEntity(RunTimeMetadata metadata, RowInfo rowInfo)
        {
            DbSetInfo dbSetInfo = rowInfo.GetDbSetInfo();
            if (rowInfo.changeType != ChangeType.Updated)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_REC_CHANGETYPE_INVALID,
                    dbSetInfo.GetEntityType().Name, rowInfo.changeType));
            }

            MethodInfoData methodData = metadata.GetOperationMethodInfo(dbSetInfo.dbSetName, MethodType.Update);
            if (methodData == null)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_DB_UPDATE_NOT_IMPLEMENTED,
                    dbSetInfo.GetEntityType().Name, GetType().Name));
            }

            object dbEntity = Activator.CreateInstance(dbSetInfo.GetEntityType());
            _operations.UpdateEntityFromRowInfo(dbEntity, rowInfo, false);
            object original = _entityVersion.GetOriginalEntity(dbEntity, rowInfo);
            rowInfo.GetChangeState().Entity = dbEntity;
            rowInfo.GetChangeState().OriginalEntity = original;
            object instance = _operations.GetMethodOwner(dbSetInfo.dbSetName, methodData);
            //apply this changes to entity that is in the database (this is done in user domain service method)
            object res = methodData.MethodInfo.Invoke(instance, new[] { dbEntity });
            if (res is Task)
            {
                await (res as Task);
            }
        }

        public async Task DeleteEntity(RunTimeMetadata metadata, RowInfo rowInfo)
        {
            DbSetInfo dbSetInfo = rowInfo.GetDbSetInfo();
            if (rowInfo.changeType != ChangeType.Deleted)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_REC_CHANGETYPE_INVALID,
                    dbSetInfo.GetEntityType().Name, rowInfo.changeType));
            }

            MethodInfoData methodData = metadata.GetOperationMethodInfo(dbSetInfo.dbSetName, MethodType.Delete);
            if (methodData == null)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_DB_DELETE_NOT_IMPLEMENTED,
                    dbSetInfo.GetEntityType().Name, GetType().Name));
            }

            object dbEntity = Activator.CreateInstance(dbSetInfo.GetEntityType());
            _operations.UpdateEntityFromRowInfo(dbEntity, rowInfo, true);
            rowInfo.GetChangeState().Entity = dbEntity;
            rowInfo.GetChangeState().OriginalEntity = dbEntity;
            object instance = _operations.GetMethodOwner(dbSetInfo.dbSetName, methodData);
            object res = methodData.MethodInfo.Invoke(instance, new[] { dbEntity });
            if (res is Task)
            {
                await (res as Task);
            }
        }

        public async Task<bool> ValidateEntity(RunTimeMetadata metadata, RequestContext requestContext)
        {
            RowInfo rowInfo = requestContext.CurrentRowInfo;
            DbSetInfo dbSetInfo = rowInfo.GetDbSetInfo();
            IEnumerable<ValidationErrorInfo> errs1 = null;
            IEnumerable<ValidationErrorInfo> errs2 = null;
            LinkedList<string> mustBeChecked = new LinkedList<string>();
            LinkedList<string> skipCheckList = new LinkedList<string>();

            if (rowInfo.changeType == ChangeType.Added)
            {
                foreach (ParentChildNode pn in rowInfo.GetChangeState().ParentRows)
                {
                    foreach (FieldRel frel in pn.Association.fieldRels)
                    {
                        skipCheckList.AddLast(frel.childField);
                    }
                }
            }

            foreach (Field fieldInfo in dbSetInfo.fieldInfos)
            {
                _dataHelper.ForEachFieldInfo("", fieldInfo, (fullName, f) =>
                {
                    if (!f.GetIsIncludeInResult())
                    {
                        return;
                    }

                    if (f.fieldType == FieldType.Object || f.fieldType == FieldType.ServerCalculated)
                    {
                        return;
                    }

                    string value = _dataHelper.SerializeField(rowInfo.GetChangeState().Entity, fullName, f);

                    switch (rowInfo.changeType)
                    {
                        case ChangeType.Added:
                            {
                                bool isSkip = f.isAutoGenerated || skipCheckList.Any(n => n == fullName);
                                if (!isSkip)
                                {
                                    _validation.CheckValue(f, value);
                                    mustBeChecked.AddLast(fullName);
                                }
                            }
                            break;
                        case ChangeType.Updated:
                            {
                                bool isChanged = _operations.IsEntityValueChanged(rowInfo, fullName, f, out string newVal);
                                if (isChanged)
                                {
                                    _validation.CheckValue(f, newVal);
                                    mustBeChecked.AddLast(fullName);
                                }
                            }
                            break;
                    }
                });
            }

            rowInfo.GetChangeState().ChangedFieldNames = mustBeChecked.ToArray();

            MethodInfoData methodData = metadata.GetOperationMethodInfo(dbSetInfo.dbSetName, MethodType.Validate);
            if (methodData != null)
            {
                object instance = _operations.GetMethodOwner(dbSetInfo.dbSetName, methodData);
                object invokeRes = methodData.MethodInfo.Invoke(instance,
                    new[] { rowInfo.GetChangeState().Entity, rowInfo.GetChangeState().ChangedFieldNames });
                errs1 = (IEnumerable<ValidationErrorInfo>)await PropHelper.GetMethodResult(invokeRes);
            }

            if (errs1 == null)
            {
                errs1 = Enumerable.Empty<ValidationErrorInfo>();
            }

            Type validatorType = rowInfo.GetDbSetInfo().GetValidatorType();
            if (validatorType != null)
            {
                IServiceProvider sp = _domainService.ServiceContainer.ServiceProvider;
                IValidator validator = (IValidator)ActivatorUtilities.CreateInstance(sp, validatorType);
                try
                {
                    errs2 = await validator.ValidateModelAsync(
                        rowInfo.GetChangeState().Entity,
                        rowInfo.GetChangeState().ChangedFieldNames);
                }
                finally
                {
                    if (validator is IDisposable disposable) { disposable.Dispose(); }
                }
            }

            if (errs2 == null)
            {
                errs2 = Enumerable.Empty<ValidationErrorInfo>();
            }

            errs1 = errs1.Concat(errs2);

            rowInfo.GetChangeState().ValidationErrors = errs1.ToArray();

            return rowInfo.GetChangeState().ValidationErrors.Length == 0;
        }

        #endregion
    }
}