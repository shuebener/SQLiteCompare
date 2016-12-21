namespace SQLiteTurbo
{
    partial class ProgressDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialog));
            this.lblSecondaryMsg = new System.Windows.Forms.Label();
            this.pbrSecondaryProgress = new System.Windows.Forms.ProgressBar();
            this.pbrPrimaryProgress = new System.Windows.Forms.ProgressBar();
            this.lblPrimaryMsg = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblSecondaryMsg
            // 
            this.lblSecondaryMsg.AutoSize = true;
            this.lblSecondaryMsg.Location = new System.Drawing.Point(12, 9);
            this.lblSecondaryMsg.Name = "lblSecondaryMsg";
            this.lblSecondaryMsg.Size = new System.Drawing.Size(117, 13);
            this.lblSecondaryMsg.TabIndex = 0;
            this.lblSecondaryMsg.Text = "Comparing table Calls...";
            // 
            // pbrSecondaryProgress
            // 
            this.pbrSecondaryProgress.Location = new System.Drawing.Point(15, 27);
            this.pbrSecondaryProgress.Name = "pbrSecondaryProgress";
            this.pbrSecondaryProgress.Size = new System.Drawing.Size(471, 18);
            this.pbrSecondaryProgress.TabIndex = 1;
            this.pbrSecondaryProgress.Value = 34;
            // 
            // pbrPrimaryProgress
            // 
            this.pbrPrimaryProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbrPrimaryProgress.Location = new System.Drawing.Point(15, 71);
            this.pbrPrimaryProgress.Name = "pbrPrimaryProgress";
            this.pbrPrimaryProgress.Size = new System.Drawing.Size(471, 18);
            this.pbrPrimaryProgress.TabIndex = 3;
            this.pbrPrimaryProgress.Value = 75;
            // 
            // lblPrimaryMsg
            // 
            this.lblPrimaryMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPrimaryMsg.AutoSize = true;
            this.lblPrimaryMsg.Location = new System.Drawing.Point(12, 53);
            this.lblPrimaryMsg.Name = "lblPrimaryMsg";
            this.lblPrimaryMsg.Size = new System.Drawing.Size(80, 13);
            this.lblPrimaryMsg.TabIndex = 2;
            this.lblPrimaryMsg.Text = "75% Completed";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(411, 105);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(498, 138);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pbrPrimaryProgress);
            this.Controls.Add(this.lblPrimaryMsg);
            this.Controls.Add(this.pbrSecondaryProgress);
            this.Controls.Add(this.lblSecondaryMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Operation Progress";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSecondaryMsg;
        private System.Windows.Forms.ProgressBar pbrSecondaryProgress;
        private System.Windows.Forms.ProgressBar pbrPrimaryProgress;
        private System.Windows.Forms.Label lblPrimaryMsg;
        private System.Windows.Forms.Button btnCancel;
    }
}