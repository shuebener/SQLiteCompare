namespace SQLiteTurbo
{
    partial class LicenseRegistrationDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseRegistrationDialog));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblLicenseHasExpired = new System.Windows.Forms.Label();
            this.txtLicenseType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLicensedTo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pbxLicense = new System.Windows.Forms.PictureBox();
            this.lnkPurchase = new System.Windows.Forms.LinkLabel();
            this.txtLicenseFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLicense)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(310, 216);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(222, 216);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(82, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(14, 67);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(98, 23);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "License file...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLicenseHasExpired);
            this.groupBox1.Controls.Add(this.txtLicenseType);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtLicensedTo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.pbxLicense);
            this.groupBox1.Location = new System.Drawing.Point(14, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 104);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "License details";
            // 
            // lblLicenseHasExpired
            // 
            this.lblLicenseHasExpired.AutoSize = true;
            this.lblLicenseHasExpired.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblLicenseHasExpired.ForeColor = System.Drawing.Color.Maroon;
            this.lblLicenseHasExpired.Location = new System.Drawing.Point(159, 82);
            this.lblLicenseHasExpired.Name = "lblLicenseHasExpired";
            this.lblLicenseHasExpired.Size = new System.Drawing.Size(163, 15);
            this.lblLicenseHasExpired.TabIndex = 18;
            this.lblLicenseHasExpired.Text = "This license has expired";
            // 
            // txtLicenseType
            // 
            this.txtLicenseType.Location = new System.Drawing.Point(162, 32);
            this.txtLicenseType.Name = "txtLicenseType";
            this.txtLicenseType.ReadOnly = true;
            this.txtLicenseType.Size = new System.Drawing.Size(191, 20);
            this.txtLicenseType.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(86, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "License type:";
            // 
            // txtLicensedTo
            // 
            this.txtLicensedTo.Location = new System.Drawing.Point(162, 58);
            this.txtLicensedTo.Name = "txtLicensedTo";
            this.txtLicensedTo.ReadOnly = true;
            this.txtLicensedTo.Size = new System.Drawing.Size(191, 20);
            this.txtLicensedTo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(86, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Licensed to:";
            // 
            // pbxLicense
            // 
            this.pbxLicense.Image = ((System.Drawing.Image)(resources.GetObject("pbxLicense.Image")));
            this.pbxLicense.Location = new System.Drawing.Point(16, 26);
            this.pbxLicense.Name = "pbxLicense";
            this.pbxLicense.Size = new System.Drawing.Size(52, 52);
            this.pbxLicense.TabIndex = 14;
            this.pbxLicense.TabStop = false;
            // 
            // lnkPurchase
            // 
            this.lnkPurchase.AutoSize = true;
            this.lnkPurchase.Location = new System.Drawing.Point(11, 221);
            this.lnkPurchase.Name = "lnkPurchase";
            this.lnkPurchase.Size = new System.Drawing.Size(120, 13);
            this.lnkPurchase.TabIndex = 7;
            this.lnkPurchase.TabStop = true;
            this.lnkPurchase.Text = "Purchase License Page";
            this.lnkPurchase.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPurchase_LinkClicked);
            // 
            // txtLicenseFile
            // 
            this.txtLicenseFile.Location = new System.Drawing.Point(118, 69);
            this.txtLicenseFile.Name = "txtLicenseFile";
            this.txtLicenseFile.ReadOnly = true;
            this.txtLicenseFile.Size = new System.Drawing.Size(249, 20);
            this.txtLicenseFile.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(355, 45);
            this.label1.TabIndex = 18;
            this.label1.Text = "Select the path to the license file you\'ve received as part of the purchasing or " +
                "evaluation transaction and click OK to install the license on your machine.";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "valid");
            this.imageList1.Images.SetKeyName(1, "not-valid");
            this.imageList1.Images.SetKeyName(2, "initial");
            // 
            // LicenseRegistrationDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(402, 245);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLicenseFile);
            this.Controls.Add(this.lnkPurchase);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseRegistrationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Install License";
            this.Load += new System.EventHandler(this.LicenseRegistrationDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLicense)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLicenseType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLicensedTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pbxLicense;
        private System.Windows.Forms.LinkLabel lnkPurchase;
        private System.Windows.Forms.TextBox txtLicenseFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLicenseHasExpired;
        private System.Windows.Forms.ImageList imageList1;
    }
}