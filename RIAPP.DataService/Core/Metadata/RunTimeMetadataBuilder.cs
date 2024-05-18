using RIAPP.DataService.Annotations;
using RIAPP.DataService.Core.CodeGen;
using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Resources;
using RIAPP.DataService.Utils;
using RIAPP.DataService.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RIAPP.DataService.Core.Metadata
{
    public class RunTimeMetadataBuilder
    {
        #region Fields

        private readonly Type domainServiceType;
        private readonly DesignTimeMetadata designTimeMetadata;
        private readonly IDataHelper dataHelper;
        private readonly IValueConverter valueConverter;

        #endregion

        public RunTimeMetadataBuilder(Type domainServiceType,
            DesignTimeMetadata designTimeMetadata,
            IDataHelper dataHelper,
            IValueConverter valueConverter)
        {
            this.domainServiceType = domainServiceType;
            this.designTimeMetadata = designTimeMetadata;
            this.dataHelper = dataHelper;
            this.valueConverter = valueConverter;
        }

        public RunTimeMetadata Build()
        {
            HashSet<string> dbSetNames = new HashSet<string>();

            foreach (DbSetInfo dbSetInfo in designTimeMetadata.DbSets)
            {
                if (dbSetNames.Contains(dbSetInfo.dbSetName))
                {
                    throw new InvalidOperationException($"Metadata has a duplicated DbSetName: {dbSetInfo.dbSetName}");
                }

                dbSetNames.Add(dbSetInfo.dbSetName);
            }

            ILookup<Type, string> dbSetsByTypeLookUp = designTimeMetadata.DbSets
                .ToLookup(v => v.GetEntityType(), v => v.dbSetName);

            MethodMap svcMethods = new MethodMap();
            OperationalMethods operMethods = new OperationalMethods();

            foreach (var dbSet in designTimeMetadata.DbSets)
            {
                Type handlerType = dbSet.GetHandlerType();
                if (handlerType != null)
                {
                    Type[] interfaces = handlerType.GetInterfaces();
                    bool isDataManager = interfaces.Any(i => i.IsAssignableTo(typeof(IDataManager)));
                    if (!isDataManager)
                    {
                        throw new InvalidOperationException($"Invalid handler type {handlerType.Name} for DbSet {dbSet.dbSetName}");
                    }
                }

                Type validatorType = dbSet.GetValidatorType();
                if (validatorType != null)
                {
                    Type[] interfaces = validatorType.GetInterfaces();
                    bool isValidator = interfaces.Any(i => i.IsAssignableTo(typeof(IValidator)));
                    if (!isValidator)
                    {
                        throw new InvalidOperationException($"Invalid validator type {validatorType.Name} for DbSet {dbSet.dbSetName}");
                    }
                }

                if (handlerType != null)
                {
                    ProcessHandlerMethodDescriptions(handlerType, svcMethods, operMethods, dbSet.dbSetName);
                }
            }

            ProcessDataServiceMethodDescriptions(domainServiceType, svcMethods, operMethods, dbSetNames, dbSetsByTypeLookUp);

            operMethods.MakeReadOnly();
            svcMethods.MakeReadOnly();

            List<DbSetRec> dbSetList = new List<DbSetRec>();

            foreach (var dbSet in designTimeMetadata.DbSets)
            {
                FieldsList fieldList = new FieldsList(dbSet.fieldInfos);
                fieldList.Initialize(dataHelper);
                dbSetList.Add(new DbSetRec(dbSet, fieldList));
            }

            IDictionary<string, DbSetRec> dbSetRecMap = dbSetList.ToDictionary(v => v.dbSetInfo.dbSetName);

            AssociationMap associations = new AssociationMap();

            foreach (Association assoc in designTimeMetadata.Associations)
            {
                ProcessAssociation(assoc, dbSetRecMap, associations);
            }

            DbSetInfoMap dbSets = new DbSetInfoMap(dbSetRecMap);

            return new RunTimeMetadata(dbSets, associations, svcMethods, operMethods, designTimeMetadata.TypeScriptImports.ToArray());
        }

        private static readonly Dictionary<Type, MethodType> _attributeMap = new Dictionary<Type, MethodType>()
        {
            { typeof(QueryAttribute), MethodType.Query },
            { typeof(InvokeAttribute), MethodType.Invoke },
            { typeof(InsertAttribute), MethodType.Insert },
            { typeof(UpdateAttribute), MethodType.Update },
            { typeof(DeleteAttribute), MethodType.Delete },
            { typeof(ValidateAttribute), MethodType.Validate },
            { typeof(RefreshAttribute), MethodType.Refresh }
        };

        private static MethodType GetMethodType(MethodInfo methodInfo)
        {
            return _attributeMap.FirstOrDefault(kv => methodInfo.IsDefined(kv.Key, false)).Value;
        }

        /// <summary>
        /// Gets CRUD methods from DataManager which implements IDataManager interface
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        private static IDictionary<MethodType, MethodInfoData> GetHandlerCRUDMethods(Type handlerType)
        {
            // removes duplicates of the method (there are could be synch and async methods)
            IDictionary<MethodType, MethodInfoData> methodTypes = new Dictionary<MethodType, MethodInfoData>();

            void AddMethodInfoData(MethodType methodType, MethodInfo methodInfo)
            {
                if (!methodTypes.ContainsKey(methodType))
                {
                    methodTypes.Add(methodType, new MethodInfoData
                    {
                        OwnerType = handlerType,
                        MethodInfo = methodInfo,
                        MethodType = methodType,
                        IsInDataManager = true
                    });
                }
            };

            void AddMethod(MethodInfo method)
            {
                switch (method.Name)
                {
                    case "Insert":
                    case "InsertAsync":
                        AddMethodInfoData(MethodType.Insert, method);
                        break;
                    case "Update":
                    case "UpdateAsync":
                        AddMethodInfoData(MethodType.Update, method);
                        break;
                    case "Delete":
                    case "DeleteAsync":
                        AddMethodInfoData(MethodType.Delete, method);
                        break;
                }
            };

            MethodInfo[] methods = handlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods)
            {
                AddMethod(method);
            }

            return methodTypes;
        }

        /// <summary>
        /// Gets all operational methods from supplied type (DataService or DataManager)
        /// </summary>
        /// <param name="fromType"></param>
        /// <returns></returns>
        private static IEnumerable<MethodInfoData> GetMethodsFromType(Type fromType, bool isDataManager)
        {
            MethodInfo[] methodInfos = fromType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            Type[] interfTypes = fromType.GetInterfaces();

            IEnumerable<MethodInfoData> allList = methodInfos.Select(m => new MethodInfoData
            {
                OwnerType = fromType,
                MethodInfo = m,
                MethodType = GetMethodType(m),
                IsInDataManager = isDataManager
            }).Where(m => m.MethodType != MethodType.None);

            IEnumerable<MethodInfoData> UnionMethods(IEnumerable<MethodInfoData> list, IDictionary<MethodType, MethodInfoData> crudMethods)
            {
                foreach (KeyValuePair<MethodType, MethodInfoData> kv in crudMethods)
                {
                    yield return kv.Value;
                }

                foreach (MethodInfoData item in list)
                {
                    if (!crudMethods.ContainsKey(item.MethodType))
                    {
                        yield return item;
                    }
                }
            };

            IEnumerable<MethodInfoData> result;


            if (isDataManager)
            {
                IDictionary<MethodType, MethodInfoData> crudMethods = GetHandlerCRUDMethods(fromType);
                result = UnionMethods(allList, crudMethods).ToArray();
            }
            else
            {
                result = allList.ToArray();
            }

            foreach (MethodInfoData data in result)
            {
                switch (data.MethodType)
                {
                    case MethodType.Query:
                        data.EntityType = data.MethodInfo.ReturnType.GetTaskResultType().GetGenericArguments().First();
                        break;
                    case MethodType.Invoke:
                        data.EntityType = null;
                        break;
                    case MethodType.Refresh:
                        data.EntityType = data.MethodInfo.ReturnType.GetTaskResultType();
                        break;
                    case MethodType.Insert:
                    case MethodType.Update:
                    case MethodType.Delete:
                    case MethodType.Validate:
                        data.EntityType = data.MethodInfo.GetParameters().First().ParameterType;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown Method Type: {data.MethodType}");
                }
            }

            return result;
        }

        private void ProcessHandlerMethodDescriptions(Type handlerType, MethodMap svcMethods, OperationalMethods operMethods, string dbSetName)
        {
            IEnumerable<MethodInfoData> allMethods = GetMethodsFromType(handlerType, true);

            // For handlers only query methods (no invoke ones, because they belong to the dataservice only, and the handlers don't have them) 
            MethodsList svcMethInfos = allMethods.GetSvcMethods(valueConverter, MethodType.Query);

            InitHandlerSvcMethods(svcMethInfos, svcMethods, dbSetName);

            IEnumerable<MethodInfoData> otherMethods = allMethods.GetMethods(MethodType.Insert | MethodType.Update | MethodType.Delete | MethodType.Refresh | MethodType.Validate);

            InitHandlerOperMethods(otherMethods, operMethods, dbSetName);
        }

        private void ProcessDataServiceMethodDescriptions(
            Type serviceType,
            MethodMap svcMethods,
            OperationalMethods operMethods,
            HashSet<string> dbSetNames,
            ILookup<Type, string> dbSetsByTypeLookUp)
        {
            IEnumerable<MethodInfoData> allMethods = GetMethodsFromType(serviceType, false);

            // For DataService query and invoke only 
            MethodsList svcMethInfos = allMethods.GetSvcMethods(valueConverter, MethodType.Query | MethodType.Invoke);

            InitSvcMethods(svcMethInfos, svcMethods, dbSetNames, dbSetsByTypeLookUp);

            IEnumerable<MethodInfoData> otherMethods = allMethods.GetMethods(MethodType.Insert | MethodType.Update | MethodType.Delete | MethodType.Refresh | MethodType.Validate);

            InitOperMethods(otherMethods, operMethods, dbSetNames, dbSetsByTypeLookUp);
        }

        private void ProcessAssociation(Association assoc, IDictionary<string, DbSetRec> dbSetMap, AssociationMap associations)
        {
            if (string.IsNullOrWhiteSpace(assoc.name))
            {
                throw new DomainServiceException(ErrorStrings.ERR_ASSOC_EMPTY_NAME);
            }
            if (!dbSetMap.ContainsKey(assoc.parentDbSetName))
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_ASSOC_INVALID_PARENT, assoc.name,
                    assoc.parentDbSetName));
            }
            if (!dbSetMap.ContainsKey(assoc.childDbSetName))
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_ASSOC_INVALID_CHILD, assoc.name,
                    assoc.childDbSetName));
            }

            DbSetRec childDb = dbSetMap[assoc.childDbSetName];
            DbSetRec parentDb = dbSetMap[assoc.parentDbSetName];
            IReadOnlyDictionary<string, Field> parentDbFields = parentDb.fieldList.GetFieldByNames();
            IReadOnlyDictionary<string, Field> childDbFields = childDb.fieldList.GetFieldByNames();

            //check navigation field
            //dont allow to define  it explicitly, the association adds the field by itself (implicitly)
            if (!string.IsNullOrEmpty(assoc.childToParentName) && childDbFields.ContainsKey(assoc.childToParentName))
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_ASSOC_INVALID_NAV_FIELD, assoc.name,
                    assoc.childToParentName));
            }

            //check navigation field
            //dont allow to define  it explicitly, the association adds the field by itself (implicitly)
            if (!string.IsNullOrEmpty(assoc.parentToChildrenName) &&
                parentDbFields.ContainsKey(assoc.parentToChildrenName))
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_ASSOC_INVALID_NAV_FIELD, assoc.name,
                    assoc.parentToChildrenName));
            }

            if (!string.IsNullOrEmpty(assoc.parentToChildrenName) && !string.IsNullOrEmpty(assoc.childToParentName) &&
                assoc.childToParentName == assoc.parentToChildrenName)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_ASSOC_INVALID_NAV_FIELD, assoc.name,
                    assoc.parentToChildrenName));
            }

            foreach (FieldRel frel in assoc.fieldRels)
            {
                if (!parentDbFields.ContainsKey(frel.parentField))
                {
                    throw new DomainServiceException(string.Format(ErrorStrings.ERR_ASSOC_INVALID_PARENT_FIELD,
                        assoc.name, frel.parentField));
                }
                if (!childDbFields.ContainsKey(frel.childField))
                {
                    throw new DomainServiceException(string.Format(ErrorStrings.ERR_ASSOC_INVALID_CHILD_FIELD,
                        assoc.name, frel.childField));
                }
            }

            //indexed by Name
            associations.Add(assoc.name, assoc);

            if (!string.IsNullOrEmpty(assoc.childToParentName))
            {
                StringBuilder sb = new StringBuilder(120);
                string dependentOn =
                    assoc.fieldRels.Aggregate(sb, (a, b) => a.Append((a.Length == 0 ? "" : ",") + b.childField),
                        a => a).ToString();

                //add navigation field to dbSet's field collection
                Field field = new Field
                {
                    fieldName = assoc.childToParentName,
                    fieldType = FieldType.Navigation,
                    dataType = DataType.None,
                    dependentOn = dependentOn
                };

                field.SetTypeScriptDataType(TypeScriptHelper.GetEntityInterfaceName(parentDb.dbSetInfo.dbSetName));
                childDb.fieldList.Add(field);
            }

            if (!string.IsNullOrEmpty(assoc.parentToChildrenName))
            {
                StringBuilder sb = new StringBuilder(120);
                Field field = new Field
                {
                    fieldName = assoc.parentToChildrenName,
                    fieldType = FieldType.Navigation,
                    dataType = DataType.None
                };

                field.SetTypeScriptDataType($"{TypeScriptHelper.GetEntityInterfaceName(childDb.dbSetInfo.dbSetName)}[]");
                //add navigation field to dbSet's field collection
                parentDb.fieldList.Add(field);
            }
        }

        private void InitHandlerSvcMethods(MethodsList methods, MethodMap svcMethods, string dbSetName)
        {
            methods.ForEach(methodDescription =>
            {
                if (methodDescription.isQuery)
                {
                    svcMethods.Add(dbSetName, methodDescription);
                }
                else
                {
                    svcMethods.Add("", methodDescription);
                }
            });
        }

        private void InitHandlerOperMethods(IEnumerable<MethodInfoData> methods, OperationalMethods operMethods, string dbSetName)
        {
            foreach (MethodInfoData methodData in methods)
            {
                operMethods.Add(dbSetName, methodData);
            }
        }

        private void InitSvcMethods(MethodsList methods, MethodMap svcMethods, HashSet<string> dbSetNames, ILookup<Type, string> dbSetsByTypeLookUp)
        {
            methods.ForEach(methodDescription =>
            {
                if (methodDescription.isQuery)
                {
                    DbSetNameAttribute dbSetAttribute = (DbSetNameAttribute)methodDescription
                       .GetMethodData()
                       .MethodInfo
                       .GetCustomAttributes(typeof(DbSetNameAttribute), false)
                       .FirstOrDefault();

                    string dbSetName = dbSetAttribute?.DbSetName;

                    if (!string.IsNullOrWhiteSpace(dbSetName))
                    {
                        if (!dbSetNames.Contains(dbSetName))
                        {
                            throw new DomainServiceException(string.Format("Can not determine the DbSet for a query method: {0} by DbSetName {1}", methodDescription.methodName, dbSetName));
                        }

                        svcMethods.Add(dbSetName, methodDescription);
                    }
                    else
                    {
                        System.Type entityType = methodDescription.GetMethodData().EntityType;

                        IEnumerable<string> entityTypeDbSets = dbSetsByTypeLookUp[entityType];

                        int cnt = 0;
                        foreach (string dbSetName2 in entityTypeDbSets)
                        {
                            svcMethods.Add(dbSetName2, methodDescription);
                            ++cnt;
                        }

                        if (cnt == 0)
                        {
                            throw new DomainServiceException(string.Format("Can not determine the DbSet for a query method: {0}", methodDescription.methodName));
                        }
                    }
                }
                else
                {
                    // Invoke methods don't belong to a DbSet, they belong to the whole DataService
                    svcMethods.Add("", methodDescription);
                }
            });
        }

        private void InitOperMethods(IEnumerable<MethodInfoData> methods, OperationalMethods operMethods, HashSet<string> dbSetNames, ILookup<Type, string> dbSetsByTypeLookUp)
        {
            MethodInfoData[] otherMethods = methods.ToArray();

            Array.ForEach(otherMethods, methodData =>
            {
                DbSetNameAttribute dbSetAttribute = (DbSetNameAttribute)methodData
                      .MethodInfo
                      .GetCustomAttributes(typeof(DbSetNameAttribute), false)
                      .FirstOrDefault();

                string dbSetName = dbSetAttribute?.DbSetName;

                if (!string.IsNullOrWhiteSpace(dbSetName))
                {
                    if (!dbSetNames.Contains(dbSetName))
                    {
                        throw new DomainServiceException(string.Format("Can not determine the DbSet for a query method: {0} by DbSetName {1}", methodData.MethodInfo.Name, dbSetName));
                    }

                    operMethods.Add(dbSetName, methodData);
                }
                else if (methodData.EntityType != null)
                {
                    IEnumerable<string> dbSets = dbSetsByTypeLookUp[methodData.EntityType];

                    foreach (string dbSetName2 in dbSets)
                    {
                        operMethods.Add(dbSetName2, methodData);
                    }
                }
                else
                {
                    operMethods.Add("", methodData);
                }
            });
        }
    }
}