using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActualServer.dbobjects
{
    class Account
    { 
        public int Id { get; set; }

        public string FivemId { get; set; }

        public string Name { get; set; }

        public Account(int id, string fivemId, string name)
        {
            Id = id;
            FivemId = fivemId;
            Name = name;
        }

    }
}
