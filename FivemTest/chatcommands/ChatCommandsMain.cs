

using FivemTest.utils;

namespace FivemTest.chatcommands
{
    static class ChatCommandsMain
    {
        public static void InitAllChatCommands()
        {
            PlayerCommands.InitPlayerPedCommands();
            VehicleCommands.InitVehicleCommands();
            WorldCommands.InitWorldCommands();

            ChatUtil.RemoveAllChatSuggestions();
        }
    }
}
