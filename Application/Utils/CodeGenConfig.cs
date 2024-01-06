using Microsoft.AspNetCore.Hosting;
using RIAPP.DataService.Core.CodeGen;

namespace RIAppDemo.Utils
{
    public class CodeGenConfig : ICodeGenConfig
    {
        private readonly IWebHostEnvironment _env;

        public CodeGenConfig(IWebHostEnvironment env)
        {
            _env = env;
        }

        bool ICodeGenConfig.IsCodeGenEnabled => _env.EnvironmentName == "Development";
    }
}
