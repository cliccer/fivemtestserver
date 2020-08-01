using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivemTest.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FivemTest.actions
{
    class PedActions
    {
        public static void ShuffleSeatAction()
        {
            PedValues.shuffleSeat = true;
            Debug.WriteLine("shuffleSeat = true");
            Thread.Sleep(3000);
            PedValues.shuffleSeat = false;
            Debug.WriteLine("shuffleSeat = false");
        }
    }
}
