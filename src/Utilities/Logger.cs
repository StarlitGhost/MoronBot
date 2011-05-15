using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MoronBot.Utilities
{
    static class Logger
    {
        public static void Write(string data, string fileName)
        {
            string path = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (StreamWriter log = new StreamWriter(fileName, true))
            {
                log.WriteLine(data);
            }
        }
    }
}
