using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RIAPP.DataService.Core.CodeGen
{
    /// <summary>
    /// Провайдер функции генерации кода на typescript
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="owner"></param>
    /// <param name="serializer"></param>
    /// <param name="dataHelper"></param>
    /// <param name="valueConverter"></param>
    /// <param name="lang"></param>
    /// <param name="jriappImportPath"></param>
    /// <param name="clientTypes"></param>
    public class TypeScriptProvider<TService>(
        IMetaDataProvider owner,
        ISerializer serializer,
        IDataHelper dataHelper,
        IValueConverter valueConverter,
        string lang,
        string jriappImportPath,
        Func<IEnumerable<Type>> clientTypes) : ICodeGenProvider<TService>
         where TService : BaseDomainService
    {
        private readonly Func<IEnumerable<Type>> _clientTypes = clientTypes ?? (() => Enumerable.Empty<Type>());
        private readonly ISerializer _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        private readonly IDataHelper _dataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
        private readonly IValueConverter _valueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));

        public string Lang => lang;
        public IMetaDataProvider Owner { get; } = owner ?? throw new ArgumentNullException(nameof(owner));

        public virtual string GenerateScript(string comment = null, bool isDraft = false)
        {
            RunTimeMetadata metadata = Owner.GetMetadata();
            TypeScriptHelper helper = new(
                _serializer,
                _dataHelper,
                _valueConverter,
                metadata,
                jriappImportPath,
                _clientTypes()
            );
            return helper.CreateTypeScript(comment);
        }
    }

    public class TypeScriptProviderFactory<TService>(
        IServiceContainer<TService> serviceContainer,
        string jriappImportPath,
        Func<IEnumerable<Type>> clientTypes = null
        ) : ICodeGenProviderFactory<TService>
         where TService : BaseDomainService
    {
        private readonly IServiceContainer<TService> _serviceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));

        public ICodeGenProvider Create(BaseDomainService owner)
        {
            return Create((TService)owner);
        }

        public ICodeGenProvider<TService> Create(TService owner)
        {
            return new TypeScriptProvider<TService>(
                owner,
                _serviceContainer.Serializer,
                _serviceContainer.GetDataHelper(),
                _serviceContainer.GetValueConverter(),
                Lang,
                JriappImportPath,
                clientTypes);
        }

        public string Lang => "ts";

        public string JriappImportPath => jriappImportPath;
    }
}
