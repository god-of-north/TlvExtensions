using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GON.Datasructures.Tlv
{
    public enum BerTlvTagClass
    {
        Universal = 0,
        Application = 1,
        ContextSpecific = 2,
        Private = 3
    }
}
