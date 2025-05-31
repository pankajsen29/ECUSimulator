using System.Reflection;
using System.Windows.Forms.Integration;
using WPFLibBase;

namespace WPFHostLib
{
    public partial class WpfHostUserControl : UserControl
    {
        private ElementHost _wpfCtrlHost;
        private string _wpfAssemblyName;
        private WpfUserControl? _wpfLib;

        public WpfHostUserControl(string wpfAssemblyName)
        {
            InitializeComponent();
            _wpfAssemblyName = wpfAssemblyName;
            _wpfCtrlHost = new ElementHost();
        }

        private void WpfHostUserControl_Load(object sender, EventArgs e)
        {
            _wpfCtrlHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfCtrlHost);

            _wpfLib = WpfViewFactory.GetWpfViewInstance(_wpfAssemblyName);
            if (_wpfLib != null)
            {
                _wpfLib.OnStart();
                _wpfCtrlHost.Child = _wpfLib;
            }
        }


        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (this.ParentForm != null)
                this.ParentForm.FormClosing += ParentForm_FormClosing;
        }

        private void ParentForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_wpfLib.OnClose())
            {
                this.Hide();
                this.Parent = null;
            }
        }
    }
}
