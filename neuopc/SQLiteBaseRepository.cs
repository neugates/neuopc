using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuopc
{
    public class SQLiteBaseRepository
    {
        public static string DbFile
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "neuopc.db"); }
        }

        public static SQLiteConnection SimpleDbConnection()
        {
            string connString = string.Format("Data Source={0};", DbFile);
            return new SQLiteConnection(connString);
        }
    }
}
