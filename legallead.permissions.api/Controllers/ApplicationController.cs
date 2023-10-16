using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private const string defaultReadme = "No ReadMe information is available.";
        private static bool isReadMeBuilt = false;

        private static readonly object _instance = new object();
        private string? _readme;
        public ApplicationController()
        {
        }

        [HttpGet(Name = "ReadMe")]
        [Route("read-me")]
        public string ReadMe()
        {
            _readme ??= defaultReadme;
            GenerateReadMe(ref _readme);
            return _readme;
        }


        private static void GenerateReadMe(ref string readme)
        {
            if (isReadMeBuilt) return;
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null || assembly.Location == null) return;
            var execName = new Uri(assembly.Location).AbsolutePath;
            if (execName != null && System.IO.File.Exists(execName))
            {
                var contentRoot = Path.GetDirectoryName(execName) ?? "";
                var dataRoot = Path.Combine(contentRoot, "_db");
                var dataFile = Path.Combine(dataRoot, "readme.txt");
                if (System.IO.File.Exists(dataFile))
                {
                    lock (_instance)
                    {
                        readme = System.IO.File.ReadAllText(dataFile);
                        isReadMeBuilt = true;
                    }
                }
                else
                {
                    readme = defaultReadme;
                }

            }
        }
    }
}
