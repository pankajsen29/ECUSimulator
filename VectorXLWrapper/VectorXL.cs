using System;
using HardwareDriverLayer.HwSettings;
using HardwareDriverLayer.WrapperInterface;
using vxlapi_NET;


namespace HardwareDriverLayer.Wrapper
{
    public class VectorXL : HwWrapperBase
    {
        private CAN_ENV _active_CAN_Env;        
        private uint _com_Baudrate;
        private CAN_DATA_FRAME_TYPE _active_Data_Frame;
        private CANFD_SETTINGS _canfd_settings;
        private String _lastErrorMessage = String.Empty;

        private const String _appName = "ECUSim";
        private uint _appChannel = 0;
        private bool _isDriverInitialized;
        private static XLDriver _xlDriverObj;
        private static XLDefine.XL_Status _xlStatus = XLDefine.XL_Status.XL_ERR_HW_NOT_PRESENT;
        private XLDefine.XL_HardwareType _hwType;
        private uint _hwIndex;
        private uint _hwChannel;
        private XLDefine.XL_BusTypes _busType;
        private static int _portHandle;
        private ulong _accessMask;
        private ulong _permissionMask;

        private static readonly Lazy<VectorXL> _sVectorXL = new Lazy<VectorXL>(() => new VectorXL());

        private VectorXL()
        {
            _active_CAN_Env = CAN_ENV.e_CAN; // Default to CAN environment
            _com_Baudrate = 500000; // Default CAN baudrate
            _active_Data_Frame = CAN_DATA_FRAME_TYPE.e_FRAME_STD; // Default to Standard CAN frame type
            _canfd_settings = null; // Default CAN FD settings are null
        }

        /// <summary>
        /// returns the singleton instance of the VectorXL wrapper.
        /// </summary>
        /// <returns></returns>
        public static VectorXL GetVectorXLWrapper()
        {
            return _sVectorXL.Value;
        }

        /// <summary>
        /// Indicate the communication environment (CAN / CAN FD) for the CAN driver.
        /// </summary>
        /// <param name="canenv"></param>
        public override void SetCANEnvironment(CAN_ENV canenv)
        {
            _active_CAN_Env = canenv;
        }

        /// <summary>
        /// Pass the communication baudrate to be used for CAN communication.
        /// </summary>
        /// <param name="baudrate"></param>
        public override void SetCommunicationBaudrate(uint baudrate)
        {
            _com_Baudrate = baudrate;
        }

        /// <summary>
        /// Indicate the type of data frame (Standard CAN or FD) to be used while CAN FD communication.
        /// </summary>
        /// <param name="frametype"></param>
        public override void SetCANDataFrameType(CAN_DATA_FRAME_TYPE frametype)
        {
            _active_Data_Frame = frametype;
        }

        /// <summary>
        /// Pass the CAN FD settings for initializing the Vector XL driver for CAN FD communication.
        /// </summary>
        /// <param name="canfdsettings"></param>
        public override void SetCANFDSettings(CANFD_SETTINGS canfdsettings)
        {
            _canfd_settings = canfdsettings;
        }

        private bool GetXlDriverObject()
        {
            try
            {
                if (null == _xlDriverObj)
                {
                    _xlDriverObj = new XLDriver();
                }
                return (null != _xlDriverObj);

            }
            catch (Exception ex)
            {
                _lastErrorMessage = "Error initializing Vector XLDriver: " + ex.Message;
                return false;
            }
        }

        private bool OpenXlDriver()
        {
            try
            {
                if (null != _xlDriverObj)
                {
                    _xlStatus = _xlDriverObj.XL_OpenDriver();
                }
                return (XLDefine.XL_Status.XL_SUCCESS == _xlStatus);
            }
            catch (Exception ex)
            {
                _lastErrorMessage = "Error opening the Vector XLDriver: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Gets info about the application assignment which is set in the Vector Hardware Configuration Tool.
        /// Otherwise, it will set the application assignment in it and then opens it.
        /// </summary>
        /// <returns></returns>
        private bool GetApplicationConfig()
        {
            _busType = XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN;
            _xlStatus = _xlDriverObj.XL_GetApplConfig(_appName, _appChannel, ref _hwType, ref _hwIndex, ref _hwChannel, _busType);
            if (XLDefine.XL_Status.XL_SUCCESS != _xlStatus)
            {
                _xlDriverObj.XL_SetApplConfig(_appName, _appChannel, _hwType, _hwIndex, _hwChannel, _busType);
                _xlDriverObj.XL_PopupHwConfig();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the channel mask of a specified hardware channel.
        /// </summary>
        /// <returns></returns>
        private bool GetChannelMask()
        {
            _accessMask = _xlDriverObj.XL_GetChannelMask(_hwType, (int)_hwIndex, (int)_hwChannel);
            _permissionMask = _accessMask;

            if (0 == _accessMask)
            {
                _lastErrorMessage = "Channel mask is 0 for the specified hardware channel!";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Prepares the CANFDConfig for XL driver based on the settings provided.
        /// </summary>
        /// <returns></returns>
        private XLClass.XLcanFdConf GetCANFDConfig()
        {
            if (null == _canfd_settings)
            {
                _canfd_settings = new CANFD_SETTINGS();
            }

            // Setting to default CAN FD parameters, if any parameter is passed as 0
            if (_canfd_settings.data_baudrate == 0 || _canfd_settings.data_tseg1 == 0 || _canfd_settings.data_tseg2 == 0 || _canfd_settings.data_sjw == 0 ||
                _canfd_settings.arb_baudrate == 0 || _canfd_settings.arb_tseg1 == 0 || _canfd_settings.arb_tseg2 == 0 || _canfd_settings.arb_sjw == 0)
            {
                _canfd_settings.data_baudrate = 2000000;
                _canfd_settings.data_tseg1 = 7;
                _canfd_settings.data_tseg2 = 2;
                _canfd_settings.data_sjw = 2;
                _canfd_settings.arb_baudrate = 500000;
                _canfd_settings.arb_tseg1 = 7;
                _canfd_settings.arb_tseg2 = 2;
                _canfd_settings.arb_sjw = 2;
            }

            XLClass.XLcanFdConf canFdConf = new XLClass.XLcanFdConf();

            canFdConf.arbitrationBitRate = _canfd_settings.arb_baudrate; //note: arbitrationBitRate is used while transmitting CAN 2.0 frames
            canFdConf.tseg1Abr = _canfd_settings.arb_tseg1;
            canFdConf.tseg2Abr = _canfd_settings.arb_tseg2;
            canFdConf.sjwAbr = _canfd_settings.arb_sjw;
            canFdConf.dataBitRate = _canfd_settings.data_baudrate; //note: dataBitRate is ignored while transmitting CAN 2.0 frames
            canFdConf.tseg1Dbr = _canfd_settings.data_tseg1;
            canFdConf.tseg2Dbr = _canfd_settings.data_tseg2;
            canFdConf.sjwDbr = _canfd_settings.data_sjw;

            return canFdConf;
        }

        /// <summary>
        /// Initializes the CAN driver based on the active CAN environment.
        /// </summary>
        /// <remarks>The initialization process depends on the value of the active CAN environment.  If
        /// the environment is set to <see cref="CAN_ENV.e_CANFD"/>, the method initializes the driver for CAN FD.
        /// Otherwise, it initializes the driver for standard CAN.</remarks>
        /// <returns><see langword="true"/> if the driver is successfully initialized; otherwise, <see langword="false"/>.</returns>
        public override bool InitializeDriver()
        {
            if (_active_CAN_Env == CAN_ENV.e_CANFD)
            {
                _isDriverInitialized = InitializeVectorXLCANFD();
            }
            else
            {
                _isDriverInitialized = InitializeVectorXLCAN();
            }
            return _isDriverInitialized;
        }

        /// <summary>
        /// Initializes the Vector XL CAN driver.
        /// </summary>
        /// <returns><see langword="true"/> if the driver is successfully initialized; otherwise, <see langword="false"/>.</returns>
        private bool InitializeVectorXLCAN()
        {
            _xlStatus = XLDefine.XL_Status.XL_ERR_HW_NOT_PRESENT;
            if (_isDriverInitialized)
            {
                CloseDriver();
            }
            _isDriverInitialized = false;
            
            if (!GetXlDriverObject())
            {
                return false;
            }

            if (!OpenXlDriver())
            {
                return false;
            }

            if(!GetApplicationConfig())
            {
                return false;
            }

            if (0 == _com_Baudrate)
            {
                _com_Baudrate = 500000;
            }

            if(!GetChannelMask())
            {
                return false;
            }

            //Opens a port for a BUS type(e.g., CAN) and grants access to the different channels that are selected by '_accessMask'
            _xlStatus = _xlDriverObj.XL_OpenPort(ref _portHandle, _appName, _accessMask, ref _permissionMask, 1024, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION, _busType);
            if ((XLDefine.XL_Status.XL_SUCCESS != _xlStatus))
            {
                _lastErrorMessage = "Could not open the port for the specified BUS type!";
                return false;
            }

            _xlStatus = _xlDriverObj.XL_DeactivateChannel(_portHandle, _accessMask);

            _xlStatus = _xlDriverObj.XL_CanSetChannelBitrate(_portHandle, _accessMask, _com_Baudrate);

            _xlStatus = _xlDriverObj.XL_ActivateChannel(_portHandle, _accessMask, _busType, XLDefine.XL_AC_Flags.XL_ACTIVATE_RESET_CLOCK);
            if (XLDefine.XL_Status.XL_SUCCESS != _xlStatus)
            {
                _lastErrorMessage = "Failed in activating the channel, XL Status:  " + _xlStatus.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Initializes the Vector XL CAN FD driver.
        /// </summary>
        /// <returns><see langword="true"/> if the driver is successfully initialized; otherwise, <see langword="false"/>.</returns>
        private bool InitializeVectorXLCANFD()
        {
            _xlStatus = XLDefine.XL_Status.XL_ERR_HW_NOT_PRESENT;
            if (_isDriverInitialized)
            {
                CloseDriver();
            }
            _isDriverInitialized = false;
            if (!GetXlDriverObject())
            {
                return false;
            }

            if (!OpenXlDriver())
            {
                return false;
            }

            if (!GetApplicationConfig())
            {
                return false;
            }

            if (!GetChannelMask())
            {
                return false;
            }

            //Opens a port for a BUS type (e.g., CAN) and grants access to the different channels that are selected by '_accessMask'
            _xlStatus = _xlDriverObj.XL_OpenPort(ref _portHandle, _appName, _accessMask, ref _permissionMask, 16000, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION_V4, _busType);
            if ((XLDefine.XL_Status.XL_SUCCESS != _xlStatus))
            {
                _lastErrorMessage = "Could not open the port for the specified BUS type!";
                return false;
            }

            _xlStatus = _xlDriverObj.XL_DeactivateChannel(_portHandle, _accessMask);

            // Gets the CANFDconfig
            XLClass.XLcanFdConf canFdConf = GetCANFDConfig();
            _xlStatus = _xlDriverObj.XL_CanFdSetConfiguration(_portHandle, _accessMask, canFdConf);

            if (XLDefine.XL_Status.XL_SUCCESS != _xlStatus)
            {
                _lastErrorMessage = "Supplied CANFD settings are not accepted by the driver!";
            }

            _xlStatus = _xlDriverObj.XL_ActivateChannel(_portHandle, _accessMask, _busType, XLDefine.XL_AC_Flags.XL_ACTIVATE_RESET_CLOCK);

            if (XLDefine.XL_Status.XL_SUCCESS != _xlStatus)
            {
                _lastErrorMessage = "Failed in activating the channel, XL Status:  " + _xlStatus.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Closes the driver and releases any associated resources.
        /// </summary>
        /// <remarks>This method deactivates the channel, closes the port, and shuts down the driver if it is currently initialized.
        /// After calling this method, the driver is no longer available for use until reinitialized.
        /// </remarks>
        /// <returns><see langword="true"/> if the driver was successfully closed; otherwise, <see langword="false"/>.</returns>
        public override bool CloseDriver()
        {
            if ((null != _xlDriverObj) && (_isDriverInitialized))
            {
                try
                {
                    _xlStatus = _xlDriverObj.XL_DeactivateChannel(_portHandle, _accessMask);
                    _xlStatus = _xlDriverObj.XL_ClosePort(_portHandle);
                    _xlStatus = _xlDriverObj.XL_CloseDriver();
                }
                catch
                {
                }
            }
            _isDriverInitialized = false;
            return !_isDriverInitialized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="len"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool SendMessage(uint id, byte len, byte[] data)
        {
            return false; // Not implemented yet
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool ReceiveMessage()
        {
            return false; // Not implemented yet
        }

        /// <summary>
        /// Retrieves the last error message recorded.
        /// </summary>
        /// <returns>
        /// The most recent error message as a <see cref="string"/>, or an empty string if no error has been recorded.
        /// </returns>
        public override string GetLastErrorMessage()
        {
            return _lastErrorMessage;
        }
    }
}
