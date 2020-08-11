using ActualServer.dao;
using CitizenFX.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ActualServer.serverevents
{
    class ServerEvents : BaseScript
    {

        public ServerEvents()
        {
            EventHandlers["serverAnnouncement"] += new Action<Player, string>(ServerAnnouncement);


        }

        private void ServerAnnouncement([FromSource] Player player, string message)
        {
            Debug.WriteLine("serverAnnouncement2");
            TriggerClientEvent("chat:addMessage", new Dictionary<string, object>
            {
                ["color"] = new[] { 255, 255, 255 },
                ["args"] = new[] { message }
            });
            DBConnection dbConn = new DBConnection();
            Debug.WriteLine("1");
            MySqlConnection connection = dbConn.Connection;
            Debug.WriteLine("2");
            MySqlCommand cmd = new MySqlCommand("select version()", connection);
            Debug.WriteLine("3");

            var reader = cmd.ExecuteReader();
            Debug.WriteLine("4");

            while (reader.Read())
            {
                Debug.WriteLine("MySql version: " + reader.GetString(0));
            }
            Debug.WriteLine("serverAnnouncement done2");
        }
    }
}
