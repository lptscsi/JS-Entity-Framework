using Microsoft.EntityFrameworkCore;
using RIAPP.DataService.Core;
using RIAPP.DataService.Core.CodeGen;

namespace RIAPP.DataService.EFCore.Utils
{
    public class CsharpProvider<TService, TDB> : BaseCsharpProvider<TService>
        where TService : EFDomainService<TDB>
        where TDB : DbContext
    {
        private readonly TDB _db;

        public CsharpProvider(TService owner, string lang) :
            base(owner, lang)
        {
            _db = owner.DB;
        }

        public override string GenerateScript(string comment = null, bool isDraft = false)
        {
            Core.Metadata.RunTimeMetadata metadata = Owner.GetMetadata();
            return DataServiceMethodsHelper.CreateMethods(metadata, _db);
        }
    }

    public class CsharpProviderFactory<TService, TDB> : ICodeGenProviderFactory<TService>
         where TService : EFDomainService<TDB>
         where TDB : DbContext
    {
        public ICodeGenProvider Create(BaseDomainService owner)
        {
            return Create((TService)owner);
        }

        public ICodeGenProvider<TService> Create(TService owner)
        {
            return new CsharpProvider<TService, TDB>(owner, Lang);
        }

        public string Lang => "csharp";
    }
}
