using WPFLibBase;

namespace WPFComSetupViewLib.View
{
    /// <summary>
    /// Interaction logic for CommunicationSetupView.xaml
    /// </summary>
    public partial class CommunicationSetupView : WpfUserControl
    {
        public CommunicationSetupView()
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
