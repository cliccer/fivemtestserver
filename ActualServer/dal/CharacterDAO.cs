using ActualServer.dal.dbobjects;
using ActualServer.dao;
using CitizenFX.Core;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActualServer.dal
{
    class CharacterDAO
    {
        private MySqlConnection conn;
        public CharacterDAO()
        {
            conn = DBConnection.GetConnection();
        }
        

        public Character GetCharacterById(int id)
        {
            MySqlCommand cmd = new MySqlCommand("select * from characters where id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            return GetCharacterFromReader(reader);
        }

        public List<Character> GetCharactersForAccount(int accountId)
        {
            MySqlCommand cmd = new MySqlCommand("select * from characters where account_id = @accountId", conn);
            cmd.Parameters.AddWithValue("@accountId", accountId);

            MySqlDataReader reader = cmd.ExecuteReader();
            List<Character> characters = new List<Character>();
            Debug.WriteLine("Reader: " + reader.RecordsAffected.ToString() + " " + reader.HasRows.ToString());
            while (reader.Read())
            {
                Debug.WriteLine("Reading");
                characters.Add(GetCharacterFromReader(reader));
            }
            reader.Close();
            return characters;
        }

        public Character GetCharacterByIdAndAccountId(int id, int accountId)
        {
            MySqlCommand cmd = new MySqlCommand("select * from `characters` where `id` = @id and `account_id` = @accountId", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@accountId", accountId);

            MySqlDataReader reader = cmd.ExecuteReader();
            Character character;
            if (reader.Read())
            {
                character = GetCharacterFromReader(reader);
                Debug.WriteLine(character.ToString());
            } else
            {
                Debug.WriteLine("character is null");
                character = null;
            }
            reader.Close();
            return character;
        }

        public List<Character> GetCharactersByNames(string firstName, string lastName)
        {
            if(firstName == null && lastName == null)
            {
                return new List<Character>();
            }
            bool firstNameGiven = firstName != null;
            bool lastNameGiven = lastName != null;
            string sql = "select * from characters where";
            if(firstNameGiven)
            {
                sql += " first_name like @firstName";

                if(lastNameGiven)
                {
                    sql += " and lastName like @lastName";
                }
            } else if(lastNameGiven)
            {
                sql += " last_name like @lastName";
            }
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            if(firstNameGiven)
            {
                cmd.Parameters.AddWithValue("@firstName", firstName);
            }
            if(lastNameGiven)
            {
                cmd.Parameters.AddWithValue("@lastName", lastName);
            }

            MySqlDataReader reader = cmd.ExecuteReader();
            List<Character> characters = new List<Character>();
            while (reader.Read())
            {
                characters.Add(GetCharacterFromReader(reader));
            }
            return characters;

        }

        public Character SaveOrUpdate(Character character)
        {
            if(character.Id == null)
            {
                return SaveCharacter(character);
            } else
            {
                return UpdateCharacter(character);
            }
            
        }

        private Character SaveCharacter(Character character)
        {
            MySqlCommand cmd = new MySqlCommand("insert into characters (account_id, first_name, last_name) values(@accountId, @firstName, @lastName)", conn);

            cmd.Parameters.AddWithValue("@accountID", character.AccountId);
            Debug.WriteLine("Added " + character.AccountId.ToString());
            cmd.Parameters.AddWithValue("@firstName", character.FirstName);
            Debug.WriteLine("Added " + character.FirstName);
            cmd.Parameters.AddWithValue("@lastName", character.LastName);
            Debug.WriteLine("Added " + character.LastName);

            cmd.ExecuteNonQuery();
            Debug.WriteLine("character.accountid " + character.AccountId);
            Character latestCharacter = GetCharactersForAccount(character.AccountId).Last();

            return latestCharacter;

        }

        private Character UpdateCharacter(Character character)
        {
            MySqlCommand cmd = new MySqlCommand("update characters set account_id = @accountID and first_name = @firstName and last_name = @lastName where id = @id", conn);
            cmd.Parameters.AddWithValue("@accountId", character.AccountId);
            cmd.Parameters.AddWithValue("@firstName", character.FirstName);
            cmd.Parameters.AddWithValue("@lastName", character.LastName);
            cmd.Parameters.AddWithValue("@id", character.Id);

            if(cmd.ExecuteNonQuery() == 1)
            {
                return character;
            } else
            {
                return null;
            }
        }

        public bool DeleteCharacter(Character character)
        {
            if(character.Id != null)
            {
                MySqlCommand cmd = new MySqlCommand("delete from characters where id = @id", conn);
                cmd.Parameters.AddWithValue("@id", character.Id);
                return cmd.ExecuteNonQuery() == 1;
            } else
            {
                Debug.WriteLine("Attempted to delete character without set id accountId=" + character.AccountId + 
                    " firstName=" + character.FirstName + " lastName=" + character.LastName);
                return false;
            }
        }

        private Character GetCharacterFromReader(MySqlDataReader reader)
        {
            Character character = new Character();
            character.Id = reader.GetInt32("id");
            character.AccountId = reader.GetInt32("account_id");
            character.FirstName = reader.GetString("first_name");
            character.LastName = reader.GetString("last_name");
            Debug.WriteLine("Added character with id " + character.Id.ToString() + " for account " + character.AccountId);
            return character;
        }
    }
}
