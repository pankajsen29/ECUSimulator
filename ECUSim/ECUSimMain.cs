using CommonHwLib;
using HwSettingsLib;
using MessageDesignerLib;
using MessageProcessorLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilityLib;
using WPFHostLib;

namespace ECUSim
{
    public partial class ECUSimMain : Form
    {
        private MessageConfigManager _msgConfigManager;
        private MessageProcessor _msgProcessor;

        private CancellationTokenSource _ctsCanSending;
        private CancellationTokenSource _ctsCanRxPooling;
        public ECUSimMain()
        {
            InitializeComponent();
            ComManagerObj.CommunicationSettingsFile = StaticKeys.Communication_Settings_File;
            ComManagerObj.MessageConfigFile = StaticKeys.Messages_Config_File;
            ComManagerObj.ApplyUpdateOfCommunicationSettings += ApplyUpdatedCommunicationSettings;

            _msgConfigManager = new MessageConfigManager();
            _msgProcessor = new MessageProcessor(ComManagerObj);
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

        private async void btnStartTransmission_Click(object sender, EventArgs e)
        {
            var isMsgConfigLoaded = await _msgProcessor.LoadMessageConfig(ComManagerObj.MessageConfigFile);
            if (isMsgConfigLoaded)
            {
                if (btnStartTransmission.Text.ToUpper().Equals("START"))
                {
                    btnStartTransmission.Text = "STOP";
                    StartCanSendingLoop();

                    StartCanRxPoolingLoop();
                }
                else if (btnStartTransmission.Text.ToUpper().Equals("STOP"))
                {
                    StopCanSendingLoop();
                    StopCanRxPoolingLoop();
                }
            }
            else
            {                 
                MessageBox.Show("Error loading message configuration file: " + _msgProcessor.LastErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// sending of longer CAN data is possible, i.e., in terms of multiple CAN/CANFD frames.
        /// </summary>
        public void StartCanSendingLoop()
        {
            _ctsCanSending = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    var token = _ctsCanSending.Token;
                    const long intervalMicroseconds = 500;
                    HighPrecisionTimer timer = new HighPrecisionTimer();

                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        timer.Reset();

                        SendCanMessage();

                        // Hybrid wait to prevent high CPU usage
                        HighPrecisionTimer.WaitMicroseconds(intervalMicroseconds);
                    }
                }
                catch (OperationCanceledException ocExp)
                {
                    //log or clean up
                    this.Invoke(() =>
                    {
                        btnStartTransmission.Text = "START";
                    });
                }
            });
        }

        private void SendCanMessage()
        {
            //Debug.WriteLine("CAN Message Sent at " + DateTime.Now.ToString("HH:mm:ss.ffffff"));

            if (null != _msgProcessor)
            {
                while (_msgProcessor.CANTxDataQueue.Count > 0)
                {
                    CANData? canData;
                    if (_msgProcessor.CANTxDataQueue.TryDequeue(out canData) && canData != null)
                    {
                        ComManagerObj.SendMessage(canData);
                    }
                }
            }           
        }


        /// <summary>
        /// currently receiving of only single CAN/CANFD frame is supported.
        /// </summary>
        public void StartCanRxPoolingLoop()
        {
            _ctsCanRxPooling = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    var token = _ctsCanRxPooling.Token;
                    const long intervalMicroseconds = 100;
                    HighPrecisionTimer timer = new HighPrecisionTimer();

                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        timer.Reset();

                        ReceiveCanMessage();

                        // Hybrid wait to prevent high CPU usage
                        HighPrecisionTimer.WaitMicroseconds(intervalMicroseconds);
                    }
                }
                catch (OperationCanceledException ocExp)
                {
                    //log or clean up
                    this.Invoke(() =>
                    {
                        btnStartTransmission.Text = "START";
                    });
                }
            });
        }

        private void ReceiveCanMessage()
        {            
            if (ComManagerObj.ReceiveMessage())
            {
                //Debug.WriteLine("CAN Message Received at " + DateTime.Now.ToString("HH:mm:ss.ffffff"));
                _msgProcessor.HandleReceivedCANMessage();
            }
        }

        /// <summary>
        /// stops the CAN Rx pooling loop by cancelling the token source.
        /// Note: can be called from the UI thread or any other thread. CancellationToken uses thread-safe, lock-free internal signaling.
        /// It internally sets token.IsCancellationRequested flag, which is a non-blocking property that can be polled from any thread.
        /// The flag is set in memory and is immediately visible to all threads (due to .NET's memory model and volatile field usage).
        /// </summary>
        public void StopCanRxPoolingLoop()
        {
            _ctsCanRxPooling?.Cancel();
        }

        /// <summary>
        /// stops the CAN sending loop by cancelling the token source.
        /// Note: can be called from the UI thread or any other thread. CancellationToken uses thread-safe, lock-free internal signaling.
        /// It internally sets token.IsCancellationRequested flag, which is a non-blocking property that can be polled from any thread.
        /// The flag is set in memory and is immediately visible to all threads (due to .NET's memory model and volatile field usage).
        /// </summary>
        public void StopCanSendingLoop()
        {
            _ctsCanSending?.Cancel();
        }
    }
}
