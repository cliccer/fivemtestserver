using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.utils
{
    class VehicleUtil
    {
        static Dictionary<VehicleClass, List<VehicleHash>> dict = null;

        public static List<VehicleHash> GetVehsForVehicleClass(VehicleClass vehicleClass)
        {
            if (dict != null && dict.TryGetValue(vehicleClass, out List<VehicleHash> vehicleList))
            {
                return vehicleList;
            }
            CreateDictionary();
            dict.TryGetValue(vehicleClass, out List<VehicleHash> vehList);
            return vehList;
        }

        public static async void SpawnVehicle(VehicleHash vehicle)
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
            ChatUtil.SendMessageToClient("[VehicleSpawner]", "You spawned a " + vehicle.ToString(), 255, 255, 255);
        }
        
        private static void CreateDictionary()
        {
            int runs = 0;
            dict = new Dictionary<VehicleClass, List<VehicleHash>>();
            foreach (VehicleHash vehicleHash in Enum.GetValues(typeof(VehicleHash)))
            {
                runs++;
                //Debug.WriteLine("Runs " + runs);

                Model model = new Model(vehicleHash);
                VehicleClass vehClass = Vehicle.GetModelClass(model);
                //Debug.WriteLine("line 31" + vehicleHash.ToString());
                if (dict.TryGetValue(vehClass, out List<VehicleHash> list))
                {
                    //Debug.WriteLine("line 34");
                    if (list.Contains(vehicleHash))
                    {
                        //Debug.WriteLine("Tried to add existing " + vehicleHash.ToString() + " to existing list of " + vehClass.ToString());
                        continue;
                    }
                    //Debug.WriteLine("Added " + vehicleHash.ToString() + " to existing list of " + vehClass.ToString());
                    list.Add(vehicleHash);
                }
                else
                {
                    //Debug.WriteLine("line 45");
                    var newList = new List<VehicleHash>();
                    newList.Add(vehicleHash);
                    dict.Add(vehClass, newList);
                    //Debug.WriteLine("Added " + vehicleHash.ToString() + " to new list of " + vehClass.ToString());
                }
            }
        }
    }
}
