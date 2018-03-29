using System.Collections.Generic;
using System.Linq;
using SqlCipher4Unity3D;

namespace Hello
{
    public class DataService
    {
        private SQLiteConnection _connection;

        public DataService(string dbFpath, string password)
        {
            this._connection = new SQLiteConnection(dbFpath, password);
        }

        public List<T> Gets<T>() where T : new()
        {
            return this._connection.Table<T>().ToList();
        }
    }
}