using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public abstract class Identifier_FF : Identifier
    {
        public byte Key = 0xFF;

        public abstract byte GetKeySub();
    }
}
