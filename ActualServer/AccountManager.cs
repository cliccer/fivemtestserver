using ActualServer.dao;
using ActualServer.dbobjects;
using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace ActualServer
{
    class AccountManager : BaseScript
    {

        private Dictionary<string, Account> playerHandleAccountMap;

        public AccountManager()
        {
            playerHandleAccountMap = new Dictionary<string, Account>();
            InitEventHandlers();
        }

        private void InitEventHandlers()
        {
            EventHandlers["playerConnecting"] += new Action<Player, string>(OnPlayerConnecting);

            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
        }

        private void OnPlayerConnecting([FromSource] Player player, string playerName)
        {
            AccountDAO accountDAO = new AccountDAO(DBConnection.Connection);
            Account account = accountDAO.GetAccount(player);
            playerHandleAccountMap.Add((int.Parse(player.Handle) - 65535).ToString(), account);
            Debug.WriteLine("Account " + account.Id.ToString() + " " + account.Name + " connected, player " + player.Handle);
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            Debug.WriteLine("OnPlayerDropped" + player.Handle);
            if(playerHandleAccountMap.TryGetValue(player.Handle, out Account account))
            {
                Debug.WriteLine("Account " + account.Id.ToString() + " " + account.Name + "dropped, player " + player.Handle);
                playerHandleAccountMap.Remove(player.Handle);
            } else
            {
                Debug.WriteLine("Player " + player.Handle + " not found in map, pls fix");
            }
        }

        public Account GetAccount(Player player)
        {
            playerHandleAccountMap.TryGetValue(player.Handle, out Account account);
            return account;
        }
    }
}
