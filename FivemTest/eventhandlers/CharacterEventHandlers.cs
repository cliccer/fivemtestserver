using CitizenFX.Core;
using FivemTest.chatcommands;
using FivemTest.entities;
using FivemTest.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivemTest.eventhandlers
{
    class CharacterEventHandlers : BaseScript
    {

        public CharacterEventHandlers()
        {
            Init();
            Debug.WriteLine("Character eventhandlers initialized");
        }

        private void Init()
        {
            EventHandlers["createdCharacter"] += new Action<int, string, string>(CreatedCharacter);

            EventHandlers["changedCharacter"] += new Action<int, string, string>(ChangedCharacter);

            EventHandlers["listCharacters"] += new Action<dynamic>(ListCharacters);

            EventHandlers["currentCharacter"] += new Action<int, string, string>(CurrentCharacter);
        }

        private void CreatedCharacter(int id, string firstName, string lastName)
        {
            ChatUtil.SendMessageToClient("", "Successfully created character " + id.ToString() + " " + firstName + " " + lastName, 255, 255, 255);
        }

        private void ChangedCharacter(int id, string firstName, string lastName)
        {
            PlayerValues.character = new Character(id, firstName, lastName);
            ChatUtil.SendMessageToClient("Character", "Successfully changed to character " + id.ToString() + " " + firstName + " " + lastName, 255, 255, 255);
        }

        private void ListCharacters(dynamic charactersList) //List<List<object>>
        {
            Debug.WriteLine("Listing characters");
            ChatUtil.SendMessageToClient("Characters", "List of all your characters", 0, 0, 255);
            foreach(List<object> character in (List<object>) charactersList)
            {

                ChatUtil.SendMessageToClient("Character", "#" + character[0].ToString() + " " + (string)character[1] + " " + (string)character[2], 0, 0, 255);
            }
        }

        private List<Character> ConvertDynamicArrayToListOfCharacters(dynamic[][] charactersArray)
        {
            List<Character> characterList = new List<Character>();
            foreach(dynamic[] array in charactersArray)
            {
                Character character = new Character((int) array[0], (string) array[1], (string) array[2]);
                characterList.Add(character);
            }
            return characterList;
        }

        private void CurrentCharacter(int id, string firstName, string lastName)
        {
            ChatUtil.SendMessageToClient("Current character", "#" + id.ToString() + " " + firstName + " " + lastName, 0, 0, 255);
        }
    }
}
