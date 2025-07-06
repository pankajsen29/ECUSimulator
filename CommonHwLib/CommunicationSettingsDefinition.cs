using System;
using HardwareDriverLayer.HwSettings;

namespace CommonHwLib
{
    public class CommunicationSettings : IDisposable
    {
        public CAN_HW_INTERFACE ACTIVE_CAN_HW { get; set; }
        public CAN_ENV ACTIVE_CAN_ENV { get; set; }
        public UInt32 ACTIVE_COM_BAUDRATE { get; set; }
        public CAN_DATA_FRAME_TYPE ACTIVE_DATA_FRAME { get; set; }
        public CANFD_SETTINGS ACTIVE_CANFD_SETTINGS { get; set; }

        ~CommunicationSettings()
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
                if (ACTIVE_CANFD_SETTINGS != null)
                {
                    ACTIVE_CANFD_SETTINGS.Dispose();
                    ACTIVE_CANFD_SETTINGS = null;
                }
            }
            m_isDisposed = true;
        }
        private bool m_isDisposed = false;
    }
}
