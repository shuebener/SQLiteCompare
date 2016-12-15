namespace SQLiteTurbo
{
    partial class ExportChangesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportChangesDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.cbxExportUpdates = new System.Windows.Forms.CheckBox();
            this.cbxExportAdded = new System.Windows.Forms.CheckBox();
            this.cbxExportDeleted = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.etchedLine1 = new SQLiteTurbo.EtchedLine();
            this.btnExportAndClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Export to:";
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(70, 12);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(309, 20);
            this.txtFile.TabIndex = 0;
            this.txtFile.TextChanged += new System.EventHandler(this.txtFile_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(385, 10);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // cbxExportUpdates
            // 
            this.cbxExportUpdates.AutoSize = true;
            this.cbxExportUpdates.Checked = true;
            this.cbxExportUpdates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxExportUpdates.Location = new System.Drawing.Point(70, 45);
            this.cbxExportUpdates.Name = "cbxExportUpdates";
            this.cbxExportUpdates.Size = new System.Drawing.Size(140, 17);
            this.cbxExportUpdates.TabIndex = 2;
            this.cbxExportUpdates.Text = "Export row modifications";
            this.cbxExportUpdates.UseVisualStyleBackColor = true;
            this.cbxExportUpdates.CheckedChanged += new System.EventHandler(this.cbxExportUpdates_CheckedChanged);
            // 
            // cbxExportAdded
            // 
            this.cbxExportAdded.AutoSize = true;
            this.cbxExportAdded.Checked = true;
            this.cbxExportAdded.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxExportAdded.Location = new System.Drawing.Point(70, 68);
            this.cbxExportAdded.Name = "cbxExportAdded";
            this.cbxExportAdded.Size = new System.Drawing.Size(114, 17);
            this.cbxExportAdded.TabIndex = 3;
            this.cbxExportAdded.Text = "Export added rows";
            this.cbxExportAdded.UseVisualStyleBackColor = true;
            this.cbxExportAdded.CheckedChanged += new System.EventHandler(this.cbxExportAdded_CheckedChanged);
            // 
            // cbxExportDeleted
            // 
            this.cbxExportDeleted.AutoSize = true;
            this.cbxExportDeleted.Checked = true;
            this.cbxExportDeleted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxExportDeleted.Location = new System.Drawing.Point(70, 91);
            this.cbxExportDeleted.Name = "cbxExportDeleted";
            this.cbxExportDeleted.Size = new System.Drawing.Size(119, 17);
            this.cbxExportDeleted.TabIndex = 4;
            this.cbxExportDeleted.Text = "Export deleted rows";
            this.cbxExportDeleted.UseVisualStyleBackColor = true;
            this.cbxExportDeleted.CheckedChanged += new System.EventHandler(this.cbxExportDeleted_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(385, 139);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(238, 139);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(141, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "Export and Open";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.FileName = "datadiffs.csv";
            this.saveFileDialog1.Filter = "CSV Files|*.csv|All Files|*.*";
            this.saveFileDialog1.RestoreDirectory = true;
            this.saveFileDialog1.Title = "Select export file";
            // 
            // etchedLine1
            // 
            this.etchedLine1.Location = new System.Drawing.Point(15, 123);
            this.etchedLine1.Name = "etchedLine1";
            this.etchedLine1.Size = new System.Drawing.Size(445, 10);
            this.etchedLine1.TabIndex = 6;
            this.etchedLine1.Text = "etchedLine1";
            // 
            // btnExportAndClose
            // 
            this.btnExportAndClose.Location = new System.Drawing.Point(93, 139);
            this.btnExportAndClose.Name = "btnExportAndClose";
            this.btnExportAndClose.Size = new System.Drawing.Size(128, 23);
            this.btnExportAndClose.TabIndex = 7;
            this.btnExportAndClose.Text = "Export and close";
            this.btnExportAndClose.UseVisualStyleBackColor = true;
            this.btnExportAndClose.Click += new System.EventHandler(this.btnExportAndClose_Click);
            // 
            // ExportChangesDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(476, 173);
            this.Controls.Add(this.btnExportAndClose);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.etchedLine1);
            this.Controls.Add(this.cbxExportDeleted);
            this.Controls.Add(this.cbxExportAdded);
            this.Controls.Add(this.cbxExportUpdates);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportChangesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Changes";
            this.Load += new System.EventHandler(this.ExportChangesDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.CheckBox cbxExportUpdates;
        private System.Windows.Forms.CheckBox cbxExportAdded;
        private System.Windows.Forms.CheckBox cbxExportDeleted;
        private EtchedLine etchedLine1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnExportAndClose;
    }
}