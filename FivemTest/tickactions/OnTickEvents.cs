using CitizenFX.Core;
using CitizenFX.Core.Native;
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
        }

        private static void DisableVehicleAirControl()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;

                if (vehicle.IsInAir && VehicleClass.Helicopters != vehicle.ClassType 
                    && VehicleClass.Planes != vehicle.ClassType && VehicleClass.Cycles != vehicle.ClassType)
                {
                    API.DisableControlAction(0, 59, true);
                    API.DisableControlAction(0, 60, true);
                    API.DisableControlAction(0, 61, true);
                    API.DisableControlAction(0, 62, true);
                    API.DisableControlAction(0, 63, true);
                }
                
            }
        }
    }
}
