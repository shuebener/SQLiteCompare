namespace SQLiteTurbo
{
    partial class CompareDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompareDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowseRight = new System.Windows.Forms.Button();
            this.txtRightFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseLeft = new System.Windows.Forms.Button();
            this.txtLeftFile = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbtnCompareSchemaOnly = new System.Windows.Forms.RadioButton();
            this.rbtnCompareSchemaAndData = new System.Windows.Forms.RadioButton();
            this.cbxCompareBlobFields = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnBrowseRight);
            this.groupBox1.Controls.Add(this.txtRightFile);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnBrowseLeft);
            this.groupBox1.Controls.Add(this.txtLeftFile);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(534, 134);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Files";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(271, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Right file: (you can also drag and drop from the Explorer)";
            // 
            // btnBrowseRight
            // 
            this.btnBrowseRight.Location = new System.Drawing.Point(451, 93);
            this.btnBrowseRight.Name = "btnBrowseRight";
            this.btnBrowseRight.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseRight.TabIndex = 7;
            this.btnBrowseRight.Text = "Browse...";
            this.btnBrowseRight.UseVisualStyleBackColor = true;
            this.btnBrowseRight.Click += new System.EventHandler(this.btnBrowseRight_Click);
            // 
            // txtRightFile
            // 
            this.txtRightFile.AllowDrop = true;
            this.txtRightFile.Location = new System.Drawing.Point(16, 95);
            this.txtRightFile.Name = "txtRightFile";
            this.txtRightFile.Size = new System.Drawing.Size(429, 20);
            this.txtRightFile.TabIndex = 6;
            this.txtRightFile.TextChanged += new System.EventHandler(this.txtRightFile_TextChanged);
            this.txtRightFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtRightFile_DragDrop);
            this.txtRightFile.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.txtRightFile_GiveFeedback);
            this.txtRightFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtRightFile_DragEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Left file: (you can also drag and drop from the Explorer)";
            // 
            // btnBrowseLeft
            // 
            this.btnBrowseLeft.Location = new System.Drawing.Point(451, 42);
            this.btnBrowseLeft.Name = "btnBrowseLeft";
            this.btnBrowseLeft.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseLeft.TabIndex = 1;
            this.btnBrowseLeft.Text = "Browse...";
            this.btnBrowseLeft.UseVisualStyleBackColor = true;
            this.btnBrowseLeft.Click += new System.EventHandler(this.btnBrowseLeft_Click);
            // 
            // txtLeftFile
            // 
            this.txtLeftFile.AllowDrop = true;
            this.txtLeftFile.Location = new System.Drawing.Point(16, 44);
            this.txtLeftFile.Name = "txtLeftFile";
            this.txtLeftFile.Size = new System.Drawing.Size(429, 20);
            this.txtLeftFile.TabIndex = 0;
            this.txtLeftFile.TextChanged += new System.EventHandler(this.txtLeftFile_TextChanged);
            this.txtLeftFile.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.txtLeftFile_QueryContinueDrag);
            this.txtLeftFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtLeftFile_DragDrop);
            this.txtLeftFile.DragLeave += new System.EventHandler(this.txtLeftFile_DragLeave);
            this.txtLeftFile.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.txtLeftFile_GiveFeedback);
            this.txtLeftFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtLeftFile_DragEnter);
            this.txtLeftFile.DragOver += new System.Windows.Forms.DragEventHandler(this.txtLeftFile_DragOver);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(471, 202);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(390, 202);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbtnCompareSchemaOnly
            // 
            this.rbtnCompareSchemaOnly.AutoSize = true;
            this.rbtnCompareSchemaOnly.Checked = true;
            this.rbtnCompareSchemaOnly.Location = new System.Drawing.Point(28, 164);
            this.rbtnCompareSchemaOnly.Name = "rbtnCompareSchemaOnly";
            this.rbtnCompareSchemaOnly.Size = new System.Drawing.Size(129, 17);
            this.rbtnCompareSchemaOnly.TabIndex = 4;
            this.rbtnCompareSchemaOnly.TabStop = true;
            this.rbtnCompareSchemaOnly.Text = "Compare schema only";
            this.rbtnCompareSchemaOnly.UseVisualStyleBackColor = true;
            this.rbtnCompareSchemaOnly.CheckedChanged += new System.EventHandler(this.rbtnCompareSchemaOnly_CheckedChanged);
            // 
            // rbtnCompareSchemaAndData
            // 
            this.rbtnCompareSchemaAndData.AutoSize = true;
            this.rbtnCompareSchemaAndData.Location = new System.Drawing.Point(163, 164);
            this.rbtnCompareSchemaAndData.Name = "rbtnCompareSchemaAndData";
            this.rbtnCompareSchemaAndData.Size = new System.Drawing.Size(152, 17);
            this.rbtnCompareSchemaAndData.TabIndex = 5;
            this.rbtnCompareSchemaAndData.Text = "Compare schema and data";
            this.rbtnCompareSchemaAndData.UseVisualStyleBackColor = true;
            this.rbtnCompareSchemaAndData.CheckedChanged += new System.EventHandler(this.rbtnCompareSchemaAndData_CheckedChanged);
            // 
            // cbxCompareBlobFields
            // 
            this.cbxCompareBlobFields.AutoSize = true;
            this.cbxCompareBlobFields.Location = new System.Drawing.Point(184, 189);
            this.cbxCompareBlobFields.Name = "cbxCompareBlobFields";
            this.cbxCompareBlobFields.Size = new System.Drawing.Size(126, 17);
            this.cbxCompareBlobFields.TabIndex = 6;
            this.cbxCompareBlobFields.Text = "Compare BLOB fields";
            this.cbxCompareBlobFields.UseVisualStyleBackColor = true;
            // 
            // CompareDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(558, 236);
            this.Controls.Add(this.cbxCompareBlobFields);
            this.Controls.Add(this.rbtnCompareSchemaAndData);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rbtnCompareSchemaOnly);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CompareDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Comparison Details";
            this.Load += new System.EventHandler(this.CompareDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLeftFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowseRight;
        private System.Windows.Forms.TextBox txtRightFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseLeft;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbtnCompareSchemaAndData;
        private System.Windows.Forms.RadioButton rbtnCompareSchemaOnly;
        private System.Windows.Forms.CheckBox cbxCompareBlobFields;
    }
}