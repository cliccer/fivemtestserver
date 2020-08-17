using ActualServer.dal;
using ActualServer.dal.dbobjects;
using ActualServer.dao;
using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActualServer.character
{
    class CharacterManager : BaseScript
    {
        Dictionary<string, Character> playerCharacters;
        public CharacterManager()
        {
            playerCharacters = new Dictionary<string, Character>();
            InitCharacterEvents();
        }

        private void InitCharacterEvents()
        {
            EventHandlers["saveNewCharacter"] += new Action<Player, string, string>(SaveCharacter);

            EventHandlers["changeCharacter"] += new Action<Player, int>(ChangeCharacter);

            EventHandlers["listCharactersForAccount"] += new Action<Player>(ListCharactersForAccount);

            EventHandlers["currentCharacter"] += new Action<Player>(CurrentCharacter);
        }

        private void SaveCharacter([FromSource] Player player, string firstName, string lastName)
        {
            Debug.WriteLine("Adding character " + firstName + " " + lastName);
            Character character = new Character(AccountManager.GetAccount(player).Id, firstName, lastName);
            Debug.WriteLine("getaccount good " + character.AccountId.ToString());
            CharacterDAO characterDAO = new CharacterDAO();
            character = characterDAO.SaveOrUpdate(character);
            player.TriggerEvent("createdCharacter", character.Id, character.FirstName, character.LastName);
        }

        private void ChangeCharacter([FromSource] Player player, int id)
        {
            CharacterDAO characterDAO = new CharacterDAO();
            Character newCharacter = characterDAO.GetCharacterByIdAndAccountId(id, AccountManager.GetAccount(player).Id);
            if(newCharacter != null)
            {
                playerCharacters.Remove(player.Handle);
                playerCharacters.Add(player.Handle, newCharacter);
                player.TriggerEvent("changedCharacter", newCharacter.Id, newCharacter.FirstName, newCharacter.LastName);
            } else
            {
                player.TriggerEvent("errorMessage", "Unable to change to character with id " + id.ToString());
            }
        }

        private void ListCharactersForAccount([FromSource] Player player)
        {
            CharacterDAO characterDAO = new CharacterDAO();
            List<Character> characters = characterDAO.GetCharactersForAccount(AccountManager.GetAccount(player).Id);
            IList<IList<dynamic>> charactersList = new List<IList<dynamic>>();
            foreach(Character character in characters)
            {
                charactersList.Add(new List<dynamic> { character.Id, character.FirstName, character.LastName });
            }
            player.TriggerEvent("listCharacters", charactersList);
        }

        private void CurrentCharacter([FromSource] Player player)
        {
            if(playerCharacters.TryGetValue(player.Handle, out Character character))
            {
                player.TriggerEvent("currentCharacter", character.Id, character.FirstName, character.LastName);
            } else
            {
                player.TriggerEvent("errorMessage", "Could not find character, please try again.");
            }
        }
    }
}
