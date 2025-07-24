using System;
using HwSettingsLib;

namespace CommonHwLib
{
    public class CANEvent
    {
        public string Name { get; private set; }
        public uint CanId { get; set; }

        public Action<CANData> OnMessageReceived { get; set; } = null;

        public CANEvent(string name, uint canid)
        {
            Name = name;
            CanId = canid;
        }

        public void RaiseOnMessageReceived(CANData candata)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(candata);
            }
        }
    }
}
