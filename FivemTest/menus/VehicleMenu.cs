using CitizenFX.Core;
using CitizenFX.Core.NaturalMotion;
using FivemTest.utils;
using MenuAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;

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
                    Vector3 position;
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        position = Game.PlayerPed.CurrentVehicle.Position;
                    } else
                    {
                        position = Game.PlayerPed.Position;
                    }
                    Vehicle newVeh = await World.CreateVehicle(model, position, Game.PlayerPed.Heading);
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

            //TODO Add VehicleModTypeToggle

            Menu colorMenu = new Menu("Color", "Vehicle colors");

            MenuItem colorMenuItem = new MenuItem("Color", "Vehicle colors")
            {
                Label = "→"
            };
            modVehMenu.AddMenuItem(colorMenuItem);

            MenuController.AddSubmenu(modVehMenu, colorMenu);
            MenuController.BindMenuItem(modVehMenu, colorMenu, colorMenuItem);

            ArrayList colorTypes = new ArrayList { "Primary", "Secondary", "Pearlescent", "Rim"};

            foreach (string colorType in colorTypes)
            {
                Menu colorTypeMenu = new Menu(colorType + " color");
                ArrayList dataList = new ArrayList();
                dataList.Add(colorType);
                dataList.Add(colorTypeMenu);

                MenuItem colorTypeMenuItem = new MenuItem(colorType + " color") 
                { 
                    ItemData = dataList
                };

                colorMenu.AddMenuItem(colorTypeMenuItem);
                MenuController.AddSubmenu(colorMenu, colorTypeMenu);
                MenuController.BindMenuItem(colorMenu, colorTypeMenu, colorTypeMenuItem);
            }
            colorMenu.OnItemSelect += (_menu, _item, _index) =>
            {
                AddColorItems(_item.ItemData[1], _item.ItemData[0]);
            };

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
                if(_item.ItemData != null)
                {

                Menu temp = _item.ItemData[1];
                temp.ClearMenuItems();
                CreateModTypeMenu(_item.ItemData[1], _item.ItemData[0], veh);
                }
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

        private static void AddColorItems(Menu colorTypeMenu, String colorType)
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;

            //TODO Fix ordering of colors

            Array vehicleColors = Enum.GetValues(typeof(VehicleColor));

            string[] vehicleColors2 = vehicleColors.OfType<object>().Select(o => o.ToString()).ToArray();

            Array.Sort<string>(vehicleColors2);
            foreach (string vehCS in vehicleColors2)
            {
                Enum.TryParse<VehicleColor>(vehCS, out VehicleColor vehC);
                ArrayList dataList = new ArrayList();
                dataList.Add(vehC);
                dataList.Add(colorType);

                MenuItem colorMenuItem = new MenuItem(vehC.ToString())
                {
                    ItemData = dataList
                };
                if(("Primary".Equals(colorType) && vehC.Equals(veh.Mods.PrimaryColor)) 
                    || ("Secondary".Equals(colorType) && vehC.Equals(veh.Mods.SecondaryColor))
                    || ("Pearlescent".Equals(colorType) && vehC.Equals(veh.Mods.PearlescentColor))
                    || ("Rim".Equals(colorType) && vehC.Equals(veh.Mods.RimColor))
                    || ("Neon".Equals(colorType) && vehC.Equals(veh.Mods.NeonLightsColor))
                    || ("Tire smoke".Equals(colorType) && vehC.Equals(veh.Mods.TireSmokeColor))){
                    colorMenuItem.RightIcon = MenuItem.Icon.TICK;
                }
                colorTypeMenu.AddMenuItem(colorMenuItem);
            }


            colorTypeMenu.OnIndexChange += (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
            {
                VehicleUtil.SetColorOnVehicle(_newItem.ItemData[1], _newItem.ItemData[0]);

            };

            colorTypeMenu.OnItemSelect += (_menu, _item, _index) =>
            {
                foreach(MenuItem menuItem in _menu.GetMenuItems())
                {
                    if (menuItem.RightIcon.Equals(MenuItem.Icon.TICK))
                    {
                        menuItem.RightIcon = MenuItem.Icon.NONE;
                        break;
                    }
                }
                _item.RightIcon = MenuItem.Icon.TICK;
            };

            colorTypeMenu.OnMenuClose += (_menu) =>
            {
                foreach (MenuItem menuItem in _menu.GetMenuItems())
                {
                    if (menuItem.RightIcon.Equals(MenuItem.Icon.TICK))
                    {
                        VehicleUtil.SetColorOnVehicle(menuItem.ItemData[1], menuItem.ItemData[0]);
                        break;
                    }
                }

            };
        }
    }
}
