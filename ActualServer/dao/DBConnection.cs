using CitizenFX.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ActualServer.dao
{
    class DBConnection : BaseScript
    {

        private string databaseName = "fivemtest";
        private string username = "dbuser";
        private string password = "password";
        private string server = "localhost";

        public DBConnection()
        {
            Connection = CreateConnection();
        }

        public MySqlConnection Connection { get; private set; } = null;

        private MySqlConnection CreateConnection()
        {
            Debug.WriteLine("11");

            string connectionString = string.Format("Server={0}; database={1}; UID={2}; password={3}", server, databaseName, username, password);
            Debug.WriteLine("12");
            Connection = new MySqlConnection(connectionString);
            Debug.WriteLine("13");
            Connection.Open();
            Debug.WriteLine("14");
            return Connection;
        }
    }
}
