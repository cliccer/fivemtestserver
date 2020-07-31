using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest
{
    public static class Emote
    {
        private static readonly Dictionary<string, string> emotes = new Dictionary<string, string>{
            {"coffee", "WORLD_HUMAN_AA_COFFEE" },
            {"smoke", "WORLD_HUMAN_SMOKING" },
            {"cop", "WORLD_HUMAN_COP_IDLES" },
            {"kneel", "WORLD_HUMAN_MEDIC_KNEEL"},
            {"notepad", "CODE_HUMAN_MEDIC_TIME_OF_DEATH" }
        };
        public static string Get(string emote)
        {
            if(emotes.TryGetValue(emote, out string emoteString))
            {
                return emoteString;
            } else
            {
                return null;
            }

        }
    }
}
