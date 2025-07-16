using System;
using HardwareDriverLayer.HwSettings;
using HwSettingsLib;

namespace HardwareDriverLayer.WrapperInterface
{
    public abstract class HwWrapperBase
    {
        public abstract void SetCANEnvironment(CAN_ENV canenv);
        public abstract void SetCommunicationBaudrate(uint baudrate);
        public abstract void SetCANDataFrameType(CAN_DATA_FRAME_TYPE frametype);
        public abstract void SetCANFDSettings(CANFD_SETTINGS canfdsettings);
        public abstract bool InitializeDriver();
        public abstract bool CloseDriver();
        public abstract bool SendMessage(uint id, byte[] data, uint len, byte payload);
        public abstract bool ReceiveMessage();
        public abstract string GetLastErrorMessage();

        public Action<CANData> OnMessageReceived { get; set; } = null;
        protected void RaiseOnMessageReceived(CANData candata)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(candata);
            }
        }

        public Action<CANData> OnMessageSent { get; set; } = null;
        protected void RaiseOnMessageSent(CANData candata)
        {
            if (OnMessageSent != null)
            {
                OnMessageSent(candata);
            }
        }
    }
}
