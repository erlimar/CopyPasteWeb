using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CopyPastWeb
{
    class Dataset
    {
        public DateTime ActivityDate { get; set; }
        public string Value { get; set; }
    }

    public class Database
    {
        private const short MAX_HOURS_AGE = 24;
        private static Database _instance;
        private readonly IDictionary<string, Dataset> _database;
        private readonly Random _random;

        private Database()
        {
            _database = new Dictionary<string, Dataset>();
            _random = new Random(new Random().Next());
        }

        public static Database Instance
        {
            get
            {
                if (_instance == null)
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

            Dataset ds = _database[sessionID];
            TimeSpan diff = DateTime.Now - ds.ActivityDate;

            if (diff.TotalHours > MAX_HOURS_AGE)
            {
                return string.Empty;
            }

            // Update activity
            ds.ActivityDate = DateTime.Now;

            return ds.Value;
        }

        public string SetValue(string sessionId, string newValue)
        {
            ThrowIfNotExistsOrExpired(sessionId);

            _database[sessionId].Value = newValue;

            return GetValue(sessionId);
        }

        public string NewID()
        {
            string id = GenerateID();

            _database.Add(id, new Dataset { ActivityDate = DateTime.Now });

            return id;
        }

        private string GenerateID(int attempts = -100)
        {
            var bytes = new byte[] { 0, 0, 0, 0 };
            var id = string.Empty;

            _random.NextBytes(bytes);
            Array.ForEach(bytes, (b) => id += b.ToString("X2"));

            // if hex already exists, 
            if (IDAlreadyExists(id))
            {
                if (attempts >= 0)
                {
                    throw new Exception("ID already exists!");
                }

                id = GenerateID(++attempts);
            }

            return id;
        }

        private bool IDAlreadyExists(string id)
        {
            return _database.ContainsKey(id);
        }

        private void ThrowIfNotExistsOrExpired(string sessionID)
        {
            if (!_database.ContainsKey(sessionID))
            {
                throw new KeyNotFoundException(string.Format("SessionID {0} not exists!", sessionID));
            }

            Dataset ds = _database[sessionID];
            TimeSpan diff = DateTime.Now - ds.ActivityDate;

            if (diff.TotalHours > MAX_HOURS_AGE)
            {
                throw new Exception(string.Format("SessionID {0} has expired!", sessionID));
            }
        }
    }
}