using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using System.Windows.Forms;

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

        public string Server = /*"irc.editingarchive.com";*//*"irc.freeside.co.uk";*//*"irc.slashnet.org"*/"c.ustream.tv";
        public int Port = 6667;
        public string Channel = "#tyranic-moron";
        public string Nick = "MoronBot";
        public string CurrentNick = "MoronBot";
        public string LeaveMessage = "Leaving";
        public string QuitMessage = "Quitting";
        public List<string> IgnoreList = new List<string>();

        [XmlIgnore]
        public static string AppPath = Path.GetDirectoryName(Application.ExecutablePath);
        [XmlIgnore]
        public string FunctionPath = Path.Combine(AppPath, "Functions");
        [XmlIgnore]
        public string DataPath = Path.Combine(AppPath, "Data");

        //public Dictionary<string, Dictionary<string, List<string>>> FunctionSettings = new Dictionary<string, Dictionary<string, List<string>>>();
    }
}
