namespace SQLiteTurbo
{
    partial class CellValueEditorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CellValueEditorDialog));
            this.tbcTypes = new System.Windows.Forms.TabControl();
            this.tbpEditInteger = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.numInteger = new System.Windows.Forms.NumericUpDown();
            this.tbpEditFloatingPoint = new System.Windows.Forms.TabPage();
            this.txtFloatingPoint = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbpEditBoolean = new System.Windows.Forms.TabPage();
            this.rbtnFalse = new System.Windows.Forms.RadioButton();
            this.rbtnTrue = new System.Windows.Forms.RadioButton();
            this.tbpEditText = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.tbpEditBlob = new System.Windows.Forms.TabPage();
            this.lblBlobSize = new System.Windows.Forms.Label();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.ucHexEditor = new Be.Windows.Forms.HexBox();
            this.tbpEditDateTime = new System.Windows.Forms.TabPage();
            this.pnlDateTimeWarning = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dtpTime = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxSetAsNull = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tbpEditGuid = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.txtGuid = new System.Windows.Forms.MaskedTextBox();
            this.tbcTypes.SuspendLayout();
            this.tbpEditInteger.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInteger)).BeginInit();
            this.tbpEditFloatingPoint.SuspendLayout();
            this.tbpEditBoolean.SuspendLayout();
            this.tbpEditText.SuspendLayout();
            this.tbpEditBlob.SuspendLayout();
            this.tbpEditDateTime.SuspendLayout();
            this.pnlDateTimeWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tbpEditGuid.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcTypes
            // 
            this.tbcTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbcTypes.Controls.Add(this.tbpEditInteger);
            this.tbcTypes.Controls.Add(this.tbpEditFloatingPoint);
            this.tbcTypes.Controls.Add(this.tbpEditBoolean);
            this.tbcTypes.Controls.Add(this.tbpEditText);
            this.tbcTypes.Controls.Add(this.tbpEditBlob);
            this.tbcTypes.Controls.Add(this.tbpEditDateTime);
            this.tbcTypes.Controls.Add(this.tbpEditGuid);
            this.tbcTypes.Location = new System.Drawing.Point(12, 34);
            this.tbcTypes.Name = "tbcTypes";
            this.tbcTypes.SelectedIndex = 0;
            this.tbcTypes.Size = new System.Drawing.Size(514, 231);
            this.tbcTypes.TabIndex = 0;
            this.tbcTypes.SelectedIndexChanged += new System.EventHandler(this.tbcTypes_SelectedIndexChanged);
            // 
            // tbpEditInteger
            // 
            this.tbpEditInteger.Controls.Add(this.label1);
            this.tbpEditInteger.Controls.Add(this.numInteger);
            this.tbpEditInteger.Location = new System.Drawing.Point(4, 22);
            this.tbpEditInteger.Name = "tbpEditInteger";
            this.tbpEditInteger.Padding = new System.Windows.Forms.Padding(3);
            this.tbpEditInteger.Size = new System.Drawing.Size(506, 205);
            this.tbpEditInteger.TabIndex = 2;
            this.tbpEditInteger.Text = "Integer";
            this.tbpEditInteger.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Value:";
            // 
            // numInteger
            // 
            this.numInteger.Location = new System.Drawing.Point(66, 17);
            this.numInteger.Name = "numInteger";
            this.numInteger.Size = new System.Drawing.Size(151, 20);
            this.numInteger.TabIndex = 0;
            // 
            // tbpEditFloatingPoint
            // 
            this.tbpEditFloatingPoint.Controls.Add(this.txtFloatingPoint);
            this.tbpEditFloatingPoint.Controls.Add(this.label6);
            this.tbpEditFloatingPoint.Location = new System.Drawing.Point(4, 22);
            this.tbpEditFloatingPoint.Name = "tbpEditFloatingPoint";
            this.tbpEditFloatingPoint.Padding = new System.Windows.Forms.Padding(3);
            this.tbpEditFloatingPoint.Size = new System.Drawing.Size(506, 205);
            this.tbpEditFloatingPoint.TabIndex = 3;
            this.tbpEditFloatingPoint.Text = "Floating Point";
            this.tbpEditFloatingPoint.UseVisualStyleBackColor = true;
            // 
            // txtFloatingPoint
            // 
            this.txtFloatingPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFloatingPoint.Location = new System.Drawing.Point(66, 15);
            this.txtFloatingPoint.Name = "txtFloatingPoint";
            this.txtFloatingPoint.Size = new System.Drawing.Size(274, 20);
            this.txtFloatingPoint.TabIndex = 7;
            this.txtFloatingPoint.TextChanged += new System.EventHandler(this.txtFloatingPoint_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Value:";
            // 
            // tbpEditBoolean
            // 
            this.tbpEditBoolean.Controls.Add(this.rbtnFalse);
            this.tbpEditBoolean.Controls.Add(this.rbtnTrue);
            this.tbpEditBoolean.Location = new System.Drawing.Point(4, 22);
            this.tbpEditBoolean.Name = "tbpEditBoolean";
            this.tbpEditBoolean.Padding = new System.Windows.Forms.Padding(3);
            this.tbpEditBoolean.Size = new System.Drawing.Size(506, 205);
            this.tbpEditBoolean.TabIndex = 6;
            this.tbpEditBoolean.Text = "Boolean";
            this.tbpEditBoolean.UseVisualStyleBackColor = true;
            // 
            // rbtnFalse
            // 
            this.rbtnFalse.AutoSize = true;
            this.rbtnFalse.Location = new System.Drawing.Point(82, 19);
            this.rbtnFalse.Name = "rbtnFalse";
            this.rbtnFalse.Size = new System.Drawing.Size(58, 17);
            this.rbtnFalse.TabIndex = 1;
            this.rbtnFalse.Text = "FALSE";
            this.rbtnFalse.UseVisualStyleBackColor = true;
            // 
            // rbtnTrue
            // 
            this.rbtnTrue.AutoSize = true;
            this.rbtnTrue.Checked = true;
            this.rbtnTrue.Location = new System.Drawing.Point(21, 19);
            this.rbtnTrue.Name = "rbtnTrue";
            this.rbtnTrue.Size = new System.Drawing.Size(55, 17);
            this.rbtnTrue.TabIndex = 0;
            this.rbtnTrue.TabStop = true;
            this.rbtnTrue.Text = "TRUE";
            this.rbtnTrue.UseVisualStyleBackColor = true;
            // 
            // tbpEditText
            // 
            this.tbpEditText.Controls.Add(this.label7);
            this.tbpEditText.Controls.Add(this.txtValue);
            this.tbpEditText.Location = new System.Drawing.Point(4, 22);
            this.tbpEditText.Name = "tbpEditText";
            this.tbpEditText.Padding = new System.Windows.Forms.Padding(3);
            this.tbpEditText.Size = new System.Drawing.Size(506, 205);
            this.tbpEditText.TabIndex = 4;
            this.tbpEditText.Text = "Text";
            this.tbpEditText.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Value:";
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Location = new System.Drawing.Point(15, 33);
            this.txtValue.Multiline = true;
            this.txtValue.Name = "txtValue";
            this.txtValue.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtValue.Size = new System.Drawing.Size(471, 153);
            this.txtValue.TabIndex = 0;
            // 
            // tbpEditBlob
            // 
            this.tbpEditBlob.Controls.Add(this.lblBlobSize);
            this.tbpEditBlob.Controls.Add(this.btnSaveAs);
            this.tbpEditBlob.Controls.Add(this.btnOpen);
            this.tbpEditBlob.Controls.Add(this.label8);
            this.tbpEditBlob.Controls.Add(this.ucHexEditor);
            this.tbpEditBlob.Location = new System.Drawing.Point(4, 22);
            this.tbpEditBlob.Name = "tbpEditBlob";
            this.tbpEditBlob.Padding = new System.Windows.Forms.Padding(3);
            this.tbpEditBlob.Size = new System.Drawing.Size(506, 205);
            this.tbpEditBlob.TabIndex = 5;
            this.tbpEditBlob.Text = "Blob";
            this.tbpEditBlob.UseVisualStyleBackColor = true;
            // 
            // lblBlobSize
            // 
            this.lblBlobSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBlobSize.Location = new System.Drawing.Point(224, 176);
            this.lblBlobSize.Name = "lblBlobSize";
            this.lblBlobSize.Size = new System.Drawing.Size(265, 23);
            this.lblBlobSize.TabIndex = 10;
            this.lblBlobSize.Text = "label4";
            this.lblBlobSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveAs.Location = new System.Drawing.Point(96, 176);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAs.TabIndex = 9;
            this.btnSaveAs.Text = "Save as...";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpen.Location = new System.Drawing.Point(15, 176);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 8;
            this.btnOpen.Text = "Open ...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnLoadFrom_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Value:";
            // 
            // ucHexEditor
            // 
            this.ucHexEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucHexEditor.BytesPerLine = 10;
            this.ucHexEditor.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ucHexEditor.LineInfoForeColor = System.Drawing.Color.Gray;
            this.ucHexEditor.LineInfoVisible = true;
            this.ucHexEditor.Location = new System.Drawing.Point(15, 33);
            this.ucHexEditor.Name = "ucHexEditor";
            this.ucHexEditor.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.ucHexEditor.Size = new System.Drawing.Size(474, 137);
            this.ucHexEditor.StringViewVisible = true;
            this.ucHexEditor.TabIndex = 0;
            this.ucHexEditor.UseFixedBytesPerLine = true;
            this.ucHexEditor.VScrollBarVisible = true;
            // 
            // tbpEditDateTime
            // 
            this.tbpEditDateTime.Controls.Add(this.pnlDateTimeWarning);
            this.tbpEditDateTime.Controls.Add(this.dtpTime);
            this.tbpEditDateTime.Controls.Add(this.label3);
            this.tbpEditDateTime.Controls.Add(this.dtpDate);
            this.tbpEditDateTime.Controls.Add(this.label2);
            this.tbpEditDateTime.Location = new System.Drawing.Point(4, 22);
            this.tbpEditDateTime.Name = "tbpEditDateTime";
            this.tbpEditDateTime.Padding = new System.Windows.Forms.Padding(3);
            this.tbpEditDateTime.Size = new System.Drawing.Size(506, 205);
            this.tbpEditDateTime.TabIndex = 7;
            this.tbpEditDateTime.Text = "DateTime";
            this.tbpEditDateTime.UseVisualStyleBackColor = true;
            // 
            // pnlDateTimeWarning
            // 
            this.pnlDateTimeWarning.Controls.Add(this.label4);
            this.pnlDateTimeWarning.Controls.Add(this.pictureBox1);
            this.pnlDateTimeWarning.Location = new System.Drawing.Point(18, 56);
            this.pnlDateTimeWarning.Name = "pnlDateTimeWarning";
            this.pnlDateTimeWarning.Size = new System.Drawing.Size(418, 79);
            this.pnlDateTimeWarning.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(46, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(369, 37);
            this.label4.TabIndex = 1;
            this.label4.Text = "The field contains a value that cannot be parsed as a valid DateTime field!";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(37, 36);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // dtpTime
            // 
            this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpTime.Location = new System.Drawing.Point(189, 15);
            this.dtpTime.Name = "dtpTime";
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Size = new System.Drawing.Size(73, 20);
            this.dtpTime.TabIndex = 5;
            this.dtpTime.ValueChanged += new System.EventHandler(this.dtpTime_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(150, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Time:";
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(54, 15);
            this.dtpDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.ShowUpDown = true;
            this.dtpDate.Size = new System.Drawing.Size(91, 20);
            this.dtpDate.TabIndex = 3;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Date:";
            // 
            // cbxSetAsNull
            // 
            this.cbxSetAsNull.AutoSize = true;
            this.cbxSetAsNull.Location = new System.Drawing.Point(12, 11);
            this.cbxSetAsNull.Name = "cbxSetAsNull";
            this.cbxSetAsNull.Size = new System.Drawing.Size(87, 17);
            this.cbxSetAsNull.TabIndex = 1;
            this.cbxSetAsNull.Text = "Set as NULL";
            this.cbxSetAsNull.UseVisualStyleBackColor = true;
            this.cbxSetAsNull.CheckedChanged += new System.EventHandler(this.cbxSetAsNull_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(447, 271);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(366, 271);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tbpEditGuid
            // 
            this.tbpEditGuid.Controls.Add(this.txtGuid);
            this.tbpEditGuid.Controls.Add(this.label5);
            this.tbpEditGuid.Location = new System.Drawing.Point(4, 22);
            this.tbpEditGuid.Name = "tbpEditGuid";
            this.tbpEditGuid.Padding = new System.Windows.Forms.Padding(3);
            this.tbpEditGuid.Size = new System.Drawing.Size(506, 205);
            this.tbpEditGuid.TabIndex = 8;
            this.tbpEditGuid.Text = "Unique Identifier";
            this.tbpEditGuid.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Value:";
            // 
            // txtGuid
            // 
            this.txtGuid.AsciiOnly = true;
            this.txtGuid.Location = new System.Drawing.Point(66, 15);
            this.txtGuid.Name = "txtGuid";
            this.txtGuid.Size = new System.Drawing.Size(228, 20);
            this.txtGuid.TabIndex = 9;
            this.txtGuid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGuid_KeyDown);
            this.txtGuid.TextChanged += new System.EventHandler(this.txtGuid_TextChanged);
            // 
            // CellValueEditorDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(537, 304);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cbxSetAsNull);
            this.Controls.Add(this.tbcTypes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(463, 328);
            this.Name = "CellValueEditorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.CellValueEditorDialog_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CellValueEditorDialog_FormClosing);
            this.tbcTypes.ResumeLayout(false);
            this.tbpEditInteger.ResumeLayout(false);
            this.tbpEditInteger.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInteger)).EndInit();
            this.tbpEditFloatingPoint.ResumeLayout(false);
            this.tbpEditFloatingPoint.PerformLayout();
            this.tbpEditBoolean.ResumeLayout(false);
            this.tbpEditBoolean.PerformLayout();
            this.tbpEditText.ResumeLayout(false);
            this.tbpEditText.PerformLayout();
            this.tbpEditBlob.ResumeLayout(false);
            this.tbpEditBlob.PerformLayout();
            this.tbpEditDateTime.ResumeLayout(false);
            this.tbpEditDateTime.PerformLayout();
            this.pnlDateTimeWarning.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tbpEditGuid.ResumeLayout(false);
            this.tbpEditGuid.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tbcTypes;
        private System.Windows.Forms.TabPage tbpEditInteger;
        private System.Windows.Forms.TabPage tbpEditFloatingPoint;
        private System.Windows.Forms.TabPage tbpEditText;
        private System.Windows.Forms.TabPage tbpEditBlob;
        private System.Windows.Forms.TabPage tbpEditBoolean;
        private System.Windows.Forms.CheckBox cbxSetAsNull;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numInteger;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbtnFalse;
        private System.Windows.Forms.RadioButton rbtnTrue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label8;
        private Be.Windows.Forms.HexBox ucHexEditor;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtFloatingPoint;
        private System.Windows.Forms.TabPage tbpEditDateTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblBlobSize;
        private System.Windows.Forms.Panel pnlDateTimeWarning;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabPage tbpEditGuid;
        private System.Windows.Forms.MaskedTextBox txtGuid;
        private System.Windows.Forms.Label label5;
    }
}