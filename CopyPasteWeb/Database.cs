using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CopyPastWeb
{
    public class Database
    {
        private static Database _instance;
        private readonly IDictionary<string, string> _database;

        private Database()
        {
            _database = new Dictionary<string, string>();
        }

        public static Database Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Database();
                }

                return _instance;
            }
        }

        public string GetValue(string sessionID)
        {
            if (!_database.ContainsKey(sessionID))
            {
                return string.Empty;
            }

            return _database[sessionID];
        }

        public string SetValue(string sessionId, string newValue)
        {
            _database[sessionId] = newValue;

            return GetValue(sessionId);
        }
    }
}