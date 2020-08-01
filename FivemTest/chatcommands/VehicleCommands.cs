using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FivemTest.chatcommands
{
    static class VehicleCommands
    {
        public static void InitVehicleCommands()
        {
            API.RegisterCommand("sv", new Action<int, List<object>>((src, args) =>
            {
                var argList = args.Select(o => o.ToString()).ToList();
                if (argList.Any() && Enum.TryParse(argList[0], true, out VehicleHash vehicle))
                {
                    VehicleUtil.SpawnVehicle(vehicle);
                }
            }
            ), false);

            API.RegisterCommand("fix", new Action<int>((src) =>
            {
                if (Game.PlayerPed.IsInVehicle())
                    Game.Player.LastVehicle.Repair();
            }
            ), false);

            API.RegisterCommand("impound", new Action<int>((src) =>
            {
                Ped playerPed = Game.PlayerPed;
                Vector3 playerPos = playerPed.Position;
                Vector3 playerOffset = API.GetOffsetFromEntityGivenWorldCoords(playerPed.Handle, 0, 1f, 0);
                int rayHandle = API.StartShapeTestCapsule(playerPos.X, playerPos.Y, playerPos.Z, playerOffset.X, playerOffset.Y, playerOffset.Z, 1, 10, playerPed.Handle, 7);
                bool hit = false;
                Vector3 endPoint = playerPos;
                Vector3 surfaceNormal = playerPos;
                int veh = 0;
                API.GetShapeTestResult(rayHandle, ref hit, ref endPoint, ref surfaceNormal, ref veh);

                if(veh != 0)
                {
                    Vehicle vehicle = new Vehicle(veh);
                    //TODO should probably check if vehicel has passengers
                    vehicle.Delete();
                }
            }
            ), false);

            API.RegisterCommand("engine", new Action<int>(src =>
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                if(veh != null)
                {
                    if (veh.IsEngineRunning)
                    {
                        API.SetVehicleEngineOn(veh.Handle, false, false, true);
                    } else
                    {
                        API.SetVehicleEngineOn(veh.Handle, true, false, true);
                    }
                }
            }), false);
        }
    }
}
