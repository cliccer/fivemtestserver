using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActualServer.dal.dbobjects
{
    class Character
    {

        public int? Id { get; set; }
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Character()
        {

        }

        public Character(int accountId, string firstName, string lastName)
        {
            AccountId = accountId;
            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString()
        {
            return "Character[" + "id=" + Id.ToString() + ", accountId=" + AccountId.ToString() + ", firstName=" + FirstName + ", lastName=" + LastName + "]";
        }
    }
}
