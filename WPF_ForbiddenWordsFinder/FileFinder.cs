using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ForbiddenWordsFinder
{
    class FileFinder
    {
        private string pathh;

        public string Pathh
        {
            get { return pathh; }
            set { pathh = value; }
        }
        public FileFinder() {}
        public FileFinder(string pathh)
        {
            this.pathh = pathh;
        }

        public static void GetAllFiles(string rootDirectory, string fileExtension, List<string> files)
        {
            try
            {
                string[] directories = Directory.GetDirectories(rootDirectory);
                files.AddRange(Directory.GetFiles(rootDirectory, fileExtension));

                foreach (string path in directories)
                    GetAllFiles(path, fileExtension, files);
            }
            catch { }
        }  
        
        //public override string ToString()
        //{
        //    return path;
        //}
    }
}
