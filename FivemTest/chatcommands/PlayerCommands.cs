using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FivemTest.chatcommands
{
    public class PlayerCommands : BaseScript
    {
        public static void InitPlayerPedCommands()
        {
            API.RegisterCommand("gw", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && Enum.TryParse(argList[0], true, out WeaponHash weapon))
                {
                    Game.PlayerPed.Weapons.Give(weapon, 250, true, false);
                }
                else
                {
                    ChatUtil.SendMessageToClient("Error", "Usage : /gw [WeaponName]", 255, 0, 0);
                }
            }


            ), false);


            API.RegisterCommand("heal", new Action<int, List<object>, string>((src, args, raw) =>
            {
                Game.PlayerPed.Armor = Game.Player.MaxArmor;
                Game.PlayerPed.Health = Game.PlayerPed.MaxHealth;
                Game.PlayerPed.ResetVisibleDamage();
            }
            ), false);

            API.RegisterCommand("killme", new Action<int, List<object>, string>((src, args, raw) =>
            {
                Game.PlayerPed.Kill();
            }
            ), false);


            API.RegisterCommand("tp", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var waypoint = API.GetFirstBlipInfoId(8);

                var waypointCoords = API.GetBlipCoords(waypoint);

                float height = 0f;
                int attempts = 0;
                for (int i = 0; i < 100; i++)
                {
                    attempts++;
                    height = 1000f;

                    API.GetGroundZFor_3dCoord(waypointCoords[0] + i, waypointCoords[1] + i, height, ref height, false);

                    if (height != 0f && height != 1000f)
                    {
                        ChatUtil.SendMessageToClient("[TP]", "Attempts : " + attempts + " Height is " + height, 255, 255, 255);
                        i = 1000;
                    }
                }

                Game.PlayerPed.Position = new Vector3(waypointCoords[0], waypointCoords[1], height + 2f);

                ChatUtil.SendMessageToClient("[TP]", "Processing is done, attemps: " + attempts, 255, 255, 255);
            }
            ), false);


            API.RegisterCommand("coords", new Action<int>(src =>
            {
                var playerLocation = Game.PlayerPed.Position;
                ChatUtil.SendMessageToClient("[Coords", "Your coordinates are x " + playerLocation.X + " y " + playerLocation.Y, 255, 255, 255);
            }), false);

            API.RegisterCommand("revive", new Action<int>(src =>
            {
                if (Game.PlayerPed.IsDead)
                {
                    Game.PlayerPed.Resurrect();
                }
            }), false);
        }

    }

}
