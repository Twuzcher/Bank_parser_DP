using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace bank_parser.Functions
{
    interface IBankFunction
    {
        DataSet GetInformation();
    }
}
