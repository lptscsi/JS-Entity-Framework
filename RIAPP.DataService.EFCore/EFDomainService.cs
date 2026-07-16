using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RIAPP.DataService.Core;
using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.EFCore.Utils;
using RIAPP.DataService.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using DataType = RIAPP.DataService.Core.Types.DataType;

namespace RIAPP.DataService.EFCore
{
    /// <summary>
    /// Базовый класс сервиса для работы с данными посредством EF
    /// </summary>
    /// <typeparam name="TDB"></typeparam>
    /// <param name="serviceContainer"></param>
    /// <param name="db"></param>
    public abstract class EFDomainService<TDB>(IServiceContainer serviceContainer, TDB db = default) 
        : BaseDomainService(serviceContainer)
       where TDB : DbContext
    {
        private bool _ownsDb;
        private readonly Lock SyncLock = new();

        public TDB DB
        {
            get
            {
                lock (SyncLock)
                {
                    if (db == null)
                    {
                        db = CreateDataContext();
                        if (db != null)
                        {
                            _ownsDb = true;
                        }
                    }
                    return db;
                }
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (db != null && _ownsDb)
            {
                db.Dispose();
                db = null;
                _ownsDb = false;
            }

            base.Dispose(isDisposing);
        }

        #region Overridable Methods

        protected virtual TDB CreateDataContext()
        {
            return Activator.CreateInstance<TDB>();
        }


        protected override async Task ExecuteChangeSet()
        {
            try
            {
                using (TransactionScope transScope = new(TransactionScopeOption.RequiresNew,
                    new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadCommitted,
                        Timeout = TimeSpan.FromMinutes(1.0)
                    }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    // can be commented if not needed
                    this.ValidateEntities();

                    await DB.SaveChangesAsync();

                    transScope.Complete();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException(ex.Message);
            }
        }

        /// <summary>
        /// Возвращает метаданные для работы сервиса
        /// </summary>
        /// <param name="isDraft"></param>
        /// <param name="dataServiceEntityTypes"></param>
        /// <returns></returns>
        protected override DesignTimeMetadata GetDesignTimeMetadata(bool isDraft, List<Type> dataServiceEntityTypes = null)
        {
            DesignTimeMetadata metadata = new();
            IModel dbModel = DB.Model;
            IEntityType[] allEntities = [.. dbModel.GetEntityTypes()];
            IEntityType[] plainEntities = [.. allEntities.Where(t => !t.IsOwned())];

            Dictionary<string, IEntityType> ownedTypesMap = allEntities
                .Where(t => t.IsOwned())
                .ToDictionary(t => t.Name);

            HashSet<Type> chosenTypes = dataServiceEntityTypes?.ToHashSet();

            foreach (IEntityType entityInfo in plainEntities)
            {
                if (chosenTypes != null && !chosenTypes.Contains(entityInfo.ClrType))
                {
                    continue;
                }

                IEnumerable<IProperty> edmProps = entityInfo.GetProperties()
                    .Where(p => !p.IsShadowProperty())
                    .ToArray();

                // IEnumerable<string> edmProps1 = entityInfo.GetNavigations().Select(n => n.ForeignKey.DeclaringEntityType.Name).ToArray();
                IEnumerable<INavigation> ownedTypes = entityInfo.GetNavigations()
                    .Where(n => ownedTypesMap.ContainsKey(n.ForeignKey.DeclaringEntityType.Name))
                    .ToArray();

                DbSetInfo dbSetInfo = new()
                {
                    dbSetName = entityInfo.ClrType.Name
                };

                dbSetInfo.SetEntityType(entityInfo.ClrType);
                metadata.DbSets.Add(dbSetInfo);
                GenerateFieldInfos(metadata, entityInfo, dbSetInfo, edmProps, ownedTypes, ownedTypesMap);
                GenerateAssociations(metadata, entityInfo, dbSetInfo);
            }

            return metadata;
        }

        #endregion

        #region helper methods

        #region Complex Type Fields
        private void UpdateNestedFieldInfo(Field fieldInfo, PropertyInfo propInfo)
        {
            fieldInfo.fieldType = FieldType.None;

            ColumnAttribute colAttr = propInfo.GetCustomAttributes<ColumnAttribute>().FirstOrDefault();
            if (colAttr != null && !string.IsNullOrEmpty(colAttr.TypeName))
            {
                if (colAttr.TypeName.ToLower() == "date")
                {
                    fieldInfo.dataType = DataType.Date;
                }
            }

            fieldInfo.isNullable = propInfo.PropertyType.IsNullableType() || (propInfo.PropertyType == typeof(string) && !propInfo.GetCustomAttributes<RequiredAttribute>().Any());
            fieldInfo.isReadOnly = fieldInfo.isAutoGenerated || propInfo.GetSetMethod() == null;

            StringLengthAttribute strLenAttr = propInfo.GetCustomAttributes<StringLengthAttribute>().FirstOrDefault();
            if (strLenAttr != null && strLenAttr.MaximumLength > 0)
            {
                fieldInfo.maxLength = strLenAttr.MaximumLength;
            }
            else
            {
                MaxLengthAttribute maxLenAttr = propInfo.GetCustomAttributes<MaxLengthAttribute>().FirstOrDefault();
                if (maxLenAttr != null && maxLenAttr.Length > 0)
                {
                    fieldInfo.maxLength = maxLenAttr.Length;
                }
            }

            NotMappedAttribute notmappedAttr = propInfo.GetCustomAttributes<NotMappedAttribute>().FirstOrDefault();
            if (notmappedAttr != null)
            {
                fieldInfo.fieldType = FieldType.ServerCalculated;
            }
        }

        private void GenerateNestedFieldInfos(Field parentField, Type nestedType)
        {
            PropertyInfo[] nestedProps = nestedType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToArray();
            DataService.Utils.IValueConverter valueConverter = ServiceContainer.ValueConverter;

            foreach (PropertyInfo propInfo in nestedProps)
            {
                bool isContinue = false;
                Field fieldInfo = new Field { fieldName = propInfo.Name };
                try
                {
                    fieldInfo.dataType = propInfo.PropertyType.IsArrayType() ? DataType.None : valueConverter.DataTypeFromType(propInfo.PropertyType);
                }
                catch (UnsupportedTypeException)
                {
                    if (propInfo.PropertyType.IsClass)
                    {
                        // complex Type field
                        GenerateNestedFieldInfos(fieldInfo, propInfo.PropertyType);
                    }
                    else
                    {
                        isContinue = true;
                    }
                }

                if (isContinue)
                {
                    continue;
                }

                if (fieldInfo.fieldType != FieldType.Object)
                {
                    UpdateNestedFieldInfo(fieldInfo, propInfo);
                }

                parentField.fieldType = FieldType.Object;
                parentField.nested.Add(fieldInfo);
            }
        }
        #endregion

        private static void GenerateAssociation(DesignTimeMetadata metadata, IEntityType entityInfo, DbSetInfo dbSetInfo, INavigation childToParentNav)
        {
            INavigation inverseNavigation = childToParentNav.Inverse; //.FindInverse();
            string assoc_name = string.Format("{0}_{1}", inverseNavigation.Name, childToParentNav.Name);
            // Console.WriteLine($"Generate association: {assoc_name}");

            Association assoc = metadata.Associations.Where(a => a.name == assoc_name).FirstOrDefault();
            if (assoc == null)
            {
                IProperty[] principalProps = inverseNavigation.ForeignKey.PrincipalKey.Properties.ToArray();
                IProperty[] childProps = childToParentNav.ForeignKey.Properties.ToArray();

                assoc = new Association
                {
                    name = assoc_name
                };
                IEntityType parentEntity = inverseNavigation.DeclaringEntityType;
                IEntityType childEntity = childToParentNav.DeclaringEntityType;

                assoc.parentDbSetName = parentEntity.ClrType.Name;
                assoc.childDbSetName = childEntity.ClrType.Name;
                assoc.childToParentName = childToParentNav.Name;

                if (inverseNavigation != null)
                {
                    assoc.parentToChildrenName = inverseNavigation.Name;
                }

                int i = 0;
                foreach (IProperty pkProp in principalProps)
                {
                    FieldRel frel = new FieldRel
                    {
                        childField = childProps?[i]?.Name,
                        parentField = pkProp.Name
                    };
                    assoc.fieldRels.Add(frel);
                    ++i;
                }

                metadata.Associations.Add(assoc);
            }
        }

        private static void UpdateFieldInfo(Field fieldInfo, IProperty edmProp)
        {
            fieldInfo.isAutoGenerated = IsAutoGenerated(edmProp);
            fieldInfo.isNullable = edmProp.IsNullable;
            fieldInfo.isReadOnly = edmProp.GetAfterSaveBehavior() == PropertySaveBehavior.Throw;
            fieldInfo.allowClientDefault = !fieldInfo.isAutoGenerated && fieldInfo.isReadOnly && edmProp.GetBeforeSaveBehavior() == PropertySaveBehavior.Save;
            int? maxLen = edmProp.GetMaxLength();
            if (maxLen.HasValue)
            {
                fieldInfo.maxLength = maxLen.Value;
            }

            if (edmProp.IsConcurrencyToken)
            {
                fieldInfo.fieldType = FieldType.RowTimeStamp;
            }
            else if (IsNotMapped(edmProp))
            {
                fieldInfo.fieldType = FieldType.ServerCalculated;
            }
            else
            {
                fieldInfo.fieldType = FieldType.None;
            }
        }

        private void GenerateOwnedTypeFieldInfos(Field parentField, IEntityType ownedType, IEnumerable<INavigation> nestedOwnedTypes, Dictionary<string, IEntityType> ownedMap)
        {
            parentField.fieldType = FieldType.Object;

            foreach (INavigation ownedNavigation in nestedOwnedTypes)
            {
                Field fieldInfo = new Field { fieldName = ownedNavigation.Name };
                string nm = ownedNavigation.ForeignKey.DeclaringEntityType.Name;
                IEntityType nestedOwnedType2 = ownedMap[nm];
                IEnumerable<INavigation> nestedOwnedTypes2 = nestedOwnedType2.GetNavigations().Where(n => ownedMap.ContainsKey(n.ForeignKey.DeclaringEntityType.Name)).ToArray();
                GenerateOwnedTypeFieldInfos(fieldInfo, nestedOwnedType2, nestedOwnedTypes2, ownedMap);
                parentField.nested.Add(fieldInfo);
            }

            IProperty[] edmProps = ownedType.GetProperties().Where(p => !p.IsShadowProperty()).ToArray();
            DataService.Utils.IValueConverter valueConverter = ServiceContainer.ValueConverter;

            foreach (IProperty edmProp in edmProps)
            {
                bool isContinue = false;
                Field fieldInfo = new Field { fieldName = edmProp.Name };

                try
                {
                    fieldInfo.dataType = edmProp.ClrType.IsArrayType() ? DataType.None : valueConverter.DataTypeFromType(edmProp.ClrType);
                }
                catch (UnsupportedTypeException)
                {
                    // Console.WriteLine($"{edmProp.Name} unsupported type {edmProp.ClrType.FullName}");

                    // complex type
                    if (edmProp.ClrType.IsClass)
                    {
                        GenerateNestedFieldInfos(fieldInfo, edmProp.ClrType);
                    }
                    else
                    {
                        isContinue = true;
                    }
                }

                if (isContinue)
                {
                    continue;
                }

                if (fieldInfo.fieldType != FieldType.Object)
                {
                    UpdateFieldInfo(fieldInfo, edmProp);
                }

                parentField.nested.Add(fieldInfo);
            }
        }

        private void GenerateFieldInfos(DesignTimeMetadata metadata, IEntityType entityInfo, DbSetInfo dbSetInfo, IEnumerable<IProperty> edmProps, IEnumerable<INavigation> ownedTypes, Dictionary<string, IEntityType> ownedMap)
        {
            short pkNum = 0;
            // Console.WriteLine($"Generate fields: {entityInfo.Name} FieldsCount: {edmProps.Count()}");

            foreach (INavigation ownedNavigation in ownedTypes)
            {
                Field fieldInfo = new Field { fieldName = ownedNavigation.Name };
                string nm = ownedNavigation.ForeignKey.DeclaringEntityType.Name;
                IEntityType ownedType = ownedMap[nm];
                IEnumerable<INavigation> nestedOwnedTypes = ownedType.GetNavigations().Where(n => ownedMap.ContainsKey(n.ForeignKey.DeclaringEntityType.Name)).ToArray();
                GenerateOwnedTypeFieldInfos(fieldInfo, ownedType, nestedOwnedTypes, ownedMap);
                dbSetInfo.fieldInfos.Add(fieldInfo);
            }

            DataService.Utils.IValueConverter valueConverter = ServiceContainer.ValueConverter;

            foreach (IProperty edmProp in edmProps)
            {
                bool isContinue = false;
                Field fieldInfo = new Field { fieldName = edmProp.Name };

                try
                {
                    fieldInfo.dataType = edmProp.ClrType.IsArrayType() ? DataType.None : valueConverter.DataTypeFromType(edmProp.ClrType);
                }
                catch (UnsupportedTypeException)
                {
                    // Console.WriteLine($"{edmProp.Name} unsupported type {edmProp.ClrType.FullName}");
                    isContinue = true;
                }

                if (isContinue)
                {
                    continue;
                }

                if (fieldInfo.fieldType != FieldType.Object)
                {
                    UpdateFieldInfo(fieldInfo, edmProp);

                    if (edmProp.IsPrimaryKey())
                    {
                        ++pkNum;
                        fieldInfo.isPrimaryKey = pkNum;
                        fieldInfo.isReadOnly = true;
                    }
                }

                dbSetInfo.fieldInfos.Add(fieldInfo);
            }
        }

        private static void GenerateAssociations(DesignTimeMetadata metadata, IEntityType entityInfo, DbSetInfo dbSetInfo)
        {
            IEnumerable<INavigation> childToParentNavs = entityInfo
                .GetNavigations()
                .Where(n => n.IsOnDependent /*IsDependentToPrincipal() */);

            foreach (INavigation childToParentNav in childToParentNavs)
            {
                GenerateAssociation(metadata, entityInfo, dbSetInfo, childToParentNav);
            }
        }

        private static bool IsAutoGenerated(IProperty prop)
        {
            return prop.ValueGenerated == ValueGenerated.OnAdd;
        }

        private static bool IsNotMapped(IProperty prop)
        {
            return prop.GetBeforeSaveBehavior() == PropertySaveBehavior.Ignore && prop.GetAfterSaveBehavior() == PropertySaveBehavior.Ignore;
        }

        #endregion
    }
}
