using System.Data.Common;
using System.IO;

using Mono.Data.Sqlite;

namespace MBUtilities
{
    public class SQLiteInterface
    {
        SqliteConnection conn = null;
        public SqliteConnection Connection
        {
            get { return conn; }
        }
        public DbCommand Command
        {
            get { return conn.CreateCommand(); }
        }
        public DbParameter Parameter
        {
            get { return new SqliteParameter(); }
        }

        #region Singleton rubbish
        static SQLiteInterface instance = null;
        static readonly object padlock = new object();

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

        SQLiteInterface()
        {
            string path = Path.Combine(Settings.Instance.DataPath, "mbdata.sqlite");
            try
            {
                conn = new SqliteConnection();
                conn.ConnectionString = "Data Source=" + path;
                conn.Open();
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                Logger.Write(ex.StackTrace, Settings.Instance.ErrorFile);
                Logger.Write(conn.ConnectionString, Settings.Instance.ErrorFile);
            }
        }

        ~SQLiteInterface()
        {
            conn.Close();
        }
    }
}
