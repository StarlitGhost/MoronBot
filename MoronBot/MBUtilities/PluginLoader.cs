using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MBUtilities
{
    public class PluginLoader
    {
        public static List<T> GetPlugins<T>(string folder)
        {
            string[] files = Directory.GetFiles(folder, "*.dll");
            List<T> tList = new List<T>();
            Debug.Assert(typeof(T).IsInterface);

            foreach (string file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsClass || type.IsNotPublic) continue;
                        Type[] interfaces = type.GetInterfaces();
                        if (((IList<Type>)interfaces).Contains(typeof(T)))
                        {
                            object obj = Activator.CreateInstance(type);
                            T t = (T)obj;
                            tList.Add(t);
                        }
                    }
                }
                catch (Exception ex)
                {
					string filePath = string.Format(@".{0}logs{0}errors.txt", Path.DirectorySeparatorChar);
                    Logger.Write(ex.ToString(), filePath);
                }
            }

            return tList;
        }

        public static void WatchDirectory(string path, FileSystemEventHandler handler)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Filter = "*.dll";

            watcher.Created += new FileSystemEventHandler(handler);

            watcher.Path = path;

            watcher.EnableRaisingEvents = true;
        }
    }
}
