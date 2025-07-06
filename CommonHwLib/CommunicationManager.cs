using System;
using System.Reflection;
using HardwareDriverLayer.HwSettings;
using HardwareDriverLayer.WrapperFactory;
using HardwareDriverLayer.WrapperInterface;

namespace CommonHwLib
{
    public class CommunicationManager
    {
        public string MessageConfigFile { get; set; } = string.Empty;
        public string CommunicationSettingsFile { get; set; } = string.Empty;
        public CommunicationSettings ComSettings { get; set; }

        private static readonly Lazy<CommunicationManager> _sComManager = new Lazy<CommunicationManager>(() => new CommunicationManager());

        private IHwWrapperFactory _hwWrapperFactory;
        private CAN_HW_INTERFACE _lastActiveCANHw = CAN_HW_INTERFACE.e_NOT_DEFINED;

        private bool _hwInitState = false;

        private CommunicationManager() 
        {
            //canfd settings are set by default
            ComSettings = new CommunicationSettings
            {
                ACTIVE_CAN_HW = CAN_HW_INTERFACE.e_VECTOR_XL,
                ACTIVE_CAN_ENV = CAN_ENV.e_CANFD,
                ACTIVE_DATA_FRAME = CAN_DATA_FRAME_TYPE.e_FRAME_STD,
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
        /// Initializes the communication driver based on the specified settings.
        /// </summary>
        /// <remarks>This method validates the provided communication settings and attempts to initialize
        /// the driver using the specified hardware interface and configuration. If the initialization fails, the last
        /// error message can be retrieved from the <c>LastErrorMessage</c> property.</remarks>        
        /// <returns><see langword="true"/> if the communication driver is successfully initialized; otherwise, <see
        /// langword="false"/>.</returns>
        public bool InitializeCommunicationDriver()
        {               
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
                    _hwInitState = hwWrapper.InitializeDriver();
                    if(!_hwInitState) LastErrorMessage = hwWrapper.GetLastErrorMessage();
                    return _hwInitState;
                }
            }
            return _hwInitState;
        }

        /// <summary>
        /// todo: first check each values are valid, if not, return false       
        /// </summary>
        /// <returns></returns>
        private bool ValidateSettings()
        {
            if (ComSettings != null)
            {
                //check each value if in range
                return true;
            }
            return false;
        }


        /// <summary>
        /// todo: first check if there is any change in settings, if not, return true       
        /// </summary>
        /// <returns></returns>
        private bool IsSettingsChanged()
        {
            if (ComSettings != null)
            {
                //check each setting if there is any change
                return true;
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
    }
}
