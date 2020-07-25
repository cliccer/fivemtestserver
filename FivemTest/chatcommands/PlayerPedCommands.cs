using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.chatcommands
{
    public class PlayerPedCommands : BaseScript
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
                for (int i = 0; i < 100; i++)
                {
                    int attempts = 0;
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

                ChatUtil.SendMessageToClient("[TP]", "Processing is done", 255, 255, 255);
            }
            ), false);
        }

    }

}
