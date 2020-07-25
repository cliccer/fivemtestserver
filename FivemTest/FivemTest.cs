using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FivemTest
{
    public class FivemTest : BaseScript
    {

        public FivemTest()
        {
            InitStartUpSettings();

            API.RegisterCommand("gw", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && Enum.TryParse(argList[0], true, out WeaponHash weapon))
                {
                    Game.PlayerPed.Weapons.Give(weapon, 250, true, false);
                }
                else
                {
                    SendChatMessage("Error", "Usage : /gw [WeaponName]", 255, 0, 0);
                }

            }


            ), false);

            API.RegisterCommand("sv", new Action<int, List<object>, string>(async (src, args, raw) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && Enum.TryParse(argList[0], true, out VehicleHash vehicle))
                {
                    var model = new Model(vehicle);
                    var veh = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0f, 5f, 2f)), Game.PlayerPed.Heading);
                    veh.NeedsToBeHotwired = false;
                    veh.Mods.InstallModKit();
                    veh.Mods[VehicleModType.Engine].Index = veh.Mods[VehicleModType.Engine].ModCount - 1;
                    veh.Mods[VehicleModType.Brakes].Index = veh.Mods[VehicleModType.Brakes].ModCount - 1;
                    veh.Mods[VehicleModType.Transmission].Index = veh.Mods[VehicleModType.Transmission].ModCount - 1;

                    veh.Mods[VehicleToggleModType.Turbo].IsInstalled = true;
                    veh.Mods[VehicleToggleModType.XenonHeadlights].IsInstalled = true;


                    veh.Rotation = Game.PlayerPed.Rotation + new Vector3(0f, 0f, 90f);
                    SendChatMessage("[VehicleSpawner]", "You spawned a " + vehicle.ToString(), 255, 255, 255);
                }
            }
            ), false);

            API.RegisterCommand("fix", new Action<int, List<object>, string>((src, args, raw) =>
            {
                if (Game.PlayerPed.IsInVehicle())
                    Game.Player.LastVehicle.Repair();
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
                        SendChatMessage("[TP]", "Attempts : " + attempts + " Height is " + height, 255, 255, 255);
                        i = 1000;
                    }
                }

                Game.PlayerPed.Position = new Vector3(waypointCoords[0], waypointCoords[1], height + 2f);

                SendChatMessage("[TP]", "Processing is done", 255, 255, 255);
            }
            ), false);

            API.RegisterCommand("wanted", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && int.TryParse(argList[0], out int level))
                {
                    API.SetMaxWantedLevel(level);
                    SendChatMessage("[WantedLevel]", "Max wanted level set to " + level, 255, 255, 255);
                }
            }
            ), false);

            API.RegisterCommand("wanted2", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && int.TryParse(argList[0], out int level))
                {
                    Game.WantedMultiplier = 0;
                    SendChatMessage("[WantedLevel]", "Wanted multiplier set to " + level, 255, 255, 255);
                }
            }
            ), false);

            API.RegisterCommand("coords", new Action<int>(src =>
            {
                var playerLocation = Game.PlayerPed.Position;
                SendChatMessage("[Coords", "Your coordinates are x " + playerLocation.X + " y " + playerLocation.Y, 255, 255, 255);
            }), false);

            API.RegisterCommand("revive", new Action<int>(src =>
            {
                if (Game.PlayerPed.IsDead)
                {
                    Game.PlayerPed.Resurrect();
                }
            }), false);

        }

        public static void SendChatMessage(string title, string message, int r, int g, int b)
        {
            var msg = new Dictionary<string, object>
            {
                ["color"] = new[] { r, g, b },
                ["args"] = new[] { title, message }
            };
            TriggerEvent("chat:addMessage", msg);
        }

        private void InitStartUpSettings()
        {
            API.DisableAutomaticRespawn(false);

            //API.
            //API.SetDispatchSpawnLocation;
        }
    }


}
