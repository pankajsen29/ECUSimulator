using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
