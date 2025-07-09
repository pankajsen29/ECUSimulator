using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTraceViewLib.Model
{
    public class CANMessage
    {
        public string Timestamp { get; set; }
        public string ID { get; set; }
        public string DLC { get; set; }
        public byte[] Data { get; set; }
    }
}
