using System.Windows.Controls;

namespace WPFLibBase
{
    public abstract class WpfUserControl : UserControl
    {
        /// <summary>
        /// for passing initialization data to the control
        /// </summary>
        /// <returns></returns>
        public abstract bool OnStart();

        /// <summary>
        /// for cleaning up resources or saving state before the control is closed
        /// </summary>
        /// <returns></returns>
        public abstract bool OnClose();
    }
}