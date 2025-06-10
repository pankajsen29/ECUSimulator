using System;
using HardwareDriverLayer.HwSettings;

namespace HardwareDriverLayer.WrapperInterface
{
    public abstract class HwWrapperBase
    {
        public abstract void SetCANEnvironment(CAN_ENV canenv);
        public abstract void SetCommunicationBaudrate(UInt32 baudrate);
        public abstract void SetCANDataFrameType(CAN_DATA_FRAME_TYPE frametype);
        public abstract void SetCANFDSettings(CANFD_SETTINGS canfdsettings);
        public abstract bool InitializeDriver();
        public abstract bool CloseDriver();
        public abstract bool SendMessage(UInt32 id, byte len, byte[] data);
        public abstract bool ReceiveMessage();
        public abstract string GetLastErrorMessage();
    }   
}
