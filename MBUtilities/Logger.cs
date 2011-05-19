using System.IO;

namespace MBUtilities
{
    public static class Logger
    {
        public static void Write(string data, string fileName)
        {
            FileUtils.CreateDirIfNotExists(fileName);

            using (StreamWriter log = new StreamWriter(fileName, true))
            {
                log.WriteLine(data);
            }
        }
    }
}
