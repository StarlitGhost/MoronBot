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
    }
}
