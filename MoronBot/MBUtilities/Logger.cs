using System.IO;

namespace MBUtilities
{
    public static class Logger
    {
        private static readonly object logSync = new object();

        public static void Write(string data, string fileName)
        {
            lock (logSync)
            {
                FileUtils.CreateDirIfNotExists(fileName);

                while (FileUtils.FileUsedByAnotherProcess(fileName))
                    System.Threading.Thread.Sleep(50);

                using (StreamWriter log = new StreamWriter(fileName, true))
                {
                    log.WriteLine(data);
                }
            }
        }
    }
}
