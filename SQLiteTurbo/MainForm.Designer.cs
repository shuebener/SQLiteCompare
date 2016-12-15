namespace SQLiteTurbo
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mniCompare = new System.Windows.Forms.ToolStripMenuItem();
            this.mniCloseComparison = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mniGenerateChangeScriptLeftToRight = new System.Windows.Forms.ToolStripMenuItem();
            this.mniGenerateChangeScriptRightToLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.mniExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mniCopyFromLeftDB = new System.Windows.Forms.ToolStripMenuItem();
            this.mniCopyFromRightDB = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mniEditSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mniGotoNextDifference = new System.Windows.Forms.ToolStripMenuItem();
            this.mniGotoPreviousDifference = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mniCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.mniReport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.mniAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnCompare = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnNextDiff = new System.Windows.Forms.ToolStripButton();
            this.btnPreviousDiff = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCopyFromLeftDB = new System.Windows.Forms.ToolStripButton();
            this.btnCopyFromRightDB = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnEditSelectedDifference = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExportDataDifferences = new System.Windows.Forms.ToolStripButton();
            this.pnlContents = new System.Windows.Forms.Panel();
            this.pbxFeedback = new System.Windows.Forms.PictureBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pnlContents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxFeedback)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 636);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(878, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.mergeToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(878, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCompare,
            this.mniCloseComparison,
            this.toolStripSeparator4,
            this.mniGenerateChangeScriptLeftToRight,
            this.mniGenerateChangeScriptRightToLeft,
            this.toolStripSeparator6,
            this.mniExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // mniCompare
            // 
            this.mniCompare.Image = ((System.Drawing.Image)(resources.GetObject("mniCompare.Image")));
            this.mniCompare.Name = "mniCompare";
            this.mniCompare.Size = new System.Drawing.Size(265, 22);
            this.mniCompare.Text = "&Compare...";
            this.mniCompare.Click += new System.EventHandler(this.mniCompare_Click);
            // 
            // mniCloseComparison
            // 
            this.mniCloseComparison.Image = ((System.Drawing.Image)(resources.GetObject("mniCloseComparison.Image")));
            this.mniCloseComparison.Name = "mniCloseComparison";
            this.mniCloseComparison.Size = new System.Drawing.Size(265, 22);
            this.mniCloseComparison.Text = "C&lose comparison";
            this.mniCloseComparison.Click += new System.EventHandler(this.mniCloseComparison_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(262, 6);
            // 
            // mniGenerateChangeScriptLeftToRight
            // 
            this.mniGenerateChangeScriptLeftToRight.Image = ((System.Drawing.Image)(resources.GetObject("mniGenerateChangeScriptLeftToRight.Image")));
            this.mniGenerateChangeScriptLeftToRight.Name = "mniGenerateChangeScriptLeftToRight";
            this.mniGenerateChangeScriptLeftToRight.Size = new System.Drawing.Size(265, 22);
            this.mniGenerateChangeScriptLeftToRight.Text = "Generate change script (left -> right)...";
            this.mniGenerateChangeScriptLeftToRight.Click += new System.EventHandler(this.mniGenerateChangeScriptLeftToRight_Click);
            // 
            // mniGenerateChangeScriptRightToLeft
            // 
            this.mniGenerateChangeScriptRightToLeft.Image = ((System.Drawing.Image)(resources.GetObject("mniGenerateChangeScriptRightToLeft.Image")));
            this.mniGenerateChangeScriptRightToLeft.Name = "mniGenerateChangeScriptRightToLeft";
            this.mniGenerateChangeScriptRightToLeft.Size = new System.Drawing.Size(265, 22);
            this.mniGenerateChangeScriptRightToLeft.Text = "Generate change script (right -> left)...";
            this.mniGenerateChangeScriptRightToLeft.Click += new System.EventHandler(this.mniGenerateChangeScriptRightToLeft_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(262, 6);
            // 
            // mniExit
            // 
            this.mniExit.Name = "mniExit";
            this.mniExit.Size = new System.Drawing.Size(265, 22);
            this.mniExit.Text = "E&xit";
            this.mniExit.Click += new System.EventHandler(this.mniExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCopyFromLeftDB,
            this.mniCopyFromRightDB,
            this.toolStripSeparator5,
            this.mniEditSelection});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // mniCopyFromLeftDB
            // 
            this.mniCopyFromLeftDB.Image = ((System.Drawing.Image)(resources.GetObject("mniCopyFromLeftDB.Image")));
            this.mniCopyFromLeftDB.Name = "mniCopyFromLeftDB";
            this.mniCopyFromLeftDB.Size = new System.Drawing.Size(198, 22);
            this.mniCopyFromLeftDB.Text = "Copy object from left DB";
            this.mniCopyFromLeftDB.Click += new System.EventHandler(this.mniCopyFromLeftDB_Click);
            // 
            // mniCopyFromRightDB
            // 
            this.mniCopyFromRightDB.Image = ((System.Drawing.Image)(resources.GetObject("mniCopyFromRightDB.Image")));
            this.mniCopyFromRightDB.Name = "mniCopyFromRightDB";
            this.mniCopyFromRightDB.Size = new System.Drawing.Size(198, 22);
            this.mniCopyFromRightDB.Text = "Copy object from right DB";
            this.mniCopyFromRightDB.Click += new System.EventHandler(this.mniCopyFromRightDB_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(195, 6);
            // 
            // mniEditSelection
            // 
            this.mniEditSelection.Image = ((System.Drawing.Image)(resources.GetObject("mniEditSelection.Image")));
            this.mniEditSelection.Name = "mniEditSelection";
            this.mniEditSelection.Size = new System.Drawing.Size(198, 22);
            this.mniEditSelection.Text = "Edit current difference...";
            this.mniEditSelection.Click += new System.EventHandler(this.mniEditSelection_Click);
            // 
            // mergeToolStripMenuItem
            // 
            this.mergeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniGotoNextDifference,
            this.mniGotoPreviousDifference});
            this.mergeToolStripMenuItem.Name = "mergeToolStripMenuItem";
            this.mergeToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.mergeToolStripMenuItem.Text = "&View";
            // 
            // mniGotoNextDifference
            // 
            this.mniGotoNextDifference.Image = ((System.Drawing.Image)(resources.GetObject("mniGotoNextDifference.Image")));
            this.mniGotoNextDifference.Name = "mniGotoNextDifference";
            this.mniGotoNextDifference.Size = new System.Drawing.Size(196, 22);
            this.mniGotoNextDifference.Text = "Go to next difference";
            this.mniGotoNextDifference.Click += new System.EventHandler(this.mniGotoNextDifference_Click);
            // 
            // mniGotoPreviousDifference
            // 
            this.mniGotoPreviousDifference.Image = ((System.Drawing.Image)(resources.GetObject("mniGotoPreviousDifference.Image")));
            this.mniGotoPreviousDifference.Name = "mniGotoPreviousDifference";
            this.mniGotoPreviousDifference.Size = new System.Drawing.Size(196, 22);
            this.mniGotoPreviousDifference.Text = "Go to previous difference";
            this.mniGotoPreviousDifference.Click += new System.EventHandler(this.mniGotoPreviousDifference_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCheckForUpdates,
            this.mniReport,
            this.toolStripSeparator8,
            this.mniAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // mniCheckForUpdates
            // 
            this.mniCheckForUpdates.Image = ((System.Drawing.Image)(resources.GetObject("mniCheckForUpdates.Image")));
            this.mniCheckForUpdates.Name = "mniCheckForUpdates";
            this.mniCheckForUpdates.Size = new System.Drawing.Size(268, 22);
            this.mniCheckForUpdates.Text = "Check for updates...";
            this.mniCheckForUpdates.Click += new System.EventHandler(this.mniCheckForUpdates_Click);
            // 
            // mniReport
            // 
            this.mniReport.Image = ((System.Drawing.Image)(resources.GetObject("mniReport.Image")));
            this.mniReport.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mniReport.Name = "mniReport";
            this.mniReport.Size = new System.Drawing.Size(268, 22);
            this.mniReport.Text = "Report a bug / Suggest improvements...";
            this.mniReport.Click += new System.EventHandler(this.mniReport_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(265, 6);
            // 
            // mniAbout
            // 
            this.mniAbout.Image = ((System.Drawing.Image)(resources.GetObject("mniAbout.Image")));
            this.mniAbout.Name = "mniAbout";
            this.mniAbout.Size = new System.Drawing.Size(268, 22);
            this.mniAbout.Text = "About...";
            this.mniAbout.Click += new System.EventHandler(this.mniAbout_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCompare,
            this.toolStripSeparator1,
            this.btnNextDiff,
            this.btnPreviousDiff,
            this.toolStripSeparator2,
            this.btnCopyFromLeftDB,
            this.btnCopyFromRightDB,
            this.toolStripSeparator3,
            this.btnEditSelectedDifference,
            this.toolStripSeparator9,
            this.btnExportDataDifferences});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(878, 31);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnCompare
            // 
            this.btnCompare.Image = ((System.Drawing.Image)(resources.GetObject("btnCompare.Image")));
            this.btnCompare.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnCompare.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(90, 28);
            this.btnCompare.Text = "Compare...";
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // btnNextDiff
            // 
            this.btnNextDiff.Image = ((System.Drawing.Image)(resources.GetObject("btnNextDiff.Image")));
            this.btnNextDiff.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnNextDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextDiff.Name = "btnNextDiff";
            this.btnNextDiff.Size = new System.Drawing.Size(77, 28);
            this.btnNextDiff.Text = "Next diff";
            this.btnNextDiff.Click += new System.EventHandler(this.btnNextDiff_Click);
            // 
            // btnPreviousDiff
            // 
            this.btnPreviousDiff.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousDiff.Image")));
            this.btnPreviousDiff.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPreviousDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreviousDiff.Name = "btnPreviousDiff";
            this.btnPreviousDiff.Size = new System.Drawing.Size(76, 28);
            this.btnPreviousDiff.Text = "Prev diff";
            this.btnPreviousDiff.Click += new System.EventHandler(this.btnPreviousDiff_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // btnCopyFromLeftDB
            // 
            this.btnCopyFromLeftDB.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyFromLeftDB.Image")));
            this.btnCopyFromLeftDB.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnCopyFromLeftDB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopyFromLeftDB.Name = "btnCopyFromLeftDB";
            this.btnCopyFromLeftDB.Size = new System.Drawing.Size(120, 28);
            this.btnCopyFromLeftDB.Text = "Copy from left DB";
            this.btnCopyFromLeftDB.Click += new System.EventHandler(this.btnCopyFromLeftDB_Click);
            // 
            // btnCopyFromRightDB
            // 
            this.btnCopyFromRightDB.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyFromRightDB.Image")));
            this.btnCopyFromRightDB.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnCopyFromRightDB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopyFromRightDB.Name = "btnCopyFromRightDB";
            this.btnCopyFromRightDB.Size = new System.Drawing.Size(126, 28);
            this.btnCopyFromRightDB.Text = "Copy from right DB";
            this.btnCopyFromRightDB.Click += new System.EventHandler(this.btnCopyFromRightDB_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
            // 
            // btnEditSelectedDifference
            // 
            this.btnEditSelectedDifference.Image = ((System.Drawing.Image)(resources.GetObject("btnEditSelectedDifference.Image")));
            this.btnEditSelectedDifference.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnEditSelectedDifference.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditSelectedDifference.Name = "btnEditSelectedDifference";
            this.btnEditSelectedDifference.Size = new System.Drawing.Size(160, 28);
            this.btnEditSelectedDifference.Text = "Edit selected difference...";
            this.btnEditSelectedDifference.Click += new System.EventHandler(this.btnEditSelectedDifference_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 31);
            // 
            // btnExportDataDifferences
            // 
            this.btnExportDataDifferences.Image = ((System.Drawing.Image)(resources.GetObject("btnExportDataDifferences.Image")));
            this.btnExportDataDifferences.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnExportDataDifferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportDataDifferences.Name = "btnExportDataDifferences";
            this.btnExportDataDifferences.Size = new System.Drawing.Size(161, 28);
            this.btnExportDataDifferences.Text = "Export data differences...";
            this.btnExportDataDifferences.Click += new System.EventHandler(this.btnExportDataDifferences_Click);
            // 
            // pnlContents
            // 
            this.pnlContents.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pnlContents.Controls.Add(this.pbxFeedback);
            this.pnlContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContents.Location = new System.Drawing.Point(0, 55);
            this.pnlContents.Name = "pnlContents";
            this.pnlContents.Size = new System.Drawing.Size(878, 581);
            this.pnlContents.TabIndex = 4;
            // 
            // pbxFeedback
            // 
            this.pbxFeedback.Image = global::SQLiteTurbo.Properties.Resources.normal_feedback_button;
            this.pbxFeedback.Location = new System.Drawing.Point(0, 224);
            this.pbxFeedback.Name = "pbxFeedback";
            this.pbxFeedback.Size = new System.Drawing.Size(40, 107);
            this.pbxFeedback.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxFeedback.TabIndex = 0;
            this.pbxFeedback.TabStop = false;
            this.pbxFeedback.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbxFeedback_MouseDown);
            this.pbxFeedback.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbxFeedback_MouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 658);
            this.Controls.Add(this.pnlContents);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "SQLite Compare";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlContents.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxFeedback)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel pnlContents;
        private System.Windows.Forms.ToolStripMenuItem mniCompare;
        private System.Windows.Forms.ToolStripButton btnCompare;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnNextDiff;
        private System.Windows.Forms.ToolStripButton btnPreviousDiff;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnCopyFromLeftDB;
        private System.Windows.Forms.ToolStripButton btnCopyFromRightDB;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mniCloseComparison;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mniExit;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripMenuItem mniGotoNextDifference;
        private System.Windows.Forms.ToolStripMenuItem mniGotoPreviousDifference;
        private System.Windows.Forms.ToolStripMenuItem mniCopyFromRightDB;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem mniEditSelection;
        private System.Windows.Forms.ToolStripMenuItem mniAbout;
        private System.Windows.Forms.ToolStripButton btnEditSelectedDifference;
        private System.Windows.Forms.ToolStripMenuItem mniCopyFromLeftDB;
        private System.Windows.Forms.ToolStripMenuItem mniReport;
        private System.Windows.Forms.ToolStripMenuItem mniGenerateChangeScriptLeftToRight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem mniGenerateChangeScriptRightToLeft;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem mniCheckForUpdates;
        private System.Windows.Forms.PictureBox pbxFeedback;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton btnExportDataDifferences;
    }
}