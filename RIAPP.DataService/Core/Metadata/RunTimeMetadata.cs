using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RIAPP.DataService.Core.Metadata
{
    /// <summary>
    /// Метаданные используемые в процессе выполнения сервиса
    /// </summary>
    /// <param name="dbSets"></param>
    /// <param name="dbSetsByEntityType"></param>
    /// <param name="associations"></param>
    /// <param name="svcMethods"></param>
    /// <param name="operMethods"></param>
    /// <param name="typeScriptImports"></param>
    public class RunTimeMetadata(DbSetsDictionary dbSets,
        ILookup<Type, DbSetInfo> dbSetsByEntityType,
        AssociationsDictionary associations,
        MethodMap svcMethods,
        OperationalMethods operMethods,
        string[] typeScriptImports)
    {
        public string[] TypeScriptImports => typeScriptImports;

        /// <summary>
        /// Lookup table for <see cref="DbSetInfo"/> indexed by entity type
        /// </summary>
        public ILookup<Type, DbSetInfo> DbSetsByEntityType => dbSetsByEntityType;

        public MethodDescription GetQueryMethod(string dbSetName, string name)
        {
            MethodDescription method = svcMethods.GetQueryMethod(dbSetName, name);
            if (method == null)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_QUERY_NAME_INVALID, name));
            }
            return method;
        }

        public IEnumerable<MethodDescription> GetQueryMethods(string dbSetName)
        {
            return svcMethods.GetQueryMethods(dbSetName);
        }

        public MethodDescription GetInvokeMethod(string name)
        {
            MethodDescription method = svcMethods.GetInvokeMethod(name) ?? 
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_METH_NAME_INVALID, name));
            return method;
        }

        public IEnumerable<MethodDescription> GetInvokeMethods()
        {
            return svcMethods.GetInvokeMethods();
        }

        public MethodInfoData GetOperationMethodInfo(string dbSetName, MethodType methodType)
        {
            return operMethods.GetMethod(dbSetName, methodType);
        }

        public DbSetsDictionary DbSets { get; } = dbSets;

        public AssociationsDictionary Associations { get; } = associations;

        public MethodsList MethodDescriptions => [.. svcMethods.Values];

    }
}