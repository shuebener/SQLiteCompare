namespace SQLiteTurbo
{
    partial class UnexpectedErrorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnexpectedErrorDialog));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtErrorLog = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLastActions = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.etchedLine1 = new SQLiteTurbo.EtchedLine();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLogFilePath = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(61, 88);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Salmon;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.Location = new System.Drawing.Point(84, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(390, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "An unexpected error has occurred in SQLite Compare";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(387, 495);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(87, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Exit";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(84, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(375, 35);
            this.label4.TabIndex = 11;
            this.label4.Text = "Unfortunately an unexpected software error has occurred and the software needs to" +
                " shutdown.";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(84, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(375, 53);
            this.label3.TabIndex = 12;
            this.label3.Text = "The software has prepared a detailed bug report and we would be grateful if you t" +
                "ake the time to fill in some of the details so we can better diagnose and fix th" +
                "e problem as soon as possible.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label2.Location = new System.Drawing.Point(84, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Error log:";
            // 
            // txtErrorLog
            // 
            this.txtErrorLog.Location = new System.Drawing.Point(87, 156);
            this.txtErrorLog.Multiline = true;
            this.txtErrorLog.Name = "txtErrorLog";
            this.txtErrorLog.ReadOnly = true;
            this.txtErrorLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErrorLog.Size = new System.Drawing.Size(387, 130);
            this.txtErrorLog.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label5.Location = new System.Drawing.Point(84, 341);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(364, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "What was the last action you did before the problem occurred?";
            // 
            // txtLastActions
            // 
            this.txtLastActions.Location = new System.Drawing.Point(87, 360);
            this.txtLastActions.MaxLength = 10000;
            this.txtLastActions.Multiline = true;
            this.txtLastActions.Name = "txtLastActions";
            this.txtLastActions.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLastActions.Size = new System.Drawing.Size(387, 54);
            this.txtLastActions.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label6.Location = new System.Drawing.Point(84, 427);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(275, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Your email address (if you want to get notified):";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(87, 446);
            this.txtEmail.MaxLength = 512;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEmail.Size = new System.Drawing.Size(387, 20);
            this.txtEmail.TabIndex = 3;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(268, 495);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(113, 23);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send the report";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // etchedLine1
            // 
            this.etchedLine1.Location = new System.Drawing.Point(12, 479);
            this.etchedLine1.Name = "etchedLine1";
            this.etchedLine1.Size = new System.Drawing.Size(462, 10);
            this.etchedLine1.TabIndex = 9;
            this.etchedLine1.Text = "etchedLine1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label7.Location = new System.Drawing.Point(84, 292);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Log file path:";
            // 
            // txtLogFilePath
            // 
            this.txtLogFilePath.Location = new System.Drawing.Point(87, 311);
            this.txtLogFilePath.MaxLength = 512;
            this.txtLogFilePath.Name = "txtLogFilePath";
            this.txtLogFilePath.ReadOnly = true;
            this.txtLogFilePath.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLogFilePath.Size = new System.Drawing.Size(387, 20);
            this.txtLogFilePath.TabIndex = 1;
            // 
            // UnexpectedErrorDialog
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(486, 533);
            this.Controls.Add(this.txtLogFilePath);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtLastActions);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtErrorLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.etchedLine1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UnexpectedErrorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unexpected Error";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.UnexpectedErrorDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private EtchedLine etchedLine1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtErrorLog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLastActions;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLogFilePath;
    }
}