using HardwareDriverLayer.HwSettings;
using HardwareDriverLayer.WrapperInterface;
using HwSettingsLib;
using System;
using System.IO;
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
        /// sends a CAN message on the CAN BUS
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="len">total length of data bytes</param>
        /// <param name="payload">length of each frame, used only for CANFD frames</param>
        /// <returns></returns>
        public override bool SendMessage(uint id, byte[] data, uint len, byte payload)
        {
            bool returnState = false;
            if (len > 0)
            {
                if ((!_isDriverInitialized) || (null == data) || (data.Length < len))
                {
                    return false;
                }
                if (_active_CAN_Env == CAN_ENV.e_CANFD)
                {
                    returnState = SendCANFDMessage(id, data, len, payload);
                }
                else
                {
                    returnState = SendCANMessage(id, data, len);
                }
            }
            return returnState;
        }

        /// <summary>
        /// transmits a CAN FD message on the CAN FD BUS.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private bool SendCANFDMessage(uint id, byte[] data, uint len, byte payload)
        {
            bool returnState = false;
            byte numberOfFrames = GetNumberOfTxFrames(len, ref payload);
            if (1 < numberOfFrames)
            {
                // Create an event collection with required messages (events)
                var xlEventCol = new XLClass.xl_canfd_event_collection(0);

                byte pos = 0;
                while (pos < numberOfFrames)
                {
                    var xlCanTxEv = new XLClass.XLcanTxEvent();

                    xlCanTxEv.tag = XLDefine.XL_CANFD_TX_EventTags.XL_CAN_EV_TAG_TX_MSG;
                    xlCanTxEv.tagData.canId = id;
                    if ((id > 0x7FF)) // If the ID is greater than 11 bits, set the extended bit
                    {
                        xlCanTxEv.tagData.canId |= 0x80000000;
                    }

                    byte actuallen = payload;
                    byte actualpos = Convert.ToByte(payload * pos);
                    if ((actualpos + actuallen) > len)
                    {
                        actuallen = Convert.ToByte(len - actualpos);
                    }
                    Array.Copy(data, actualpos, xlCanTxEv.tagData.data, 0, actuallen);

                    xlCanTxEv.tagData.dlc = VectorUtil.GET_TRANSMIT_XL_CANFD_DLC(actuallen);
                    if (_active_Data_Frame == CAN_DATA_FRAME_TYPE.e_FRAME_FD)
                    {
                        xlCanTxEv.tagData.msgFlags = XLDefine.XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_BRS | XLDefine.XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_EDL;
                    }
                    else
                    {
                        //xlCanTxEv.tagData.dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES;
                        xlCanTxEv.tagData.msgFlags = 0; //Set to 0 to transmit a CAN 2.0 frame                                        
                    }

                    xlEventCol.xlCANFDEvent.Add(xlCanTxEv);
                    xlEventCol.messageCount++;
                    pos++;
                }

                // Transmit event collection
                uint messageCounterSent = 0;
                _xlStatus = _xlDriverObj.XL_CanTransmitEx(_portHandle, _accessMask, ref messageCounterSent, xlEventCol);
                returnState = (_xlStatus == XLDefine.XL_Status.XL_SUCCESS);
                if (returnState)
                {
                    foreach (var xlCanTxEv in xlEventCol.xlCANFDEvent)
                    {
                        NotifyMessageSent(xlCanTxEv.tagData.canId, xlCanTxEv.tagData.data, (byte)xlCanTxEv.tagData.dlc, payload);
                    }
                }
            }
            else
            {
                var xlCanTxEv = new XLClass.XLcanTxEvent();

                xlCanTxEv.tag = XLDefine.XL_CANFD_TX_EventTags.XL_CAN_EV_TAG_TX_MSG;
                xlCanTxEv.tagData.canId = id;
                if ((id > 0x7FF)) // If the ID is greater than 11 bits, set the extended bit
                {
                    xlCanTxEv.tagData.canId |= 0x80000000;
                }
                Array.Copy(data, xlCanTxEv.tagData.data, len);

                byte actuallen = payload;
                xlCanTxEv.tagData.dlc = VectorUtil.GET_TRANSMIT_XL_CANFD_DLC(actuallen);
                if (_active_Data_Frame == CAN_DATA_FRAME_TYPE.e_FRAME_FD)
                {
                    xlCanTxEv.tagData.msgFlags = XLDefine.XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_BRS | XLDefine.XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_EDL;
                }
                else
                {
                    //xlCanTxEv.tagData.dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES;
                    xlCanTxEv.tagData.msgFlags = 0; //Set to 0 to transmit a CAN 2.0 frame
                }

                // Transmit events
                uint messageCounterSent = 0;
                _xlStatus = _xlDriverObj.XL_CanTransmitEx(_portHandle, _accessMask, ref messageCounterSent, xlCanTxEv);
                returnState = (_xlStatus == XLDefine.XL_Status.XL_SUCCESS);
                if (returnState)
                {
                    NotifyMessageSent(xlCanTxEv.tagData.canId, xlCanTxEv.tagData.data, (byte)xlCanTxEv.tagData.dlc, payload);
                }
            }
            return returnState;
        }

        /// <summary>
        /// transmits a CAN message on the CAN BUS.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private bool SendCANMessage(uint id, byte[] data, uint len)
        {
            bool returnState = false;
            var payload = (byte)8; // For CAN, the payload is always 8 bytes
            byte numberOfFrames = GetNumberOfTxFrames(len, ref payload);
            if (1 < numberOfFrames)
            {
                var canCol = new XLClass.xl_event_collection(0);
                byte pos = 0;
                while (pos < numberOfFrames)
                {
                    var canEv = new XLClass.xl_event();

                    canEv.tagData.can_Msg.id = id;
                    if ((id > 0x7FF)) // If the ID is greater than 11 bits, set the extended bit
                    {
                        canEv.tagData.can_Msg.id |= 0x80000000;
                    }
                    canEv.tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;

                    byte actuallen = 8;
                    byte actualpos = Convert.ToByte(8 * pos);
                    if ((actualpos + actuallen) > len)
                    {
                        actuallen = Convert.ToByte(len - actualpos);
                    }
                    canEv.tagData.can_Msg.dlc = actuallen;
                    Array.Copy(data, actualpos, canEv.tagData.can_Msg.data, 0, actuallen);

                    canCol.xlEvent.Add(canEv);
                    canCol.messageCount++;
                    pos++;
                }
                _xlStatus = _xlDriverObj.XL_CanTransmit(_portHandle, _accessMask, canCol);
                returnState = ((_xlStatus == XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY) || (_xlStatus == XLDefine.XL_Status.XL_SUCCESS));
                if (returnState)
                {
                    foreach (var canEv in canCol.xlEvent)
                    {
                        NotifyMessageSent(canEv.tagData.can_Msg.id, canEv.tagData.can_Msg.data, (byte)canEv.tagData.can_Msg.dlc, (uint)8);
                    }
                }
            }
            else
            {
                var canEv = new XLClass.xl_event();
                canEv.tagData.can_Msg.id = id;
                if ((id > 0x7FF)) // If the ID is greater than 11 bits, set the extended bit
                {
                    canEv.tagData.can_Msg.id |= 0x80000000;
                }

                byte actuallen = 8;
                canEv.tagData.can_Msg.dlc = actuallen;
                canEv.tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;
                Array.Copy(data, canEv.tagData.can_Msg.data, len);

                _xlStatus = _xlDriverObj.XL_CanTransmit(_portHandle, _accessMask, canEv);
                returnState = (_xlStatus == XLDefine.XL_Status.XL_SUCCESS);
                if (returnState)
                {
                    NotifyMessageSent(canEv.tagData.can_Msg.id, canEv.tagData.can_Msg.data, (byte)canEv.tagData.can_Msg.dlc, (uint)8);
                }
            }
            return returnState;
        }

        /// <summary>
        /// Calculates the number of CAN frames required to transmit the specified data length 
        /// based on the active CAN environment and payload size.
        /// </summary>
        /// <remarks>The calculation considers the active CAN environment and data frame type. For CAN FD
        /// environments, the payload size may be adjusted to 8 bytes if FD frames are not required. For non-CAN FD
        /// environments, the payload size is treated as 8 bytes, and any remaining bytes that do not fit into a full
        /// frame will require an additional message.</remarks>
        /// <param name="datalen">The total length of the data to be transmitted, in bytes.</param>
        /// <param name="payload">The size of the payload, in bytes, for each CAN message. If less than 8, it will be adjusted to 8.</param>
        /// <returns>The total number of CAN frames required to transmit the specified data length.</returns>
        private byte GetNumberOfTxFrames(uint datalen, ref byte payload)
        {
            byte numberOfFrames = 0;
            if (payload < 8)
            {
                payload = 8; // If payload is less than 8, set it to 8 bytes
            }
            if (_active_CAN_Env == CAN_ENV.e_CANFD)
            {
                if (_active_Data_Frame != CAN_DATA_FRAME_TYPE.e_FRAME_FD)
                {
                    payload = 8; // If fd frames are not required, then set payload to 8 bytes
                }
                numberOfFrames = Convert.ToByte(datalen / payload);
                if (datalen % payload != 0) numberOfFrames++;
            }
            else
            {
                numberOfFrames = Convert.ToByte(datalen >> 3);
                if (0 != (datalen & 0x07)) //if the length is not multiple of 8, then add one more extra frame for the extra bytes
                {
                    numberOfFrames++;
                }
            }
            return numberOfFrames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="dlc"></param>
        /// <param name="payload"></param>
        private void NotifyMessageSent(uint id, byte[] data, byte dlc, uint payload)
        {
            CANData canData = new CANData
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                //Id = id & 0x1FFFFFFF, // Mask to get the 29-bit ID
                Id = id,
                //Channel = 0,
                Dlc = dlc,
                RxData = false
            };
            if (dlc < 8)
            {
                canData.Data = new byte[dlc];
                Array.Copy(data, canData.Data, dlc);
            }
            else
            {
                canData.Data = new byte[payload];
                Array.Copy(data, canData.Data, payload);
            }
            // Notify the sent CAN data
            RaiseOnMessageSent(canData);
        }

        /// <summary>
        /// receives a CAN/CANFD message from the CAN BUS.
        /// </summary>
        /// <returns></returns>
        public override bool ReceiveMessage()
        {
            if (!_isDriverInitialized)
            {
                return false;
            }
            try
            {
                if (_active_CAN_Env == CAN_ENV.e_CANFD)
                {
                    // Create new object containing received data
                    var receivedRxEvent = new XLClass.XLcanRxEvent();
                    do
                    {
                        _xlStatus = _xlDriverObj.XL_CanReceive(_portHandle, ref receivedRxEvent);
                        if (XLDefine.XL_Status.XL_SUCCESS == _xlStatus)
                        {
                            NotifyCANFDMessageReceived(receivedRxEvent);
                        }
                    } while (_xlStatus == XLDefine.XL_Status.XL_SUCCESS);
                }
                else
                {
                    var xlevent = new XLClass.xl_event();
                    do
                    {
                        _xlStatus = _xlDriverObj.XL_Receive(_portHandle, ref xlevent);
                        if (XLDefine.XL_Status.XL_SUCCESS == _xlStatus)
                        {
                            NotifyCANMessageReceived(xlevent);
                        }
                    } while (_xlStatus == XLDefine.XL_Status.XL_SUCCESS);
                }
            }
            catch (IOException ex)
            {
                if (ex.Message.Length > 0)
                {
                    /* ToDo: Global exeption eintrag schreiben */
                }
            }
            catch (IndexOutOfRangeException exp)
            {
                if (exp.Message.Length > 0)
                {
                    /* ToDo: Global exeption eintrag schreiben */
                }
            }
            return true;
        }

        /// <summary>
        /// decodes the received CAN message and raises the OnMessageReceived event.
        /// </summary>
        /// <param name="xlevent"></param>
        private void NotifyCANMessageReceived(XLClass.xl_event xlevent)
        {
            if (null == xlevent)
            {
                return;
            }
            try
            {
                CANData canData = new CANData
                {
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    //Id = xlevent.tagData.can_Msg.id & 0x1FFFFFFF, // Mask to get the 29-bit ID
                    Id = xlevent.tagData.can_Msg.id,
                    Channel = xlevent.portHandle,
                    Dlc = (byte)xlevent.tagData.can_Msg.dlc,
                    RxData = true,
                    Data = new byte[xlevent.tagData.can_Msg.dlc]
                };
                for (int i = 0; i < canData.Data.Length; i++)
                {
                    canData.Data[i] = xlevent.tagData.can_Msg.data[i];
                }
                // Notify the received CAN data
                RaiseOnMessageReceived(canData);
            }
            catch (Exception ex)
            {
                _lastErrorMessage = "Error processing received CAN data: " + ex.Message;
                return;
            }
        }

        /// <summary>
        /// decodes the received CAN FD message and raises the OnMessageReceived event for CAN FD.
        /// </summary>
        /// <param name="receivedRxEvent"></param>
        private void NotifyCANFDMessageReceived(XLClass.XLcanRxEvent receivedRxEvent)
        {
            if (null == receivedRxEvent)
            {
                return;
            }
            try
            {
                CANData canData = new CANData
                {
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    //Id = receivedRxEvent.tagData.canRxOkMsg.canId & 0x1FFFFFFF, // Mask to get the 29-bit ID
                    Id = receivedRxEvent.tagData.canRxOkMsg.canId,
                    Channel = receivedRxEvent.channelIndex,
                    Dlc = (byte)receivedRxEvent.tagData.canRxOkMsg.dlc,
                    RxData = true,
                    Data = new byte[VectorUtil.GET_CANFD_PAYLOAD(receivedRxEvent.tagData.canRxOkMsg.dlc)]
                };
                for (int i = 0; i < canData.Data.Length; i++)
                {
                    canData.Data[i] = receivedRxEvent.tagData.canRxOkMsg.data[i];
                }
                // Notify the received CAN data
                RaiseOnMessageReceived(canData);
            }
            catch (Exception ex)
            {
                _lastErrorMessage = "Error processing received CANFD data: " + ex.Message;
                return;
            }
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
