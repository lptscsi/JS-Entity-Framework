using RIAPP.DataService.Core.Metadata;

namespace RIAPP.DataService.Core.CodeGen
{
    /// <summary>
    /// Провайдер функции генерации кода на XML (xaml)
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="owner"></param>
    /// <param name="lang"></param>
    public class XamlProvider<TService>(IMetaDataProvider owner, string lang) : ICodeGenProvider<TService>
         where TService : BaseDomainService
    {
        public string Lang => lang;

        public IMetaDataProvider Owner => owner;

        public virtual string GenerateScript(string comment = null, bool isDraft = false)
        {
            DesignTimeMetadata metadata = Owner.GetDesignTimeMetadata(isDraft);
            return metadata.ToXML();
        }
    }

    public class XamlProviderFactory<TService> : ICodeGenProviderFactory<TService>
         where TService : BaseDomainService
    {
        public ICodeGenProvider Create(BaseDomainService owner)
        {
            return Create((TService)owner);
        }

        public ICodeGenProvider<TService> Create(TService owner)
        {
            return new XamlProvider<TService>(owner, Lang);
        }

        public string Lang => "xaml";
    }
}
