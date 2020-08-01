using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.entities;
using FivemTest.hud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.tickactions
{
    class OnTickEvents
    {

        public static void Execute()
        {
            DisableVehicleAirControl();
            DisableVehicleSeatAutoShuffle();
            EnterVehicleClosestDoor();

            VehicleHud.UpdateVehicleHud();
        }

        private static void DisableVehicleAirControl()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;

                if (vehicle.IsInAir && !VehicleClass.Helicopters.Equals(vehicle.ClassType) 
                    && !VehicleClass.Planes.Equals(vehicle.ClassType) && !VehicleClass.Cycles.Equals(vehicle.ClassType))
                {
                    API.DisableControlAction(0, 59, true);
                    API.DisableControlAction(0, 60, true);
                    API.DisableControlAction(0, 61, true);
                    API.DisableControlAction(0, 62, true);
                    API.DisableControlAction(0, 63, true);
                }
                
            }
        }

        private static void DisableVehicleSeatAutoShuffle()
        {

            Ped ped = Game.PlayerPed;
            if (ped.IsInVehicle() && !PedValues.shuffleSeat)
            {
                Vehicle veh = ped.CurrentVehicle;
                if (API.GetLastPedInVehicleSeat(veh.Handle, 0) == ped.Handle && API.GetIsTaskActive(ped.Handle, 165))
                {
                    API.SetPedIntoVehicle(ped.Handle, veh.Handle, 0);
                } else if (API.GetLastPedInVehicleSeat(veh.Handle, 1) == ped.Handle && API.GetIsTaskActive(ped.Handle, 165))
                {
                    API.SetPedIntoVehicle(ped.Handle, veh.Handle, 1);
                }
            }
        }

        private static void EnterVehicleClosestDoor()
        {

            if(Game.PlayerPed.IsInVehicle())
            {
                return;
            }

            Vector3 playerPos = Game.PlayerPed.Position;
            if (API.IsControlJustPressed(1, 23))
            {
                int veh = API.GetClosestVehicle(playerPos.X, playerPos.Y, playerPos.Z, 4f, 0, 70);

                if (veh != 0)
                {
                    Vehicle vehicle = new Vehicle(veh);
                    if(!VehicleClass.Motorcycles.Equals(vehicle.ClassType) && !VehicleClass.Cycles.Equals(vehicle.ClassType))
                    {
                        int closestDoor = -1;
                        float closestDoorDist = 5f;
                        for(int i = 0; i < API.GetNumberOfVehicleDoors(veh); i++)
                        {
                            Vector3 doorPos = API.GetEntryPositionOfDoor(veh, i);
                            float dist = API.Vdist2(playerPos.X, playerPos.Y, playerPos.Z, doorPos.X, doorPos.Y, doorPos.Z);
                            if (dist < closestDoorDist && !API.DoesEntityExist(API.GetPedInVehicleSeat(veh, i - 1)))
                            {
                                closestDoor = i - 1;
                                closestDoorDist = dist;
                            }
                        }
                        API.TaskEnterVehicle(Game.PlayerPed.Handle, veh, 10000, closestDoor, 2.0f, 1, 0);
                    }

                }
            }

        }
    }
}
