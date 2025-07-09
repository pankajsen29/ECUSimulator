using CommonHwLib;
using WPFLibBase;
using WPFTraceViewLib.ViewModel;

namespace WPFTraceViewLib.View
{
    /// <summary>
    /// Interaction logic for CANTraceView.xaml
    /// </summary>
    public partial class CANTraceView : WpfUserControl
    {
        private CANTraceViewModel _CANTraceVM;
        public CANTraceView()
        {
            InitializeComponent();
            this.DataContext = _CANTraceVM = new CANTraceViewModel();
        }

        public override bool OnStart(CommunicationManager comManager)
        {
            _CANTraceVM.OnStart(comManager);
            return true;
        }

        public override bool OnClose()
        {
            //cleanup logic here, ask for save for example
            return true;
        }
    }
}
