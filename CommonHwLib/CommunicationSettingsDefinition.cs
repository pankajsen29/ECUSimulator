using System;
using HardwareDriverLayer.HwSettings;

namespace CommonHwLib
{
    public class CommunicationSettings
    {
        public CAN_HW_INTERFACE ACTIVE_CAN_HW { get; set; }
        public CAN_ENV ACTIVE_CAN_ENV { get; set; }
        public UInt32 ACTIVE_COM_BAUDRATE { get; set; }
        public CAN_DATA_FRAME_TYPE ACTIVE_DATA_FRAME { get; set; }
        public CANFD_SETTINGS ACTIVE_CANFD_SETTINGS { get; set; }
    }
}
