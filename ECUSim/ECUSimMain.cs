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
        private static readonly Lazy<CommunicationManager> _sComManager = new Lazy<CommunicationManager>(() => new CommunicationManager());

        public ECUSimMain()
        {
            InitializeComponent();
            GetCommunicationManager().ApplyUpdateOfCommunicationSettings += ApplyUpdatedCommunicationSettings;
        }

        private static CommunicationManager GetCommunicationManager()
        {
            return _sComManager.Value;
        }

        private void ECUSimMain_Load(object sender, EventArgs e)
        {
            LoadTraceViewTab();            
            LoadRequestResponseTab();
            LoadCommunicationSettingsTab();
        }

        private void LoadCommunicationSettingsTab()
        {
            var communicationSettingsUserControl = new WpfHostUserControl("WPFComSettingsViewLib", GetCommunicationManager());
            tcMain.TabPages[2].Controls.Add(communicationSettingsUserControl);
            communicationSettingsUserControl.Dock = DockStyle.Fill;
        }

        private void LoadTraceViewTab()
        {
            var traceViewUserControl = new WpfHostUserControl("WPFTraceViewLib", GetCommunicationManager());
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

        private void ApplyUpdatedCommunicationSettings()
        {
            InitCANDriver_Click(null, EventArgs.Empty);
        }

        /// <summary>
        /// check for comSettings json file in Documents folder
        /// if exists: 
        ///     deserialize it and load it
        ///     and initialize active comSettings object with the loaded settings
        ///     and then call GetCommunicationManager().InitializeCommunicationDriver(comSettings);
        /// 
        /// if doesn't exist:
        ///     show Yes/No Messagebox to ask the user if the driver needs to be initialized with default settings
        ///         if Yes:
        ///             then do the below to initialize active comSettings object with default values
        ///             and then call GetCommunicationManager().InitializeCommunicationDriver(comSettings);
        ///         else abort initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitCANDriver_Click(object sender, EventArgs e)
        {
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
            var initCANStatus = GetCommunicationManager().InitializeCommunicationDriver(comSettings);
            InitCANDriver.BackColor = initCANStatus ? System.Drawing.Color.LightGreen : System.Drawing.Color.Red;
            if (!initCANStatus)
            {
                MessageBox.Show($"Error initializing CAN driver: {GetCommunicationManager().LastErrorMessage}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
