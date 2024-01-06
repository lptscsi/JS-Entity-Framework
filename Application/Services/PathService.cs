using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RIAppDemo.BLL.Utils;

namespace RIAppDemo.Services
{
    public class PathService : IPathService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public PathService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public string AppRoot
        {
            get
            {
                string appRoot = _env.ContentRootPath;
                return appRoot;
            }
        }

        public string DataDirectory => System.IO.Path.Combine(AppRoot, "App_Data");

        public string ConfigFolder => _configuration[$"AppSettings:FOLDER_BROWSER_PATH"];
    }
}
