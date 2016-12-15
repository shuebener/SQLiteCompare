namespace SQLiteTurbo
{
    partial class CheckUpdatesDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckUpdatesDialog));
            this.pnlContents = new Liron.Windows.Forms.MultiPanel();
            this.pgCheckForUpdates = new Liron.Windows.Forms.MultiPanelPage();
            this.pbrCheckProgress = new System.Windows.Forms.ProgressBar();
            this.lblCheckMessage = new System.Windows.Forms.Label();
            this.pgDownloadUpdates = new Liron.Windows.Forms.MultiPanelPage();
            this.lblDownloadingUpdates = new System.Windows.Forms.Label();
            this.pbrDownloadProgress = new System.Windows.Forms.ProgressBar();
            this.btnDownloadUpdates = new System.Windows.Forms.Button();
            this.btnNewUpdatesDetails = new System.Windows.Forms.Button();
            this.lblNewUpdatesMessage = new System.Windows.Forms.Label();
            this.pgConfirm = new Liron.Windows.Forms.MultiPanelPage();
            this.btnInstallUpdates = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbxCheckUpdates = new System.Windows.Forms.CheckBox();
            this.etchedLine1 = new SQLiteTurbo.EtchedLine();
            this.pnlContents.SuspendLayout();
            this.pgCheckForUpdates.SuspendLayout();
            this.pgDownloadUpdates.SuspendLayout();
            this.pgConfirm.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContents
            // 
            this.pnlContents.Controls.Add(this.pgCheckForUpdates);
            this.pnlContents.Controls.Add(this.pgDownloadUpdates);
            this.pnlContents.Controls.Add(this.pgConfirm);
            this.pnlContents.Location = new System.Drawing.Point(12, 4);
            this.pnlContents.Name = "pnlContents";
            this.pnlContents.Size = new System.Drawing.Size(387, 133);
            this.pnlContents.TabIndex = 0;
            // 
            // pgCheckForUpdates
            // 
            this.pgCheckForUpdates.Controls.Add(this.pbrCheckProgress);
            this.pgCheckForUpdates.Controls.Add(this.lblCheckMessage);
            this.pgCheckForUpdates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgCheckForUpdates.Location = new System.Drawing.Point(0, 0);
            this.pgCheckForUpdates.Name = "pgCheckForUpdates";
            this.pgCheckForUpdates.Size = new System.Drawing.Size(387, 133);
            this.pgCheckForUpdates.TabIndex = 0;
            this.pgCheckForUpdates.TabStop = false;
            this.pgCheckForUpdates.Text = "Checking for updates";
            // 
            // pbrCheckProgress
            // 
            this.pbrCheckProgress.Location = new System.Drawing.Point(6, 75);
            this.pbrCheckProgress.MarqueeAnimationSpeed = 25;
            this.pbrCheckProgress.Name = "pbrCheckProgress";
            this.pbrCheckProgress.Size = new System.Drawing.Size(381, 23);
            this.pbrCheckProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbrCheckProgress.TabIndex = 1;
            // 
            // lblCheckMessage
            // 
            this.lblCheckMessage.AutoSize = true;
            this.lblCheckMessage.Location = new System.Drawing.Point(3, 15);
            this.lblCheckMessage.Name = "lblCheckMessage";
            this.lblCheckMessage.Size = new System.Drawing.Size(337, 13);
            this.lblCheckMessage.TabIndex = 0;
            this.lblCheckMessage.Text = "Please wait while SQLite Compare checks for new software updates...";
            // 
            // pgDownloadUpdates
            // 
            this.pgDownloadUpdates.Controls.Add(this.lblDownloadingUpdates);
            this.pgDownloadUpdates.Controls.Add(this.pbrDownloadProgress);
            this.pgDownloadUpdates.Controls.Add(this.btnDownloadUpdates);
            this.pgDownloadUpdates.Controls.Add(this.btnNewUpdatesDetails);
            this.pgDownloadUpdates.Controls.Add(this.lblNewUpdatesMessage);
            this.pgDownloadUpdates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgDownloadUpdates.Location = new System.Drawing.Point(0, 0);
            this.pgDownloadUpdates.Name = "pgDownloadUpdates";
            this.pgDownloadUpdates.Size = new System.Drawing.Size(387, 133);
            this.pgDownloadUpdates.TabIndex = 1;
            this.pgDownloadUpdates.Text = "Download updates";
            // 
            // lblDownloadingUpdates
            // 
            this.lblDownloadingUpdates.AutoSize = true;
            this.lblDownloadingUpdates.Location = new System.Drawing.Point(3, 73);
            this.lblDownloadingUpdates.Name = "lblDownloadingUpdates";
            this.lblDownloadingUpdates.Size = new System.Drawing.Size(119, 13);
            this.lblDownloadingUpdates.TabIndex = 5;
            this.lblDownloadingUpdates.Text = "Downloading updates...";
            this.lblDownloadingUpdates.Visible = false;
            // 
            // pbrDownloadProgress
            // 
            this.pbrDownloadProgress.Location = new System.Drawing.Point(6, 90);
            this.pbrDownloadProgress.MarqueeAnimationSpeed = 25;
            this.pbrDownloadProgress.Name = "pbrDownloadProgress";
            this.pbrDownloadProgress.Size = new System.Drawing.Size(381, 23);
            this.pbrDownloadProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbrDownloadProgress.TabIndex = 4;
            this.pbrDownloadProgress.Visible = false;
            // 
            // btnDownloadUpdates
            // 
            this.btnDownloadUpdates.Location = new System.Drawing.Point(87, 37);
            this.btnDownloadUpdates.Name = "btnDownloadUpdates";
            this.btnDownloadUpdates.Size = new System.Drawing.Size(78, 23);
            this.btnDownloadUpdates.TabIndex = 3;
            this.btnDownloadUpdates.Text = "Download";
            this.btnDownloadUpdates.UseVisualStyleBackColor = true;
            this.btnDownloadUpdates.Click += new System.EventHandler(this.btnInstallNewUpdates_Click);
            // 
            // btnNewUpdatesDetails
            // 
            this.btnNewUpdatesDetails.Location = new System.Drawing.Point(6, 37);
            this.btnNewUpdatesDetails.Name = "btnNewUpdatesDetails";
            this.btnNewUpdatesDetails.Size = new System.Drawing.Size(75, 23);
            this.btnNewUpdatesDetails.TabIndex = 2;
            this.btnNewUpdatesDetails.Text = "Details...";
            this.btnNewUpdatesDetails.UseVisualStyleBackColor = true;
            this.btnNewUpdatesDetails.Click += new System.EventHandler(this.btnNewUpdatesDetails_Click);
            // 
            // lblNewUpdatesMessage
            // 
            this.lblNewUpdatesMessage.AutoSize = true;
            this.lblNewUpdatesMessage.Location = new System.Drawing.Point(3, 15);
            this.lblNewUpdatesMessage.Name = "lblNewUpdatesMessage";
            this.lblNewUpdatesMessage.Size = new System.Drawing.Size(136, 13);
            this.lblNewUpdatesMessage.TabIndex = 1;
            this.lblNewUpdatesMessage.Text = "New updates are available.";
            // 
            // pgConfirm
            // 
            this.pgConfirm.Controls.Add(this.btnInstallUpdates);
            this.pgConfirm.Controls.Add(this.label1);
            this.pgConfirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgConfirm.Location = new System.Drawing.Point(0, 0);
            this.pgConfirm.Name = "pgConfirm";
            this.pgConfirm.Size = new System.Drawing.Size(387, 133);
            this.pgConfirm.TabIndex = 2;
            this.pgConfirm.Text = "Confirmation";
            // 
            // btnInstallUpdates
            // 
            this.btnInstallUpdates.Location = new System.Drawing.Point(6, 51);
            this.btnInstallUpdates.Name = "btnInstallUpdates";
            this.btnInstallUpdates.Size = new System.Drawing.Size(133, 23);
            this.btnInstallUpdates.TabIndex = 1;
            this.btnInstallUpdates.Text = "Exit && Install Updates";
            this.btnInstallUpdates.UseVisualStyleBackColor = true;
            this.btnInstallUpdates.Click += new System.EventHandler(this.btnInstallUpdates_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(367, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "The software will now exit in order to install the updated version.";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(324, 161);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbxCheckUpdates
            // 
            this.cbxCheckUpdates.AutoSize = true;
            this.cbxCheckUpdates.Checked = true;
            this.cbxCheckUpdates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxCheckUpdates.Location = new System.Drawing.Point(12, 165);
            this.cbxCheckUpdates.Name = "cbxCheckUpdates";
            this.cbxCheckUpdates.Size = new System.Drawing.Size(227, 17);
            this.cbxCheckUpdates.TabIndex = 0;
            this.cbxCheckUpdates.Text = "Automatically check for updates on startup";
            this.cbxCheckUpdates.UseVisualStyleBackColor = true;
            this.cbxCheckUpdates.CheckedChanged += new System.EventHandler(this.cbxCheckUpdates_CheckedChanged);
            // 
            // etchedLine1
            // 
            this.etchedLine1.Location = new System.Drawing.Point(12, 146);
            this.etchedLine1.Name = "etchedLine1";
            this.etchedLine1.Size = new System.Drawing.Size(387, 10);
            this.etchedLine1.TabIndex = 1;
            this.etchedLine1.Text = "etchedLine1";
            // 
            // CheckUpdatesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(411, 193);
            this.Controls.Add(this.cbxCheckUpdates);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.etchedLine1);
            this.Controls.Add(this.pnlContents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckUpdatesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SQLite Compare Updates";
            this.Load += new System.EventHandler(this.CheckUpdatesDialog_Load);
            this.Shown += new System.EventHandler(this.CheckUpdatesDialog_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CheckUpdatesDialog_FormClosing);
            this.pnlContents.ResumeLayout(false);
            this.pgCheckForUpdates.ResumeLayout(false);
            this.pgCheckForUpdates.PerformLayout();
            this.pgDownloadUpdates.ResumeLayout(false);
            this.pgDownloadUpdates.PerformLayout();
            this.pgConfirm.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Liron.Windows.Forms.MultiPanel pnlContents;
        private Liron.Windows.Forms.MultiPanelPage pgCheckForUpdates;
        private EtchedLine etchedLine1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbxCheckUpdates;
        private Liron.Windows.Forms.MultiPanelPage pgDownloadUpdates;
        private System.Windows.Forms.Label lblCheckMessage;
        private System.Windows.Forms.ProgressBar pbrCheckProgress;
        private System.Windows.Forms.Label lblNewUpdatesMessage;
        private System.Windows.Forms.Button btnDownloadUpdates;
        private System.Windows.Forms.Button btnNewUpdatesDetails;
        private System.Windows.Forms.ProgressBar pbrDownloadProgress;
        private System.Windows.Forms.Label lblDownloadingUpdates;
        private Liron.Windows.Forms.MultiPanelPage pgConfirm;
        private System.Windows.Forms.Button btnInstallUpdates;
        private System.Windows.Forms.Label label1;

    }
}