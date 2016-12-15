namespace SQLiteTurbo
{
    partial class IndefiniteProgressDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.spinningProgress1 = new CircularProgress.SpinningProgress.SpinningProgress();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.lblMessage);
            this.panel1.Controls.Add(this.spinningProgress1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(372, 60);
            this.panel1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(286, 20);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(64, 14);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(216, 30);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "Sending email to server.";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spinningProgress1
            // 
            this.spinningProgress1.AutoIncrementFrequency = 100;
            this.spinningProgress1.Location = new System.Drawing.Point(5, 6);
            this.spinningProgress1.Name = "spinningProgress1";
            this.spinningProgress1.Size = new System.Drawing.Size(45, 45);
            this.spinningProgress1.TabIndex = 0;
            this.spinningProgress1.TransistionSegment = 5;
            // 
            // IndefiniteProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 60);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "IndefiniteProgressDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "IndefiniteProgressDialog";
            this.Shown += new System.EventHandler(this.IndefiniteProgressDialog_Shown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private CircularProgress.SpinningProgress.SpinningProgress spinningProgress1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnCancel;
    }
}