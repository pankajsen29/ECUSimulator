using CommonHwLib;
using MessageDesignerLib;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            ComManagerObj.CommunicationSettingsFile = StaticKeys.Communication_Settings_File;
            ComManagerObj.MessageConfigFile = StaticKeys.Messages_Config_File;
            ComManagerObj.ApplyUpdateOfCommunicationSettings += ApplyUpdatedCommunicationSettings;
        }
        private CommunicationManager ComManagerObj
        {
            get { return CommunicationManager.GetCommunicationManager(); }
        }

        private void ECUSimMain_Load(object sender, EventArgs e)
        {
            DisplayUserRights();
            LoadTraceViewTab();            
            LoadRequestResponseTab();
            ApplyUpdatedCommunicationSettings();
            LoadCommunicationSettingsTab();
        }

        /// <summary>
        /// displays the current user name and if the user is an administrator.
        /// </summary>
        private void DisplayUserRights()
        {
            WindowsIdentity userWinIdentity = WindowsIdentity.GetCurrent();
            var userWinPrincipal = new WindowsPrincipal(userWinIdentity);

            UserInfo.Text = Environment.UserName;
            if (userWinPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                UserInfo.Text += @" ADMIN";
            }
        }

        private void LoadCommunicationSettingsTab()
        {
            var communicationSettingsUserControl = new WpfHostUserControl("WPFComSettingsViewLib", ComManagerObj);
            tcMain.TabPages[2].Controls.Add(communicationSettingsUserControl);
            communicationSettingsUserControl.Dock = DockStyle.Fill;
        }

        private void LoadTraceViewTab()
        {
            var traceViewUserControl = new WpfHostUserControl("WPFTraceViewLib", ComManagerObj);
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
        ///     and then call GetCommunicationManager().InitializeCommunicationDriver();
        /// 
        /// if doesn't exist:
        ///     show Yes/No Messagebox to ask the user if the driver needs to be initialized with default settings
        ///         if Yes:
        ///             then do the below to initialize active comSettings object with default values
        ///             and then call GetCommunicationManager().InitializeCommunicationDriver();
        ///         else abort initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InitCANDriver_Click(object sender, EventArgs e)
        {
            //try to load the Settings from comSettings.json file in public Documents folder.
            //if file exists, deserialize it and load it
            if (File.Exists(ComManagerObj.CommunicationSettingsFile))
            {
                ComManagerObj.ComSettings.Dispose();
                ComManagerObj.ComSettings = null;
                ComManagerObj.ComSettings = (CommunicationSettings)await JsonSerializationHelper.Deserialize<CommunicationSettings>(ComManagerObj.CommunicationSettingsFile);
            }

            var initCANStatus = ComManagerObj.InitializeCommunicationDriver();
            InitCANDriver.BackColor = initCANStatus ? System.Drawing.Color.LightGreen : System.Drawing.Color.Red;
            if (!initCANStatus)
            {
                MessageBox.Show($"Error initializing CAN driver: {ComManagerObj.LastErrorMessage}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!string.IsNullOrWhiteSpace(ComManagerObj.LastErrorMessage))
            {
                MessageBox.Show(ComManagerObj.LastErrorMessage, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
