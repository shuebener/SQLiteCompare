namespace SQLiteTurbo
{
    partial class UserSurveyDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserSurveyDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProblems = new System.Windows.Forms.TextBox();
            this.txtMissingFeatures = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtImprovements = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.etchedLine1 = new SQLiteTurbo.EtchedLine();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtName);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtEmail);
            this.panel1.Controls.Add(this.etchedLine1);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(632, 549);
            this.panel1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(69, 66);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(96, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(522, 66);
            this.label1.TabIndex = 1;
            this.label1.Text = "Gain a free software license by helping us improve SQLite Compare!";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label2.ForeColor = System.Drawing.Color.Green;
            this.label2.Location = new System.Drawing.Point(12, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(606, 66);
            this.label2.TabIndex = 3;
            this.label2.Text = "Here is the deal... You\'ll tell us how SQLite Compare can be improved or fixed an" +
                "d we\'ll send you a free license if we find your information valuable for improvi" +
                "ng the product !";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtImprovements);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtMissingFeatures);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtProblems);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 165);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(606, 287);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Here is what we want to know ...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label3.Location = new System.Drawing.Point(20, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(486, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Did you have any software related problems? if so - can you describe them in deta" +
                "il?";
            // 
            // txtProblems
            // 
            this.txtProblems.Location = new System.Drawing.Point(23, 47);
            this.txtProblems.Multiline = true;
            this.txtProblems.Name = "txtProblems";
            this.txtProblems.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtProblems.Size = new System.Drawing.Size(570, 53);
            this.txtProblems.TabIndex = 0;
            // 
            // txtMissingFeatures
            // 
            this.txtMissingFeatures.Location = new System.Drawing.Point(23, 128);
            this.txtMissingFeatures.Multiline = true;
            this.txtMissingFeatures.Name = "txtMissingFeatures";
            this.txtMissingFeatures.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMissingFeatures.Size = new System.Drawing.Size(570, 55);
            this.txtMissingFeatures.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label4.Location = new System.Drawing.Point(20, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(386, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Do you think that there are missing features that should be added?";
            // 
            // txtImprovements
            // 
            this.txtImprovements.Location = new System.Drawing.Point(23, 209);
            this.txtImprovements.Multiline = true;
            this.txtImprovements.Name = "txtImprovements";
            this.txtImprovements.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtImprovements.Size = new System.Drawing.Size(570, 55);
            this.txtImprovements.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label5.Location = new System.Drawing.Point(20, 191);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(330, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Anything else that may help us improve SQLite Compare?";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label6.Location = new System.Drawing.Point(32, 473);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Your name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(118, 470);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(170, 20);
            this.txtName.TabIndex = 0;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(118, 508);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(170, 20);
            this.txtEmail.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label7.Location = new System.Drawing.Point(32, 511);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Your email:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label8.Location = new System.Drawing.Point(294, 511);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(264, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "(your name and email will remain confidential)";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(342, 558);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(135, 23);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(483, 558);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(135, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // etchedLine1
            // 
            this.etchedLine1.Location = new System.Drawing.Point(12, 84);
            this.etchedLine1.Name = "etchedLine1";
            this.etchedLine1.Size = new System.Drawing.Size(606, 23);
            this.etchedLine1.TabIndex = 2;
            this.etchedLine1.Text = "etchedLine1";
            // 
            // UserSurveyDialog
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(632, 589);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserSurveyDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Short Survey";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private EtchedLine etchedLine1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtProblems;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMissingFeatures;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtImprovements;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnClose;
    }
}