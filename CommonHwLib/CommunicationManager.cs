using System;
using System.Reflection;
using HardwareDriverLayer.HwSettings;
using HardwareDriverLayer.WrapperFactory;
using HardwareDriverLayer.WrapperInterface;

namespace CommonHwLib
{
    public class CommunicationManager
    {
        public bool InitializeCommunicationDriver(CommunicationSettings comSettings)
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
                return hwWrapper.InitializeDriver();
            }
            return false;
        }


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
