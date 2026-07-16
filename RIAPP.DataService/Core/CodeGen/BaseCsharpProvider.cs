using RIAPP.DataService.Core.Metadata;

namespace RIAPP.DataService.Core.CodeGen
{
    public abstract class BaseCsharpProvider<TService>(IMetaDataProvider owner, string lang) : ICodeGenProvider<TService>
         where TService : BaseDomainService
    {
        public string Lang
        {
            get;
        } = lang;

        public IMetaDataProvider Owner { get; } = owner;

        public abstract string GenerateScript(string comment = null, bool isDraft = false);
    }
}
