using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Threading;

namespace ServerTCPLibrary
{
    public class DatabaseAccess
    {
        private static ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

        public static ReaderWriterLockSlim ReaderWriterLock { get => _readerWriterLock; set => _readerWriterLock = value; }

        /// <summary>
        /// Metoda sprawdzająca czy dane znajdują się w bazie danych
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Account checkDB(string login, string password)
        {
            string select = $"SELECT * FROM Account WHERE Login='{login}' AND Password='{password}'";
            using (IDbConnection cnn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString))
            {
                Account acc;
                try
                {
                    ReaderWriterLock.EnterReadLock();
                    acc = cnn.QueryFirst<Account>(select, new DynamicParameters());
                }
                finally { ReaderWriterLock.ExitReadLock(); }

                return acc;
            }
        }
        
        /// <summary>
        /// Metoda dodająca dane do bazy danych
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public static void addUser(string login, string password)
        {
            //string select = $"SELECT * FROM Account WHERE Login='{login}' AND Password='{password}'";
            string insert = $"INSERT INTO Account (Login, Password) VALUES ('{login}', '{password}')";
            using (IDbConnection cnn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString))
            {
                try
                {
                    ReaderWriterLock.EnterWriteLock();
                    cnn.Execute(insert);
                    
                }
                finally { ReaderWriterLock.ExitWriteLock(); }
            }
        }
    }
}
