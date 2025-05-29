using HardwareDriverLayer.WrapperInterface;
using HardwareDriverLayer.Wrapper;

namespace HardwareDriverLayer.WrapperFactory
{
    internal class VectorXLWrapperFactory : IHwWrapperFactory
    {
        public VectorXLWrapperFactory()
        {            
        }
        public HwWrapperBase GetHwWrapper()
        {
            return new VectorXL();
        }
    }
}
