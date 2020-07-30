using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.menus
{
    class PositionRestrictedMenu
    {

        public static void InitMenuMRPDLockerRoom(Menu menu)
        {
            MenuItem menuItem = new MenuItem("Only in MRPD locker room");

            menu.OnMenuOpen += (_menu) =>
            {
                Vector3 pos = Game.PlayerPed.Position;
                if ((pos.X > 449 && pos.X < 460) && (pos.Y > -993 && pos.Y < -987))
                {
                    menu.AddMenuItem(menuItem);
                }
            };

            menu.OnMenuClose += (_menu) =>
            {
                if(menu.GetMenuItems().Contains(menuItem))
                    menu.RemoveMenuItem(menuItem);
            };
        }
    }
}
