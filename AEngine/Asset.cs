using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEngine
{
    public class Asset
    {
        public static string BasePath = "";

        public Asset(string fileName)
        {
            BaseFileName = fileName;
            FileName = Path.Combine(BasePath, fileName);
        }

        public string Name { get; set; }

        public string FileName { get; private set; }
        public string BaseFileName { get; private set; }
        public Engine Engine { get; internal set; }
    }
}
