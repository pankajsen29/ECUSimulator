using HardwareDriverLayer.HwSettings;
using HardwareDriverLayer.WrapperFactory;
using HardwareDriverLayer.WrapperInterface;
using HwSettingsLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UtilityLib;

namespace CommonHwLib
{
    public class CommunicationManager
    {
        public string MessageConfigFile { get; set; } = string.Empty;
        public string CommunicationSettingsFile { get; set; } = string.Empty;
        public string GeneralLogFile { get; set; } = string.Empty;
        public string CANLogFile { get; set; } = string.Empty;

        public CommunicationSettings ComSettings { get; set; }

        private static readonly Lazy<CommunicationManager> _sComManager = new Lazy<CommunicationManager>(() => new CommunicationManager());

        private IHwWrapperFactory _hwWrapperFactory;
        private CAN_HW_INTERFACE _lastActiveCANHw = CAN_HW_INTERFACE.e_NOT_DEFINED;
        private string _lastActiveComSettingsJSON = string.Empty;

        private bool _hwInitState = false;

        public Action<CANData> NotifyTraceView { get; set; } = null;

        public List<CANEvent> CANRxEventsList { get; set; } = null;

        private CommunicationManager()
        {
            //canfd settings are set by default
            ComSettings = new CommunicationSettings
            {
                ACTIVE_CAN_HW = CAN_HW_INTERFACE.e_VECTOR_XL,
                ACTIVE_CAN_ENV = CAN_ENV.e_CANFD,
                ACTIVE_DATA_FRAME = CAN_DATA_FRAME_TYPE.e_FRAME_FD,
                ACTIVE_CANFD_SETTINGS = new CANFD_SETTINGS
                {
                    arb_baudrate = 500000,
                    arb_tseg1 = 7,
                    arb_tseg2 = 2,
                    arb_sjw = 2,
                    data_baudrate = 2000000,
                    data_tseg1 = 7,
                    data_tseg2 = 2,
                    data_sjw = 2
                }
            };
            CANRxEventsList = new List<CANEvent>();
        }

        /// <summary>
        /// returns the singleton instance of the CommunicationManager.
        /// </summary>
        /// <returns></returns>
        public static CommunicationManager GetCommunicationManager()
        {
            return _sComManager.Value;
        }

        public Action ApplyUpdateOfCommunicationSettings { get; set; }

        /// <summary>
        /// Gets the message associated with the most recent error that occurred.
        /// </summary>
        public string LastErrorMessage { get; private set; }


        /// <summary>
        /// Notifies the system of updates to communication settings.
        /// </summary>
        /// <remarks>This method should be called whenever communication settings are modified to ensure
        /// the system is aware of the changes. It does not take any parameters and does not return a value.</remarks>
        public void NotifyUpdateOfCommunicationSettings()
        {
            if (ApplyUpdateOfCommunicationSettings != null)
            {
                ApplyUpdateOfCommunicationSettings();
            }
        }

        /// <summary>
        /// This is used to add a new CAN Rx event to the list of CAN Rx events.
        /// This is useful when the application has many different modules or UI for example,
        /// and each module can then add its own CAN Rx events to the list to indicate the CAN IDs it is interested in.
        /// With this approach, there is no need of any generic handler in the application for all the CAN Rx events, 
        /// and each module can handle its own CAN Rx events independently.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="canId"></param>
        /// <returns></returns>
        public CANEvent AddCANRxEvent(string name, uint canId)
        {
            var canEvent = new CANEvent(name, canId);
            CANRxEventsList.Add(canEvent);
            return canEvent;
        }


        /// <summary>
        /// Initializes the communication driver based on the specified settings.
        /// </summary>
        /// <remarks>This method validates the provided communication settings and attempts to initialize
        /// the driver using the specified hardware interface and configuration. If the initialization fails, the last
        /// error message can be retrieved from the <c>LastErrorMessage</c> property.</remarks>        
        /// <returns><see langword="true"/> if the communication driver is successfully initialized; otherwise, <see
        /// langword="false"/>.</returns>
        public bool InitializeCommunicationDriver()
        {
            LastErrorMessage = string.Empty;
            if (ValidateSettings() && IsSettingsChanged())
            {
                //create the new factory object only if the new CAN HW is different than the last active CAN HW
                if (ComSettings.ACTIVE_CAN_HW != _lastActiveCANHw)
                {
                    _lastActiveCANHw = ComSettings.ACTIVE_CAN_HW;

                    switch (ComSettings.ACTIVE_CAN_HW)
                    {
                        case CAN_HW_INTERFACE.e_VECTOR_XL:
                            _hwWrapperFactory = LoadFactory("HardwareDriverLayer.WrapperFactory.VectorXLWrapperFactory");
                            break;

                        case CAN_HW_INTERFACE.e_NOT_DEFINED:
                            _hwWrapperFactory = null;
                            break;

                        default:
                            _hwWrapperFactory = null;
                            break;
                    }
                }

                if (null != _hwWrapperFactory)
                {
                    HwWrapperBase hwWrapper = _hwWrapperFactory.GetHwWrapper();
                    hwWrapper.SetCANEnvironment(ComSettings.ACTIVE_CAN_ENV);
                    hwWrapper.SetCommunicationBaudrate(ComSettings.ACTIVE_COM_BAUDRATE);
                    hwWrapper.SetCANFDSettings(ComSettings.ACTIVE_CANFD_SETTINGS);
                    hwWrapper.SetCANDataFrameType(ComSettings.ACTIVE_DATA_FRAME);
                    hwWrapper.OnMessageReceived += HandleOnMessageReceived;
                    hwWrapper.OnMessageSent += HandleOnMessageSent;

                    _hwInitState = hwWrapper.InitializeDriver();
                    if (_hwInitState)
                    {
                        //from a non-async method, calling the async method on a separate thread and wait for the result
                        _lastActiveComSettingsJSON = Task.Run(async () => (string)await JsonSerializationHelper.SerializeObject<CommunicationSettings>(ComSettings, true)).Result;
                    }
                    else
                    {
                        LastErrorMessage += hwWrapper.GetLastErrorMessage();
                    }
                    return _hwInitState;
                }
            }
            return _hwInitState;
        }

        /// <summary>
        /// few settings are validated before initializing the driver.      
        /// </summary>
        /// <returns></returns>
        private bool ValidateSettings()
        {
            if (ComSettings != null)
            {
                //validate: sjw must be less than or equal to tseg2                
                if (ComSettings.ACTIVE_CANFD_SETTINGS.arb_sjw > ComSettings.ACTIVE_CANFD_SETTINGS.arb_tseg2)
                {
                    ComSettings.ACTIVE_CANFD_SETTINGS.arb_sjw = ComSettings.ACTIVE_CANFD_SETTINGS.arb_tseg2;
                    LastErrorMessage += "CANFD settings warning: arb_sjw must be less than or equal to arb_tseg2. Hence arb_sjw is reset to the value of arb_tseg2. ";
                    return true;
                }
                if (ComSettings.ACTIVE_CANFD_SETTINGS.data_sjw > ComSettings.ACTIVE_CANFD_SETTINGS.data_tseg2)
                {
                    ComSettings.ACTIVE_CANFD_SETTINGS.data_sjw = ComSettings.ACTIVE_CANFD_SETTINGS.data_tseg2;
                    LastErrorMessage += "CANFD settings warning: data_sjw must be less than or equal to data_tseg2. Hence data_sjw is reset to the value of data_tseg2. ";
                    return true;
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// check if there is any change in settings by comparing the old and new json string of the ComSettings object.      
        /// </summary>
        /// <returns></returns>
        private bool IsSettingsChanged()
        {
            if (ComSettings != null)
            {
                //driver initialization for the firt time
                if (string.IsNullOrWhiteSpace(_lastActiveComSettingsJSON))
                {
                    return true;
                }
                //from a non-async method, calling the async method on a separate thread and wait for the result
                var newComSettingsJSON = Task.Run(async () => (string)await JsonSerializationHelper.SerializeObject<CommunicationSettings>(ComSettings, true)).Result;
                if (!string.Equals(_lastActiveComSettingsJSON, newComSettingsJSON, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Loads an instance of a hardware wrapper factory based on the specified class name.
        /// </summary>
        /// <remarks>The method attempts to locate the type within the assembly containing <see
        /// cref="IHwWrapperFactory"/>  and create an instance of it using <see cref="Activator.CreateInstance(Type)"/>.
        /// If the type cannot be  found or the instance cannot be created, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="className">The fully qualified name of the class to instantiate. This must represent a type that implements  <see
        /// cref="IHwWrapperFactory"/>.</param>
        /// <returns>An instance of <see cref="IHwWrapperFactory"/> if the specified class name corresponds to a valid type  and
        /// the instance is successfully created; otherwise, <see langword="null"/>.</returns>
        private IHwWrapperFactory LoadFactory(string className)
        {
            Assembly? assem = typeof(IHwWrapperFactory).Assembly;
            if (assem != null)
            {
                Type? type = assem.GetType(className);
                if (type != null)
                {
                    return Activator.CreateInstance(type) as IHwWrapperFactory;
                }
            }
            return null;
        }

        public bool SendMessage(CANData candata)
        {
            if (_hwWrapperFactory != null)
            {
                HwWrapperBase hwWrapper = _hwWrapperFactory.GetHwWrapper();
                if (hwWrapper != null)
                {
                    return hwWrapper.SendMessage(candata);
                }
            }
            LastErrorMessage = "Hardware wrapper factory is not initialized or the wrapper is not available.";
            return false;
        }

        public bool ReceiveMessage()
        {
            if (_hwWrapperFactory != null)
            {
                HwWrapperBase hwWrapper = _hwWrapperFactory.GetHwWrapper();
                if (hwWrapper != null)
                {
                    return hwWrapper.ReceiveMessage();
                }
            }
            LastErrorMessage = "Hardware wrapper factory is not initialized or the wrapper is not available.";
            return false;
        }

        private void HandleOnMessageReceived(CANData data)
        {
            if (CANRxEventsList != null && CANRxEventsList.Count > 0)
            {
                foreach (var canEvent in CANRxEventsList)
                {
                    if (canEvent.CanId == data.Id) // it ensures only the relevant rx ids are considered, not all the received data on the bus 
                    {
                        canEvent.RaiseOnMessageReceived(data);
                        if (NotifyTraceView != null)
                        {
                            NotifyTraceView(data); //notify to the trace view
                        }
                    }
                }
            }            
        }

        private void HandleOnMessageSent(CANData data)
        {
            if (NotifyTraceView != null)
            {
                NotifyTraceView(data); //notify to the trace view
            }
        }
    }
}
