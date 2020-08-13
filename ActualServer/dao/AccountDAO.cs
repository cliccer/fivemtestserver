using ActualServer.dbobjects;
using CitizenFX.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActualServer.dao
{
    class AccountDAO
    {
        private MySqlConnection conn;
        public AccountDAO(MySqlConnection conn)
        {
            this.conn = conn;
        }


        public Account GetAccount(Player player)
        {
            Account account = FindAccountFromFivemId(player.Identifiers["fivem"]);

            if(account != null)
            {
                return account;
            } else
            {
                return SaveNewAccount(player);
            }


            //MySqlConnection connection = dbConn.Connection;
            //Debug.WriteLine("2");
            //MySqlCommand cmd = new MySqlCommand("select version()", connection);
        }

        private Account FindAccountFromFivemId(string fivemId)
        {
            Account account = null;
            MySqlCommand cmd = new MySqlCommand("select * from account where fivem_id = @fivemId", conn);
            cmd.Parameters.AddWithValue("@fivemId", fivemId);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string fivem_id = reader.GetString("fivem_id");
                    string name = reader.GetString("name");
                    account = new Account(id, fivem_id, name);
                }
            }
            reader.Close();
            
            return account;
            
            
        }

        private Account SaveNewAccount(Player player)
        {
            Debug.WriteLine("Account not found in database saving new account for fivem id " + player.Identifiers["fivem"]);
            MySqlCommand cmd = new MySqlCommand("insert into account (fivem_id, name) values(@fivemId, @name)", conn);
            cmd.Parameters.AddWithValue("@fivemID", player.Identifiers["fivem"]);
            cmd.Parameters.AddWithValue("@name", player.Name);
            cmd.ExecuteNonQuery();

            return FindAccountFromFivemId(player.Identifiers["fivem"]);
        }
    }
}
