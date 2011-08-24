using System.IO;

namespace MBUtilities
{
    public static class FileUtils
    {
        public static void CreateDirIfNotExists(string filePath)
        {
            string path = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static bool FileUsedByAnotherProcess(string fileName)
        {
            try
            {
                File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (System.IO.IOException)
            {
                return true;
            }
            return false;
        }
    }
}
