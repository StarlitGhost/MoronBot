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
            List<string> files = new List<string>(Directory.GetFiles(folder, "*.dll"));
            files.Sort();

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
                    Logger.Write(ex.ToString(), Settings.Instance.ErrorFile);
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
