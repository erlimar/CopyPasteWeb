using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace CopyPasteData
{
    public class RepositoryBase<T> : IDisposable
    {
        private readonly SQLiteConnection _conn;
        private readonly string _directoryBasePath;

        public RepositoryBase()
        {
            _directoryBasePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();

            _conn = DbConnection();

            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            if (_conn.State != System.Data.ConnectionState.Open)
            {
                _conn.Open();
            }
        }

        public void Dispose()
        {
            if (_conn != null)
            {
                if (_conn.State != System.Data.ConnectionState.Closed)
                {
                    _conn.Close();
                }

                _conn.Dispose();
            }
        }

        private string DbFile
        {
            get { return Path.Combine(_directoryBasePath, "Dataset.sqlite"); }
        }

        private SQLiteConnection DbConnection()
        {
            return new SQLiteConnection(string.Format("Data Source={0}", DbFile));
        }

        private void CreateDatabase()
        {
            string dirPath = Path.GetDirectoryName(DbFile);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            _conn.Open();

            using(var t = Transaction)
            {
                _conn.Execute(@"
                create table Dataset
                (
                    ID              integer primary key AUTOINCREMENT,
                    SessionID       character(8) unique,
                    ActivityDate    datetime not null,
                    Value           text null
                )");

                t.Commit();
            }
        }

        protected SQLiteConnection Conn { get { return _conn; } }

        protected SQLiteTransaction Transaction { get { return _conn.BeginTransaction(); } }

        protected int Execute(string sql, object param = null)
        {
            return _conn.Execute(sql, param);
        }

        protected int ExecuteWithLastId(string sql, object param = null)
        {
            return _conn.QuerySingle<int>(sql + "; select last_insert_rowid()", param);
        }

        protected IEnumerable<T> Query(string sql, object param = null)
        {
            return _conn.Query<T>(sql, param);
        }

        protected T QueryFirst(string sql, object param = null)
        {
            return _conn.QueryFirst<T>(sql, param);
        }

        protected T QueryFirstOrDefault(string sql, object param = null)
        {
            return _conn.QueryFirstOrDefault<T>(sql, param);
        }

        protected T QuerySingle(string sql, object param = null)
        {
            return _conn.QuerySingle<T>(sql, param);
        }

        protected T QuerySingleOrDefault(string sql, object param = null)
        {
            return _conn.QuerySingleOrDefault<T>(sql, param);
        }
    }
}
