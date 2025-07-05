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
        /// <param name="comSettings">The communication settings to use for driver initialization. This parameter must contain valid configuration
        /// values, including the active CAN hardware interface, baud rate, and other related settings.</param>
        /// <returns><see langword="true"/> if the communication driver is successfully initialized; otherwise, <see
        /// langword="false"/>.</returns>
        public bool InitializeCommunicationDriver(CommunicationSettings comSettings)
        {               
            if (ValidateSettings(comSettings))
            {
                IHwWrapperFactory factory;

                switch (comSettings.ACTIVE_CAN_HW)
                {
                    case CAN_HW_INTERFACE.e_VECTOR_XL:
                        factory = LoadFactory("HardwareDriverLayer.WrapperFactory.VectorXLWrapperFactory");
                        break;

                    case CAN_HW_INTERFACE.e_NOT_DEFINED:
                        factory = null;
                        break;

                    default:
                        factory = null;
                        break;
                }

                if (null != factory)
                {
                    HwWrapperBase hwWrapper = factory.GetHwWrapper();
                    hwWrapper.SetCANEnvironment(comSettings.ACTIVE_CAN_ENV);
                    hwWrapper.SetCommunicationBaudrate(comSettings.ACTIVE_COM_BAUDRATE);
                    hwWrapper.SetCANFDSettings(comSettings.ACTIVE_CANFD_SETTINGS);
                    hwWrapper.SetCANDataFrameType(comSettings.ACTIVE_DATA_FRAME);
                    var initState = hwWrapper.InitializeDriver();
                    if(!initState) LastErrorMessage = hwWrapper.GetLastErrorMessage();
                    return initState;
                }
            }
            return false;
        }

        /// <summary>
        /// todo: first check if there is any change in settings, if not, return true       
        /// </summary>
        /// <param name="comSettings"></param>
        /// <returns></returns>
        private bool ValidateSettings(CommunicationSettings comSettings)
        {
            if (comSettings != null)
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
