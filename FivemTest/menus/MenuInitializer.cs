using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.utils;
using System;
using MenuAPI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FivemTest.menus
{
    class MenuInitializer : BaseScript
    {

        public static void InitMenu()
        {
            API.RegisterKeyMapping("openMenu", "Opens M menu", "keyboard", "m");

            API.RegisterCommand("openMenu", new Action<int>(src => {
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            Menu menu = new Menu("Menu", "");
            MenuController.AddMenu(menu);
                AddSpawnVehicleMenu(menu);
                AddVehicleModMenu(menu);
            }), false); 
        }

        private static void AddSpawnVehicleMenu(Menu menu)
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

                vehicleMenu.OnItemSelect += (_menu, _item, _index) =>
                {
                    Debug.WriteLine($"OnIndexChange: [{_menu}, {_item.ItemData}, {_index}");
                    VehicleUtil.SpawnVehicle(_item.ItemData);
                    MenuController.CloseAllMenus();
                };


            }
            menu.AddMenuItem(spawnVehicleMenuItem);
            MenuController.BindMenuItem(menu, spawnVehicleMenu, spawnVehicleMenuItem);
        }

        private static void AddVehicleModMenu(Menu menu)
        {
            Menu modVehMenu = new Menu("Modify vehicle", "Modify vehicle");
            MenuController.AddSubmenu(menu, modVehMenu);

            MenuItem modVehMenuItem = new MenuItem("Modify vehicle", "Modify vehicle")
            {
                Label = "→"
            };

            menu.AddMenuItem(modVehMenuItem);
            MenuController.BindMenuItem(menu, modVehMenu, modVehMenuItem);

            Vehicle veh = Game.PlayerPed.LastVehicle;
            veh.Mods.InstallModKit();
            foreach (VehicleMod mod in veh.Mods.GetAllMods())
            {
                if(Enum.TryParse<VehicleModType>(mod.ModType.ToString(), out VehicleModType modType))
                {
                    Menu modMenu = new Menu(modType.ToString(), modType.ToString());

                    MenuController.AddSubmenu(modVehMenu, modMenu);
                    MenuItem modMenuItem = new MenuItem(modType.ToString(), modType.ToString())
                    {
                        Label = "→"
                    };
                    modVehMenu.AddMenuItem(modMenuItem);
                    MenuController.BindMenuItem(modVehMenu, modMenu, modMenuItem);
                    
                    Debug.WriteLine("ModType " + modType.ToString() + "");

                    for(int i = 0; i <= veh.Mods[modType].ModCount - 1; i++)
                    {
                        int installedMod = 0;
                        installedMod = veh.Mods[modType].Index;

                        MenuItem modTypeMenuItem;
                        if(installedMod == i)
                        {
                            modTypeMenuItem = new MenuItem(i.ToString(), i.ToString())
                            {
                                RightIcon = MenuItem.Icon.TICK
                            };
                        } else
                        {
                            modTypeMenuItem = new MenuItem(i.ToString(), i.ToString())
                            {

                            };
                        }
                        
                        modTypeMenuItem.ItemData = i;
                        modMenu.AddMenuItem(modTypeMenuItem);
                        


                        
                        modMenu.OnMenuOpen += (_menu) =>
                        {
                            Debug.WriteLine("OnMenuOpen");
                            installedMod = veh.Mods[modType].Index;
                        };

                        modMenu.OnMenuClose += (_menu) =>
                        {
                            veh.Mods[modType].Index = installedMod;
                        };

                        modMenu.OnIndexChange += (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
                        {
                            veh.Mods[modType].Index = _newItem.ItemData;
                        };

                        modMenu.OnItemSelect += (_menu, _item, _index) =>
                        {
                            Debug.WriteLine($"OnIndexChange: [{_menu}, {_item}, {_index}");
                            installedMod = _item.ItemData;
                        };
                    }
                }
                
            }
        }

        private static void ModMenu_OnMenuOpen(Menu menu)
        {
            throw new NotImplementedException();
        }
    }
}
