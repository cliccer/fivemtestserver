using ActualServer.dao;
using ActualServer.dbobjects;
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
            Debug.WriteLine("Serverevents initialized");

        }

        private void ServerAnnouncement([FromSource] Player player, string message)
        {
            
            Debug.WriteLine("serverAnnouncement2");
            TriggerClientEvent("chat:addMessage", new Dictionary<string, object>
            {
                ["color"] = new[] { 255, 255, 255 },
                ["args"] = new[] { message }
            });

            Debug.WriteLine("serverAnnouncement done2");
        }
    }
}
