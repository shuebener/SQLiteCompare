namespace SQLiteTurbo
{
    partial class FeedbackDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeedbackDialog));
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtFeedback = new System.Windows.Forms.TextBox();
            this.pbxIdea = new System.Windows.Forms.PictureBox();
            this.pbxQuestion = new System.Windows.Forms.PictureBox();
            this.pbxProblem = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.btnSendFeedback = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxIdea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxProblem)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblMessage.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblMessage.Location = new System.Drawing.Point(35, 44);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(322, 23);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Share an idea";
            // 
            // txtFeedback
            // 
            this.txtFeedback.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.txtFeedback.Location = new System.Drawing.Point(39, 138);
            this.txtFeedback.Multiline = true;
            this.txtFeedback.Name = "txtFeedback";
            this.txtFeedback.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFeedback.Size = new System.Drawing.Size(517, 207);
            this.txtFeedback.TabIndex = 1;
            // 
            // pbxIdea
            // 
            this.pbxIdea.BackColor = System.Drawing.Color.Transparent;
            this.pbxIdea.Image = ((System.Drawing.Image)(resources.GetObject("pbxIdea.Image")));
            this.pbxIdea.Location = new System.Drawing.Point(39, 70);
            this.pbxIdea.Name = "pbxIdea";
            this.pbxIdea.Size = new System.Drawing.Size(103, 62);
            this.pbxIdea.TabIndex = 2;
            this.pbxIdea.TabStop = false;
            this.pbxIdea.Click += new System.EventHandler(this.pbxIdea_Click);
            // 
            // pbxQuestion
            // 
            this.pbxQuestion.BackColor = System.Drawing.Color.Transparent;
            this.pbxQuestion.Image = ((System.Drawing.Image)(resources.GetObject("pbxQuestion.Image")));
            this.pbxQuestion.Location = new System.Drawing.Point(148, 70);
            this.pbxQuestion.Name = "pbxQuestion";
            this.pbxQuestion.Size = new System.Drawing.Size(153, 62);
            this.pbxQuestion.TabIndex = 3;
            this.pbxQuestion.TabStop = false;
            this.pbxQuestion.Click += new System.EventHandler(this.pbxQuestion_Click);
            // 
            // pbxProblem
            // 
            this.pbxProblem.BackColor = System.Drawing.Color.Transparent;
            this.pbxProblem.Image = ((System.Drawing.Image)(resources.GetObject("pbxProblem.Image")));
            this.pbxProblem.Location = new System.Drawing.Point(307, 70);
            this.pbxProblem.Name = "pbxProblem";
            this.pbxProblem.Size = new System.Drawing.Size(147, 62);
            this.pbxProblem.TabIndex = 4;
            this.pbxProblem.TabStop = false;
            this.pbxProblem.Click += new System.EventHandler(this.pbxProblem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label2.Location = new System.Drawing.Point(36, 363);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Reply email (optional):";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(175, 360);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(243, 20);
            this.txtEmail.TabIndex = 6;
            // 
            // btnSendFeedback
            // 
            this.btnSendFeedback.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnSendFeedback.Location = new System.Drawing.Point(424, 359);
            this.btnSendFeedback.Name = "btnSendFeedback";
            this.btnSendFeedback.Size = new System.Drawing.Size(132, 23);
            this.btnSendFeedback.TabIndex = 7;
            this.btnSendFeedback.Text = "Send feedback";
            this.btnSendFeedback.UseVisualStyleBackColor = true;
            this.btnSendFeedback.Click += new System.EventHandler(this.btnSendFeedback_Click);
            // 
            // FeedbackDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(585, 394);
            this.ControlBox = false;
            this.Controls.Add(this.btnSendFeedback);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pbxProblem);
            this.Controls.Add(this.pbxQuestion);
            this.Controls.Add(this.pbxIdea);
            this.Controls.Add(this.txtFeedback);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FeedbackDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Feedback";
            this.TransparencyKey = System.Drawing.Color.Teal;
            this.Load += new System.EventHandler(this.FeedbackDialog_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FeedbackDialog_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbxIdea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxProblem)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtFeedback;
        private System.Windows.Forms.PictureBox pbxIdea;
        private System.Windows.Forms.PictureBox pbxQuestion;
        private System.Windows.Forms.PictureBox pbxProblem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Button btnSendFeedback;

    }
}