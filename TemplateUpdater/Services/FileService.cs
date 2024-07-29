using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateUpdater.Services
{
    public static class FileService
    {
        public static void EnsureUploadsFolderExists()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "LocalTemplates");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
