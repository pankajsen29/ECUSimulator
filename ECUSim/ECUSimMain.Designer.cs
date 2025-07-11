
namespace ECUSim
{
    partial class ECUSimMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            StatusInfo = new System.Windows.Forms.StatusStrip();
            UserInfo = new System.Windows.Forms.ToolStripStatusLabel();
            tlpMasterLayout = new System.Windows.Forms.TableLayoutPanel();
            tcMain = new System.Windows.Forms.TabControl();
            tabTrace = new System.Windows.Forms.TabPage();
            tabRequestReplySetup = new System.Windows.Forms.TabPage();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            txtMessage = new System.Windows.Forms.TextBox();
            txtMessagesConfig = new System.Windows.Forms.TextBox();
            btnAddMessage = new System.Windows.Forms.Button();
            btnLoadMsgConfig = new System.Windows.Forms.Button();
            txtMessageConfigFilePath = new System.Windows.Forms.TextBox();
            tabCommunicationSettings = new System.Windows.Forms.TabPage();
            InitCANDriver = new System.Windows.Forms.Button();
            StatusInfo.SuspendLayout();
            tlpMasterLayout.SuspendLayout();
            tcMain.SuspendLayout();
            tabRequestReplySetup.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // StatusInfo
            // 
            StatusInfo.ImageScalingSize = new System.Drawing.Size(24, 24);
            StatusInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { UserInfo });
            StatusInfo.Location = new System.Drawing.Point(0, 582);
            StatusInfo.Name = "StatusInfo";
            StatusInfo.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            StatusInfo.Size = new System.Drawing.Size(1141, 22);
            StatusInfo.TabIndex = 1;
            StatusInfo.Text = "statusStrip1";
            // 
            // UserInfo
            // 
            UserInfo.Name = "UserInfo";
            UserInfo.Size = new System.Drawing.Size(118, 17);
            UserInfo.Text = "toolStripStatusLabel1";
            // 
            // tlpMasterLayout
            // 
            tlpMasterLayout.AutoSize = true;
            tlpMasterLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tlpMasterLayout.ColumnCount = 3;
            tlpMasterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            tlpMasterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tlpMasterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tlpMasterLayout.Controls.Add(tcMain, 0, 1);
            tlpMasterLayout.Controls.Add(InitCANDriver, 2, 0);
            tlpMasterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            tlpMasterLayout.Location = new System.Drawing.Point(0, 0);
            tlpMasterLayout.Margin = new System.Windows.Forms.Padding(2);
            tlpMasterLayout.Name = "tlpMasterLayout";
            tlpMasterLayout.RowCount = 2;
            tlpMasterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tlpMasterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            tlpMasterLayout.Size = new System.Drawing.Size(1141, 582);
            tlpMasterLayout.TabIndex = 2;
            // 
            // tcMain
            // 
            tcMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tlpMasterLayout.SetColumnSpan(tcMain, 3);
            tcMain.Controls.Add(tabTrace);
            tcMain.Controls.Add(tabRequestReplySetup);
            tcMain.Controls.Add(tabCommunicationSettings);
            tcMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            tcMain.Location = new System.Drawing.Point(2, 60);
            tcMain.Margin = new System.Windows.Forms.Padding(2);
            tcMain.Name = "tcMain";
            tcMain.SelectedIndex = 0;
            tcMain.Size = new System.Drawing.Size(1137, 520);
            tcMain.TabIndex = 0;
            // 
            // tabTrace
            // 
            tabTrace.Location = new System.Drawing.Point(4, 24);
            tabTrace.Margin = new System.Windows.Forms.Padding(2);
            tabTrace.Name = "tabTrace";
            tabTrace.Padding = new System.Windows.Forms.Padding(2);
            tabTrace.Size = new System.Drawing.Size(1129, 492);
            tabTrace.TabIndex = 0;
            tabTrace.Text = "Trace";
            tabTrace.UseVisualStyleBackColor = true;
            // 
            // tabRequestReplySetup
            // 
            tabRequestReplySetup.Controls.Add(tableLayoutPanel1);
            tabRequestReplySetup.Location = new System.Drawing.Point(4, 24);
            tabRequestReplySetup.Margin = new System.Windows.Forms.Padding(2);
            tabRequestReplySetup.Name = "tabRequestReplySetup";
            tabRequestReplySetup.Padding = new System.Windows.Forms.Padding(2);
            tabRequestReplySetup.Size = new System.Drawing.Size(1129, 492);
            tabRequestReplySetup.TabIndex = 1;
            tabRequestReplySetup.Text = "Request/Response Setup";
            tabRequestReplySetup.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            tableLayoutPanel1.Controls.Add(txtMessage, 0, 1);
            tableLayoutPanel1.Controls.Add(txtMessagesConfig, 1, 1);
            tableLayoutPanel1.Controls.Add(btnAddMessage, 0, 0);
            tableLayoutPanel1.Controls.Add(btnLoadMsgConfig, 2, 0);
            tableLayoutPanel1.Controls.Add(txtMessageConfigFilePath, 1, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93F));
            tableLayoutPanel1.Size = new System.Drawing.Size(1125, 488);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // txtMessage
            // 
            txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            txtMessage.ForeColor = System.Drawing.SystemColors.Highlight;
            txtMessage.Location = new System.Drawing.Point(2, 36);
            txtMessage.Margin = new System.Windows.Forms.Padding(2);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtMessage.Size = new System.Drawing.Size(502, 450);
            txtMessage.TabIndex = 0;
            txtMessage.WordWrap = false;
            // 
            // txtMessagesConfig
            // 
            tableLayoutPanel1.SetColumnSpan(txtMessagesConfig, 2);
            txtMessagesConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            txtMessagesConfig.Location = new System.Drawing.Point(508, 36);
            txtMessagesConfig.Margin = new System.Windows.Forms.Padding(2);
            txtMessagesConfig.Multiline = true;
            txtMessagesConfig.Name = "txtMessagesConfig";
            txtMessagesConfig.ReadOnly = true;
            txtMessagesConfig.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtMessagesConfig.Size = new System.Drawing.Size(615, 450);
            txtMessagesConfig.TabIndex = 1;
            txtMessagesConfig.WordWrap = false;
            // 
            // btnAddMessage
            // 
            btnAddMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnAddMessage.Location = new System.Drawing.Point(328, 2);
            btnAddMessage.Margin = new System.Windows.Forms.Padding(2);
            btnAddMessage.Name = "btnAddMessage";
            btnAddMessage.Size = new System.Drawing.Size(176, 30);
            btnAddMessage.TabIndex = 2;
            btnAddMessage.Text = "Add the below message >>";
            btnAddMessage.UseVisualStyleBackColor = true;
            btnAddMessage.Click += btnAddMessage_Click;
            // 
            // btnLoadMsgConfig
            // 
            btnLoadMsgConfig.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btnLoadMsgConfig.Location = new System.Drawing.Point(1070, 2);
            btnLoadMsgConfig.Margin = new System.Windows.Forms.Padding(2);
            btnLoadMsgConfig.Name = "btnLoadMsgConfig";
            btnLoadMsgConfig.Size = new System.Drawing.Size(53, 30);
            btnLoadMsgConfig.TabIndex = 3;
            btnLoadMsgConfig.Text = "Load";
            btnLoadMsgConfig.UseVisualStyleBackColor = true;
            btnLoadMsgConfig.Click += btnLoadMsgConfig_Click;
            // 
            // txtMessageConfigFilePath
            // 
            txtMessageConfigFilePath.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtMessageConfigFilePath.Location = new System.Drawing.Point(508, 5);
            txtMessageConfigFilePath.Margin = new System.Windows.Forms.Padding(2);
            txtMessageConfigFilePath.Name = "txtMessageConfigFilePath";
            txtMessageConfigFilePath.Size = new System.Drawing.Size(558, 23);
            txtMessageConfigFilePath.TabIndex = 4;
            // 
            // tabCommunicationSettings
            // 
            tabCommunicationSettings.Location = new System.Drawing.Point(4, 24);
            tabCommunicationSettings.Margin = new System.Windows.Forms.Padding(2);
            tabCommunicationSettings.Name = "tabCommunicationSettings";
            tabCommunicationSettings.Padding = new System.Windows.Forms.Padding(2);
            tabCommunicationSettings.Size = new System.Drawing.Size(1129, 492);
            tabCommunicationSettings.TabIndex = 2;
            tabCommunicationSettings.Text = "Communication Settings";
            tabCommunicationSettings.UseVisualStyleBackColor = true;
            // 
            // InitCANDriver
            // 
            InitCANDriver.BackColor = System.Drawing.Color.Red;
            InitCANDriver.Dock = System.Windows.Forms.DockStyle.Fill;
            InitCANDriver.Location = new System.Drawing.Point(972, 3);
            InitCANDriver.Name = "InitCANDriver";
            InitCANDriver.Size = new System.Drawing.Size(166, 52);
            InitCANDriver.TabIndex = 1;
            InitCANDriver.Text = "Init/ReInit CAN";
            InitCANDriver.UseVisualStyleBackColor = false;
            InitCANDriver.Click += InitCANDriver_Click;
            // 
            // ECUSimMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1141, 604);
            Controls.Add(tlpMasterLayout);
            Controls.Add(StatusInfo);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "ECUSimMain";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ECU Simulator";
            Load += ECUSimMain_Load;
            StatusInfo.ResumeLayout(false);
            StatusInfo.PerformLayout();
            tlpMasterLayout.ResumeLayout(false);
            tcMain.ResumeLayout(false);
            tabRequestReplySetup.ResumeLayout(false);
            tabRequestReplySetup.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.StatusStrip StatusInfo;
        private System.Windows.Forms.ToolStripStatusLabel UserInfo;
        private System.Windows.Forms.TableLayoutPanel tlpMasterLayout;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabTrace;
        private System.Windows.Forms.TabPage tabRequestReplySetup;
        private System.Windows.Forms.TabPage tabCommunicationSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.TextBox txtMessagesConfig;
        private System.Windows.Forms.Button btnAddMessage;
        private System.Windows.Forms.Button InitCANDriver;
        private System.Windows.Forms.Button btnLoadMsgConfig;
        private System.Windows.Forms.TextBox txtMessageConfigFilePath;
    }
}

