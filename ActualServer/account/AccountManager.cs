using ActualServer.dao;
using ActualServer.dbobjects;
using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace ActualServer
{
    class AccountManager : BaseScript
    {

        private static Dictionary<string, Account> playerHandleAccountMap;

        public AccountManager()
        {
            playerHandleAccountMap = new Dictionary<string, Account>();
            InitEventHandlers();
            AddCurrentPlayers();
        }

        private void InitEventHandlers()
        {
            EventHandlers["playerConnecting"] += new Action<Player, string>(OnPlayerConnecting);

            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
        }

        private void OnPlayerConnecting([FromSource] Player player, string playerName)
        {
            AccountDAO accountDAO = new AccountDAO();
            Account account = accountDAO.GetAccount(player);
            playerHandleAccountMap.Add((int.Parse(player.Handle) - 65535).ToString(), account);
            Debug.WriteLine("Account " + account.Id.ToString() + " " + account.Name + " connected, player " + player.Handle);
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            if(playerHandleAccountMap.TryGetValue(player.Handle, out Account account))
            {
                Debug.WriteLine("Account " + account.Id.ToString() + " " + account.Name + "dropped, player " + player.Handle);
                playerHandleAccountMap.Remove(player.Handle);
            } else
            {
                Debug.WriteLine("Player " + player.Handle + " not found in map, pls fix");
            }
        }

        public static Account GetAccount(Player player)
        {
            Debug.WriteLine("Getting account for player " + player.Handle);
            playerHandleAccountMap.TryGetValue(player.Handle, out Account account);
            if(account == null)
            {
                Debug.WriteLine("GetAccount() Account is null");
            }
            Debug.WriteLine("Accountid " + account.Id);
            return account;
        }

        private async void AddCurrentPlayers()
        {
            await Delay(2000);
            AccountDAO accountDAO = new AccountDAO();
            PlayerList pl = new PlayerList();
            foreach(Player player in pl)
            {
                Debug.WriteLine("Adding player " + player.Name + " to list of accounts");
                playerHandleAccountMap.Add(player.Handle , accountDAO.GetAccount(player));
            }
        }
    }
}
