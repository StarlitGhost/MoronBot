using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Community.CsharpSqlite;
using Community.CsharpSqlite.SQLiteClient;
using System.Data.Common;

namespace MBUtilities
{
    public class SQLiteInterface
    {
        SqliteConnection conn = null;
        public SqliteConnection Connection
        {
            get { return conn; }
        }

        #region Singleton rubbish
        static SQLiteInterface instance = null;
        static readonly object padlock = new object();

        SQLiteInterface()
        {
            string path = Path.Combine(Settings.Instance.DataPath, "mbdata.sqlite");
            try
            {
                conn = new SqliteConnection();
                conn.ConnectionString = "Data Source=file:" + path;
                conn.Open();
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
            }
        }

        public static SQLiteInterface Instance
        {
            get 
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SQLiteInterface();
                    }
                    return instance;
                }
            }
        }

        public static void Assign(SQLiteInterface sqliteInterface)
        {
            lock (padlock)
            {
                instance = sqliteInterface;
            }
        }
        #endregion Singleton rubbish

        ~SQLiteInterface()
        {
            conn.Close();
        }
    }
}
