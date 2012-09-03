using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MBUtilities
{
    [Serializable()]
    public class Settings
    {
        #region Singleton rubbish
        static Settings instance = null;
        static readonly object padlock = new object();

        Settings()
        {
            FileUtils.CreateDirIfNotExists(DataPath);
        }

        public static Settings Instance
        {
            get 
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Settings();
                    }
                    return instance;
                }
            }
        }

        public static void Assign(Settings settings)
        {
            lock (padlock)
            {
                instance = settings;
            }
        }
        #endregion Singleton rubbish

        public string Server = "irc.desertbus.org"/*"irc.editingarchive.com";*//*"irc.freeside.co.uk";*//*"irc.slashnet.org"*//*"c.ustream.tv"*//*"127.0.0.1"*/;
        public int Port = 6667;
        public string Channel = "#help";
        public string Nick = "MoronBot";
        public string LeaveMessage = "Leaving";
        public string QuitMessage = "Quitting";
        public string Owner = "Tyranic-Moron";
        public List<string> IgnoreList = new List<string>();
        public bool ShowForm = false;

        public List<string> ExcludedFunctions = new List<string>();

        [XmlIgnore]
        public string CurrentNick = "MoronBot";
        [XmlIgnore]
        public static string AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        [XmlIgnore]
        public string FunctionPath = Path.Combine(AppPath, "Functions");
        [XmlIgnore]
        public string DataPath = Path.Combine(AppPath, "Data");
        [XmlIgnore]
        public string LogPath = Path.Combine(AppPath, "logs");
        [XmlIgnore]
        public string ErrorFile = Path.Combine(AppPath, string.Format("logs{0}errors.txt", Path.DirectorySeparatorChar));
    }
}
