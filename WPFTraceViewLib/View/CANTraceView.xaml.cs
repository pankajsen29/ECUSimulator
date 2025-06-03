using WPFLibBase;

namespace WPFTraceViewLib.View
{
    /// <summary>
    /// Interaction logic for CANTraceView.xaml
    /// </summary>
    public partial class CANTraceView : WpfUserControl
    {
        public CANTraceView()
        {
            InitializeComponent();
        }

        public override bool OnStart()
        {
            //get the initialization
            return true;
        }

        public override bool OnClose()
        {
            //cleanup logic here, ask for save for example
            return true;
        }
    }
}
