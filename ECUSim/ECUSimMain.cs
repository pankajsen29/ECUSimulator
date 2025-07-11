using CommonHwLib;
using MessageDesignerLib;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilityLib;
using WPFHostLib;

namespace ECUSim
{
    public partial class ECUSimMain : Form
    {
        private MessageConfigManager _msgConfigManager;
        private MessageManager _msgManager;
        public ECUSimMain()
        {
            InitializeComponent();
            ComManagerObj.CommunicationSettingsFile = StaticKeys.Communication_Settings_File;
            ComManagerObj.MessageConfigFile = StaticKeys.Messages_Config_File;
            ComManagerObj.ApplyUpdateOfCommunicationSettings += ApplyUpdatedCommunicationSettings;

            _msgConfigManager = new MessageConfigManager();
            _msgManager = new MessageManager();
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
            txtMessage.Text = await _msgConfigManager.LoadMessageDefinition();
            txtMessageConfigFilePath.Text = ComManagerObj.MessageConfigFile;
            await LoadMsgConfig();
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

        /// <summary>
        /// Call from "async" method to another "async" method (with/without return value) 
        /// => just call using "await", it will not block the UI thread and wait for the result
        /// 
        /// Call from "non-async" method to an "async" method (without return value) 
        /// => just call AnAsyncMethod() without using "async", it will not block the UI thread and wait for the result  
        /// 
        /// Call from "non-async" method to an "async" method (with return value) should be done on a separate thread using Task.Run() to avoid blocking the UI thread.
        /// => var returnValue = Task.Run(async () => await AnAsyncMethod()).Result;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnAddMessage_Click(object sender, EventArgs e)
        {
            //var isMsgConfigLoaded = Task.Run(async () => await LoadMsgConfig()).Result;
            var isMsgConfigLoaded = await LoadMsgConfig();
            if (isMsgConfigLoaded)
            {
                await _msgConfigManager.AddToMessagesConfig(txtMessage.Text, ComManagerObj.MessageConfigFile);
                await LoadMsgConfig(); //load the updated message config file
            }
        }

        private void btnLoadMsgConfig_Click(object sender, EventArgs e)
        {
            var isMsgConfigLoaded = Task.Run(async () => await LoadMsgConfig()).Result;
        }

        private async Task<bool> LoadMsgConfig()
        {
            var message = string.Empty;
            var messageConfigFilePath = txtMessageConfigFilePath.Text;
            if (PathValidator.IsValidFilePath(messageConfigFilePath, ".json", ref message))
            {
                ComManagerObj.MessageConfigFile = messageConfigFilePath;
                if (File.Exists(messageConfigFilePath))
                {
                    txtMessageConfigFilePath.BackColor = System.Drawing.Color.LightGreen;
                    txtMessagesConfig.Text = await _msgConfigManager.LoadMessageConfig(ComManagerObj.MessageConfigFile);
                }
                else
                {
                    txtMessageConfigFilePath.BackColor = System.Drawing.Color.Red;
                }
                return true;
            }
            else
            {
                txtMessageConfigFilePath.BackColor = System.Drawing.Color.Red;
                MessageBox.Show(message, "Invalid Message Config File Path!, Enter full path of a json to be created/modified.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }       
    }
}
