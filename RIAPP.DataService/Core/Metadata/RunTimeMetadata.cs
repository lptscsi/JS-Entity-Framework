using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RIAPP.DataService.Core.Metadata
{
    public class RunTimeMetadata(
        DbSetInfoMap dbSets,
        AssociationMap associations,
        MethodMap svcMethods,
        OperationalMethods operMethods,
        string[] typeScriptImports)
    {
        private readonly OperationalMethods _operMethods = operMethods;
        private readonly MethodMap _svcMethods = svcMethods;

        public string[] TypeScriptImports
        {
            get;
        } = typeScriptImports;

        /// <summary>
        /// Lookup table for <see cref="DbSetInfo"/> indexed by entity type
        /// </summary>
        public ILookup<Type, DbSetInfo> DbSetsByEntityType
        {
            get;
        }

        public MethodDescription GetQueryMethod(string dbSetName, string name)
        {
            MethodDescription method = _svcMethods.GetQueryMethod(dbSetName, name);
            if (method == null)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_QUERY_NAME_INVALID, name));
            }
            return method;
        }

        public IEnumerable<MethodDescription> GetQueryMethods(string dbSetName)
        {
            return _svcMethods.GetQueryMethods(dbSetName);
        }

        public MethodDescription GetInvokeMethod(string name)
        {
            MethodDescription method = _svcMethods.GetInvokeMethod(name);
            if (method == null)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_METH_NAME_INVALID, name));
            }
            return method;
        }

        public IEnumerable<MethodDescription> GetInvokeMethods()
        {
            return _svcMethods.GetInvokeMethods();
        }

        public MethodInfoData GetOperationMethodInfo(string dbSetName, MethodType methodType)
        {
            return _operMethods.GetMethod(dbSetName, methodType);
        }

        public DbSetInfoMap DbSets { get; } = dbSets;

        public AssociationMap Associations { get; } = associations;

        public MethodsList MethodDescriptions => new(_svcMethods.Values);

    }
}