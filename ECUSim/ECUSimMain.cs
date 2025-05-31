using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonHwLib;
using HardwareDriverLayer.HwSettings;
using MessageDesignerLib;
using UtilityLib;
using WPFHostLib;
using Message = MessageDesignerLib.Message;

namespace ECUSim
{
    public partial class ECUSimMain : Form
    {
        public ECUSimMain()
        {
            InitializeComponent();
        }

        private void ECUSimMain_Load(object sender, EventArgs e)
        {
            LoadTraceViewTab();
            LoadCommunicationSetupTab();
            LoadRequestResponseTab();
        }

        private void LoadCommunicationSetupTab()
        {
            var communicationSetupUserControl = new WpfHostUserControl("WPFComSetupViewLib");
            tcMain.TabPages[2].Controls.Add(communicationSetupUserControl);
            communicationSetupUserControl.Dock = DockStyle.Fill;
        }

        private void LoadTraceViewTab()
        {
            var traceViewUserControl = new WpfHostUserControl("WPFTraceViewLib");
            tcMain.TabPages[0].Controls.Add(traceViewUserControl);
            traceViewUserControl.Dock = DockStyle.Fill;
        }

        private async void LoadRequestResponseTab()
        {
            await LoadMessageDefinition();
            await LoadMessagesConfig();
        }
        private async Task LoadMessageDefinition()
        {
            //request json
            var pattern = new Pattern();
            pattern.Index = "";
            pattern.RangeStart = "";
            pattern.RangeEnd = "";

            var dataPatterns = new DataPatterns();
            dataPatterns.Pattern = new Pattern[] { pattern };

            var reqhexdata = new RequestHexData();
            reqhexdata.IsPatternBased = "false";
            reqhexdata.DataString = "41 81 C0 11 00 00 00 00";
            reqhexdata.DataPatterns = dataPatterns;

            var request = new Request();
            request.Id = "0x31";
            request.Payload = "8";
            request.IsCanFdFrame = "false";
            request.RequestHexData = reqhexdata;


            //response json string
            var substitution = new Substitution();
            substitution.SourceDataIndexFromRequest = "";
            substitution.DestinationDataIndexToResponse = "";

            var dataSubstitution = new DataSubstitutions();
            dataSubstitution.Substitution = new Substitution[] { substitution };

            var reshexdata = new ResponseHexData();
            reshexdata.IsPatternBased = "false";
            reshexdata.DataString = "41 81 C0 11 00 00 00 00";
            reshexdata.DataSubstitutions = dataSubstitution;

            var response = new Response();
            response.Id = "0x30";
            response.Payload = "8";
            response.IsCanFdFrame = "false";
            response.ResponseHexData = reshexdata;

            //message json string
            var message = new Message();
            message.Name = "cmd_1";
            message.Request = request;
            message.Response = response;

            txtMessage.Text = (string)(await JsonSerializationHelper.SerializeObject<Message>(message, true));
        }

        private async Task LoadMessagesConfig()
        {

        }
        private void btnAddMessage_Click(object sender, EventArgs e)
        {

        }        

        /// <summary>
        /// temp function, to be reworked later
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitCANDriver_Click(object sender, EventArgs e)
        {
            var comManager = new CommunicationManager();

            //CommunicationSettings comSettings = new CommunicationSettings
            //{
            //    ACTIVE_CAN_HW = CAN_HW_INTERFACE.e_VECTOR_XL,
            //    ACTIVE_CAN_ENV = CAN_ENV.e_CAN,
            //    ACTIVE_COM_BAUDRATE = 500000
            //};

            CommunicationSettings comSettings = new CommunicationSettings
            {
                ACTIVE_CAN_HW = CAN_HW_INTERFACE.e_VECTOR_XL,
                ACTIVE_CAN_ENV = CAN_ENV.e_CANFD,
                ACTIVE_DATA_FRAME = CAN_DATA_FRAME_TYPE.e_FRAME_STD,
                ACTIVE_CANFD_SETTINGS = new CANFD_SETTINGS
                {
                    arb_baudrate = 500000,
                    arb_tseg1 = 7,
                    arb_tseg2 = 2,
                    arb_sjw = 2,
                    data_baudrate = 2000000,
                    data_tseg1 = 7,
                    data_tseg2 = 2,
                    data_sjw = 2
                }
            };

            MessageBox.Show(comManager.InitializeCommunicationDriver(comSettings).ToString());
        }
    }
}
