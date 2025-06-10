using CommonHwLib;
using WPFComSettingsViewLib.ViewModel;
using WPFLibBase;

namespace WPFComSettingsViewLib.View
{
    /// <summary>
    /// Interaction logic for CommunicationSettingsView.xaml
    /// </summary>
    public partial class CommunicationSettingsView : WpfUserControl
    {
        private CommunicationSettingsViewModel _comSettingsVM;

        public CommunicationSettingsView()
        {
            InitializeComponent();
            this.DataContext = _comSettingsVM = new CommunicationSettingsViewModel();
        }

        public override bool OnStart(CommunicationManager comManager)
        {
            _comSettingsVM.OnStart(comManager);
            return true;
        }

        public override bool OnClose()
        {
            //cleanup logic here, ask for save for example
            return true;
        }
    }
}
