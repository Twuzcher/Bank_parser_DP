using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank_parser.Functions
{
    class Bank
    {
        string name;
        string nameId;

        public Bank(string name, string nameId)
        {
            this.name = name;
            this.nameId = nameId;
        }

        public string getName()
        {
            return name;
        }

        public string getNameId()
        {
            return nameId;
        }
    }
}
