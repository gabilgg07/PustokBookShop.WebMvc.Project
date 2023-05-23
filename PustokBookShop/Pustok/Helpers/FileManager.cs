using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Helpers
{
    public static class FileManager
    {
        public static string Save(string rootPath, string folder, IFormFile file)
        {
            string newFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string path = Path.Combine(rootPath, folder, newFileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return  newFileName;
        }

        public static bool Delete(string rootPath, string folder, string filename)
        {
            string path = Path.Combine(rootPath, folder, filename);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
