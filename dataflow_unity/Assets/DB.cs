using System.Collections.Generic;
using System.Linq;
using SqlCipher4Unity3D;

namespace Hello
{
    public class DataService
    {
        private SQLiteConnection _connection;

        public DataService(string dbFpath, string password = "")
        {
            //const string DateTimeSqliteDefaultFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff";
            //SQLiteConnectionString conStr = new SQLiteConnectionString(dbFpath, storeDateTimeAsTicks: false, key: password, openFlags: SQLiteOpenFlags.ReadOnly, dateTimeStringFormat: DateTimeSqliteDefaultFormat);
            //this._connection = new SQLiteConnection(conStr);
            this._connection = new SQLiteConnection(dbFpath, password, storeDateTimeAsTicks: true);
        }

        public List<T> Gets<T>() where T : new()
        {
            return this._connection.Table<T>().ToList();
        }
    }
}