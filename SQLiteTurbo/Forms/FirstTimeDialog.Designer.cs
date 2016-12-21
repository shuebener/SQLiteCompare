namespace SQLiteTurbo
{
    partial class FirstTimeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstTimeDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.cbxAutomaticUpdates = new System.Windows.Forms.CheckBox();
            this.btnCheckNow = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.etchedLine1 = new SQLiteTurbo.EtchedLine();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "It is highly recommended to check if a newer version is available.";
            // 
            // cbxAutomaticUpdates
            // 
            this.cbxAutomaticUpdates.AutoSize = true;
            this.cbxAutomaticUpdates.Checked = true;
            this.cbxAutomaticUpdates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxAutomaticUpdates.Location = new System.Drawing.Point(16, 83);
            this.cbxAutomaticUpdates.Name = "cbxAutomaticUpdates";
            this.cbxAutomaticUpdates.Size = new System.Drawing.Size(265, 17);
            this.cbxAutomaticUpdates.TabIndex = 1;
            this.cbxAutomaticUpdates.Text = "Automatically check for updates on system startup.";
            this.cbxAutomaticUpdates.UseVisualStyleBackColor = true;
            this.cbxAutomaticUpdates.CheckedChanged += new System.EventHandler(this.cbxAutomaticUpdates_CheckedChanged);
            // 
            // btnCheckNow
            // 
            this.btnCheckNow.Location = new System.Drawing.Point(85, 48);
            this.btnCheckNow.Name = "btnCheckNow";
            this.btnCheckNow.Size = new System.Drawing.Size(208, 23);
            this.btnCheckNow.TabIndex = 0;
            this.btnCheckNow.Text = "Check now";
            this.btnCheckNow.UseVisualStyleBackColor = true;
            this.btnCheckNow.Click += new System.EventHandler(this.btnCheckNow_Click);
            // 
            // label2
            // 
            this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 38);
            this.label2.TabIndex = 4;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(286, 123);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // etchedLine1
            // 
            this.etchedLine1.Location = new System.Drawing.Point(16, 109);
            this.etchedLine1.Name = "etchedLine1";
            this.etchedLine1.Size = new System.Drawing.Size(345, 10);
            this.etchedLine1.TabIndex = 5;
            this.etchedLine1.Text = "etchedLine1";
            // 
            // FirstTimeDialog
            // 
            this.AcceptButton = this.btnCheckNow;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(377, 152);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.etchedLine1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCheckNow);
            this.Controls.Add(this.cbxAutomaticUpdates);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirstTimeDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Check for updates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbxAutomaticUpdates;
        private System.Windows.Forms.Button btnCheckNow;
        private System.Windows.Forms.Label label2;
        private EtchedLine etchedLine1;
        private System.Windows.Forms.Button btnClose;
    }
}