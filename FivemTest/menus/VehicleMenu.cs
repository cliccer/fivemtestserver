using CitizenFX.Core;
using FivemTest.utils;
using MenuAPI;
using System;
using System.Collections;

namespace FivemTest.menus
{
    static class VehicleMenu
    {

        public static void SpawnVehicleMenu(Menu menu)
        {
            Menu spawnVehicleMenu = new Menu("Spawn vehicle", "Spawn a vehicle");
            MenuController.AddSubmenu(menu, spawnVehicleMenu);

            MenuItem spawnVehicleMenuItem = new MenuItem("Spawn vehicle", "Spawn a vehicle")
            {
                Label = "→"
            };
            foreach (VehicleClass vehicleClass in Enum.GetValues(typeof(VehicleClass)))
            {
                Menu vehicleMenu = new Menu(vehicleClass.ToString());
                MenuController.AddSubmenu(spawnVehicleMenu, vehicleMenu);
                MenuItem menuItem = new MenuItem(vehicleClass.ToString(), vehicleClass.ToString())
                {
                    Label = "→"
                };
                spawnVehicleMenu.AddMenuItem(menuItem);

                MenuController.BindMenuItem(spawnVehicleMenu, vehicleMenu, menuItem);

                foreach (VehicleHash veh in VehicleUtil.GetVehsForVehicleClass(vehicleClass))
                {
                    MenuItem vehicleMenuItem = new MenuItem(veh.ToString(), veh.ToString())
                    {

                    };
                    vehicleMenuItem.ItemData = veh;
                    vehicleMenu.AddMenuItem(vehicleMenuItem);
                }

                vehicleMenu.OnIndexChange += async (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
                {
                    Vehicle oldVeh = Game.PlayerPed.CurrentVehicle;
                    if (oldVeh != null)
                    {
                        oldVeh.Delete();
                    }

                    Model model = new Model(_newItem.ItemData);
                    Vehicle newVeh = await World.CreateVehicle(model, Game.PlayerPed.Position, 0f);
                    Game.PlayerPed.SetIntoVehicle(newVeh, VehicleSeat.Driver);
                };

                vehicleMenu.OnItemSelect += (_menu, _item, _index) =>
                {
                    Vehicle veh = Game.PlayerPed.CurrentVehicle;
                    veh.Mods.InstallModKit();
                    veh.Mods[VehicleModType.Engine].Index = veh.Mods[VehicleModType.Engine].ModCount - 1;
                    veh.Mods[VehicleModType.Brakes].Index = veh.Mods[VehicleModType.Brakes].ModCount - 1;
                    veh.Mods[VehicleModType.Transmission].Index = veh.Mods[VehicleModType.Transmission].ModCount - 1;

                    veh.Mods[VehicleToggleModType.Turbo].IsInstalled = true;
                    veh.Mods[VehicleToggleModType.XenonHeadlights].IsInstalled = true;
                    MenuController.CloseAllMenus();
                };

                vehicleMenu.OnMenuClose += (_menu) =>
                {
                    Vehicle oldVeh = Game.PlayerPed.CurrentVehicle;
                    if (oldVeh != null && !oldVeh.Mods[VehicleToggleModType.XenonHeadlights].IsInstalled)
                    {
                        oldVeh.Delete();
                    }
                };


            }
            menu.AddMenuItem(spawnVehicleMenuItem);
            MenuController.BindMenuItem(menu, spawnVehicleMenu, spawnVehicleMenuItem);
        }


        public static void ModVehicleMenu(Menu menu)
        {
            Menu modVehMenu = new Menu("Modify vehicle", "Modify vehicle");
            MenuController.AddSubmenu(menu, modVehMenu);

            MenuItem modVehMenuItem = new MenuItem("Modify vehicle", "Modify vehicle")
            {
                Label = "→"
            };

            menu.AddMenuItem(modVehMenuItem);
            MenuController.BindMenuItem(menu, modVehMenu, modVehMenuItem);

            menu.OnItemSelect += (_menu, _item, _index) =>
            {
                modVehMenu.ClearMenuItems();
                CreateModMenu(modVehMenu);
            };


        }


        private static void CreateModMenu(Menu modVehMenu)
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh == null)
            {
                return;
            }
            veh.Mods.InstallModKit();

            //TODO Add VehicleModTypeToggle here

            foreach (VehicleMod mod in veh.Mods.GetAllMods())
            {
                if (Enum.TryParse<VehicleModType>(mod.ModType.ToString(), out VehicleModType modType))
                {
                    Menu modMenu = new Menu(modType.ToString(), modType.ToString());

                    ArrayList dataList = new ArrayList();
                    dataList.Add(modType);
                    dataList.Add(modMenu);


                    MenuController.AddSubmenu(modVehMenu, modMenu);
                    MenuItem modMenuItem = new MenuItem(modType.ToString(), modType.ToString())
                    {
                        Label = "→",
                        ItemData = dataList

                    };
                    modVehMenu.AddMenuItem(modMenuItem);
                    MenuController.BindMenuItem(modVehMenu, modMenu, modMenuItem);
                }
            }
            modVehMenu.OnItemSelect += (_menu, _item, _index) =>
            {
                Menu temp = _item.ItemData[1];
                temp.ClearMenuItems();
                CreateModTypeMenu(_item.ItemData[1], _item.ItemData[0], veh);
            };
        }

        private static void CreateModTypeMenu(Menu modMenu, VehicleModType modType, Vehicle veh)
        {
            for (int i = 0; i <= veh.Mods[modType].ModCount - 1; i++)
            {
                MenuItem modTypeMenuItem = null;
                if (veh.Mods[modType].Index == i)
                {
                    modTypeMenuItem = new MenuItem(i.ToString(), i.ToString())
                    {
                        RightIcon = MenuItem.Icon.TICK
                    };
                }
                else
                {
                    modTypeMenuItem = new MenuItem(i.ToString(), i.ToString())
                    {

                    };
                }

                modTypeMenuItem.ItemData = i;
                modMenu.AddMenuItem(modTypeMenuItem);
            }

            modMenu.OnMenuClose += (_menu) =>
            {

                foreach (MenuItem item in modMenu.GetMenuItems())
                {
                    if (item.RightIcon.Equals(MenuItem.Icon.TICK))
                    {
                        veh.Mods[modType].Index = item.ItemData;
                    }
                }
            };

            modMenu.OnIndexChange += (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
            {
                veh.Mods[modType].Index = _newItem.ItemData;
            };

            modMenu.OnItemSelect += (_menu, _item, _index) =>
            {
                foreach (MenuItem item in modMenu.GetMenuItems())
                {
                    item.RightIcon = MenuItem.Icon.NONE;
                }
                _item.RightIcon = MenuItem.Icon.TICK;
                veh.Mods[modType].Index = _item.ItemData;
            };
        }
    }
}
