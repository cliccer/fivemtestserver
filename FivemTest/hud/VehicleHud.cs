using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.tickactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.hud
{
    class VehicleHud
    {


        public static void UpdateVehicleHud()
        {
            if (Game.PlayerPed.IsInVehicle())
            {

                DrawSpeedHudElement();

            }

        }

        private static void DrawSpeedHudElement()
        {
            float posX = 0.083f;
            float posY = 0.77f;
            float speed = API.GetEntitySpeed(Game.PlayerPed.Handle) * 2.236936f;

            //drawTxt(timeText, 4, locationColorText, 0.4, screenPosX, screenPosY + 0.048)
            API.SetTextFont(4);
            API.SetTextScale(0.5f, 0.5f);
            API.SetTextColour(255, 255, 255, 255);
            API.SetTextEntry("STRING");
            API.AddTextComponentString(Math.Round(speed, 0).ToString() + " mph");
            API.DrawText(posX, posY);
        }
        
    }
}
