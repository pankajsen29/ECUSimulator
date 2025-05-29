using HardwareDriverLayer.WrapperInterface;

namespace HardwareDriverLayer.WrapperFactory
{
    public interface IHwWrapperFactory
    {
        HwWrapperBase GetHwWrapper();
    }
}