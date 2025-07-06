namespace HardwareDriverLayer.HwSettings
{
    public enum CAN_HW_INTERFACE : byte
    {
        e_VECTOR_XL = 0,
        e_NOT_DEFINED = 99
    }

    public enum CAN_ENV : byte
    {
        e_CAN = 0,
        e_CANFD = 1
    }

    public enum CAN_DATA_FRAME_TYPE : byte
    {
        e_FRAME_STD = 0,
        e_FRAME_FD = 1
    }


    public class CANFD_SETTINGS : IDisposable
    {
        public UInt32 arb_baudrate;
        public UInt32 arb_tseg1;
        public UInt32 arb_tseg2;
        public UInt32 arb_sjw;

        public UInt32 data_baudrate;
        public UInt32 data_tseg1;
        public UInt32 data_tseg2;
        public UInt32 data_sjw;


        ~CANFD_SETTINGS()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (m_isDisposed)
                return;

            if (disposing)
            {
            }
            m_isDisposed = true;
        }
        private bool m_isDisposed = false;
    }
}
