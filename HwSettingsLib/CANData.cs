using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HwSettingsLib
{
    public class CANData
    {
        public string Timestamp;
        public uint Id;
        public ushort Channel;
        public byte Dlc;
        public bool RxData;
        public byte[] Data;
    }
}
