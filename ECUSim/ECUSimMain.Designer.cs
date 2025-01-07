
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
            this.StatusInfo = new System.Windows.Forms.StatusStrip();
            this.UserInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlpMasterLayout = new System.Windows.Forms.TableLayoutPanel();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabTrace = new System.Windows.Forms.TabPage();
            this.dgCANTrace = new System.Windows.Forms.DataGridView();
            this.tabTxRxSetup = new System.Windows.Forms.TabPage();
            this.txtReqRepJson = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabCommunicationSetup = new System.Windows.Forms.TabPage();
            this.StatusInfo.SuspendLayout();
            this.tlpMasterLayout.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tabTrace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCANTrace)).BeginInit();
            this.tabTxRxSetup.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusInfo
            // 
            this.StatusInfo.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.StatusInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UserInfo});
            this.StatusInfo.Location = new System.Drawing.Point(0, 920);
            this.StatusInfo.Name = "StatusInfo";
            this.StatusInfo.Size = new System.Drawing.Size(1472, 32);
            this.StatusInfo.TabIndex = 1;
            this.StatusInfo.Text = "statusStrip1";
            // 
            // UserInfo
            // 
            this.UserInfo.Name = "UserInfo";
            this.UserInfo.Size = new System.Drawing.Size(179, 25);
            this.UserInfo.Text = "toolStripStatusLabel1";
            // 
            // tlpMasterLayout
            // 
            this.tlpMasterLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpMasterLayout.AutoSize = true;
            this.tlpMasterLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpMasterLayout.ColumnCount = 3;
            this.tlpMasterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tlpMasterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpMasterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpMasterLayout.Controls.Add(this.tcMain, 0, 1);
            this.tlpMasterLayout.Location = new System.Drawing.Point(12, 12);
            this.tlpMasterLayout.Name = "tlpMasterLayout";
            this.tlpMasterLayout.RowCount = 2;
            this.tlpMasterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlpMasterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tlpMasterLayout.Size = new System.Drawing.Size(1441, 881);
            this.tlpMasterLayout.TabIndex = 2;
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpMasterLayout.SetColumnSpan(this.tcMain, 3);
            this.tcMain.Controls.Add(this.tabTrace);
            this.tcMain.Controls.Add(this.tabTxRxSetup);
            this.tcMain.Controls.Add(this.tabCommunicationSetup);
            this.tcMain.Location = new System.Drawing.Point(3, 91);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1435, 787);
            this.tcMain.TabIndex = 0;
            // 
            // tabTrace
            // 
            this.tabTrace.Controls.Add(this.dgCANTrace);
            this.tabTrace.Location = new System.Drawing.Point(4, 34);
            this.tabTrace.Name = "tabTrace";
            this.tabTrace.Padding = new System.Windows.Forms.Padding(3);
            this.tabTrace.Size = new System.Drawing.Size(1427, 749);
            this.tabTrace.TabIndex = 0;
            this.tabTrace.Text = "Trace";
            this.tabTrace.UseVisualStyleBackColor = true;
            // 
            // dgCANTrace
            // 
            this.dgCANTrace.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgCANTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgCANTrace.Location = new System.Drawing.Point(3, 3);
            this.dgCANTrace.Name = "dgCANTrace";
            this.dgCANTrace.RowHeadersWidth = 62;
            this.dgCANTrace.RowTemplate.Height = 33;
            this.dgCANTrace.Size = new System.Drawing.Size(1421, 743);
            this.dgCANTrace.TabIndex = 0;
            // 
            // tabTxRxSetup
            // 
            this.tabTxRxSetup.Controls.Add(this.txtReqRepJson);
            this.tabTxRxSetup.Controls.Add(this.label1);
            this.tabTxRxSetup.Location = new System.Drawing.Point(4, 34);
            this.tabTxRxSetup.Name = "tabTxRxSetup";
            this.tabTxRxSetup.Padding = new System.Windows.Forms.Padding(3);
            this.tabTxRxSetup.Size = new System.Drawing.Size(1427, 749);
            this.tabTxRxSetup.TabIndex = 1;
            this.tabTxRxSetup.Text = "Tx/Rx Setup";
            this.tabTxRxSetup.UseVisualStyleBackColor = true;
            // 
            // txtReqRepJson
            // 
            this.txtReqRepJson.Location = new System.Drawing.Point(6, 53);
            this.txtReqRepJson.Multiline = true;
            this.txtReqRepJson.Name = "txtReqRepJson";
            this.txtReqRepJson.Size = new System.Drawing.Size(1416, 687);
            this.txtReqRepJson.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Request/Reply Settings:";
            // 
            // tabCommunicationSetup
            // 
            this.tabCommunicationSetup.Location = new System.Drawing.Point(4, 34);
            this.tabCommunicationSetup.Name = "tabCommunicationSetup";
            this.tabCommunicationSetup.Padding = new System.Windows.Forms.Padding(3);
            this.tabCommunicationSetup.Size = new System.Drawing.Size(1428, 750);
            this.tabCommunicationSetup.TabIndex = 2;
            this.tabCommunicationSetup.Text = "Com Setup";
            this.tabCommunicationSetup.UseVisualStyleBackColor = true;
            // 
            // ECUSimMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1472, 952);
            this.Controls.Add(this.tlpMasterLayout);
            this.Controls.Add(this.StatusInfo);
            this.Name = "ECUSimMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ECUSim";
            this.StatusInfo.ResumeLayout(false);
            this.StatusInfo.PerformLayout();
            this.tlpMasterLayout.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tabTrace.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgCANTrace)).EndInit();
            this.tabTxRxSetup.ResumeLayout(false);
            this.tabTxRxSetup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip StatusInfo;
        private System.Windows.Forms.ToolStripStatusLabel UserInfo;
        private System.Windows.Forms.TableLayoutPanel tlpMasterLayout;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabTrace;
        private System.Windows.Forms.TabPage tabTxRxSetup;
        private System.Windows.Forms.TabPage tabCommunicationSetup;
        private System.Windows.Forms.DataGridView dgCANTrace;
        private System.Windows.Forms.TextBox txtReqRepJson;
        private System.Windows.Forms.Label label1;
    }
}

