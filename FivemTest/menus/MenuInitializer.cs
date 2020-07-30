using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using MenuAPI;

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
                VehicleMenu.SpawnVehicleMenu(menu);
                VehicleMenu.ModVehicleMenu(menu);
                PositionRestrictedMenu.InitMenuMRPDLockerRoom(menu);
            }), false); 
        }

        
    }
}
