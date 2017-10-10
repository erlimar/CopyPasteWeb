using System;
using System.Collections.Generic;

namespace CopyPasteData
{
    public class DatasetRepository : RepositoryBase<Dataset>, IDatasetRepository
    {
        private const short MAX_HOURS_AGE = 24;
        private readonly Random _random;

        public DatasetRepository()
        {
            _random = new Random(new Random().Next());
        }
        public string GetValue(string sessionID)
        {
            string value = null;

            using (var t = Transaction)
            {
                Dataset ds = QuerySingleOrDefault(
                    "select * from Dataset where SessionID = @id",
                    new { id = sessionID });

                if(ds == null)
                {
                    return string.Empty;
                }

                TimeSpan diff = DateTime.Now - ds.ActivityDate;

                if (diff.TotalHours > MAX_HOURS_AGE)
                {
                    return string.Empty;
                }

                ds.ActivityDate = DateTime.Now;

                Execute("update Dataset set ActivityDate = @ActivityDate", ds);

                value = ds.Value;

                t.Commit();
            }

            return value;
        }

        public string SetValue(string sessionId, string newValue)
        {
            using (var t = Transaction)
            {
                Dataset ds = QuerySingleOrDefault("select * from Dataset where SessionID = @id", new { id = sessionId });

                if(ds == null)
                {
                    throw new KeyNotFoundException(string.Format("SessionID {0} not exists!", sessionId));
                }

                TimeSpan diff = DateTime.Now - ds.ActivityDate;

                if (diff.TotalHours > MAX_HOURS_AGE)
                {
                    throw new Exception(string.Format("SessionID {0} has expired!", sessionId));
                }

                ds.ActivityDate = DateTime.Now;
                ds.Value = newValue;

                Execute("update Dataset set ActivityDate = @ActivityDate, Value = @Value", ds);

                t.Commit();
            }

            return newValue;
        }

        public string NewID()
        {
            string newId = null;

            using (var t = Transaction)
            {
                int id = ExecuteWithLastId(@"
                    INSERT INTO Dataset(SessionID, ActivityDate) VALUES(hex(randomblob(4)), datetime('now'));
                    select last_insert_rowid()");

                Dataset ds = QuerySingleOrDefault("select * from Dataset where ID = @id", new { id = id});

                newId = ds.SessionID;

                t.Commit();
            }

            return newId;
        }
    }
}
