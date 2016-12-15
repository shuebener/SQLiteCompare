namespace SQLiteTurbo
{
    partial class SearchDataRowsDialog
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
            this.tbcSearch = new System.Windows.Forms.TabControl();
            this.tbpExact = new System.Windows.Forms.TabPage();
            this.cbxValue = new System.Windows.Forms.CheckBox();
            this.pnlValues = new Liron.Windows.Forms.MultiPanel();
            this.pnlTextValue = new Liron.Windows.Forms.MultiPanelPage();
            this.txtFieldValue = new System.Windows.Forms.TextBox();
            this.pnlIntegerValue = new Liron.Windows.Forms.MultiPanelPage();
            this.numIntValue = new System.Windows.Forms.NumericUpDown();
            this.pnlDateTimeValue = new Liron.Windows.Forms.MultiPanelPage();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.pnlFloatingPoint = new Liron.Windows.Forms.MultiPanelPage();
            this.numFloatingPoint = new System.Windows.Forms.NumericUpDown();
            this.pnlBooleanValue = new Liron.Windows.Forms.MultiPanelPage();
            this.rbtnFalse = new System.Windows.Forms.RadioButton();
            this.rbtnTrue = new System.Windows.Forms.RadioButton();
            this.pnlGuidValue = new Liron.Windows.Forms.MultiPanelPage();
            this.txtGuid = new System.Windows.Forms.MaskedTextBox();
            this.pnlBlobValue = new Liron.Windows.Forms.MultiPanelPage();
            this.pnlNull = new Liron.Windows.Forms.MultiPanelPage();
            this.cboColumnName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbpSQL = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSQL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBlob = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbcSearch.SuspendLayout();
            this.tbpExact.SuspendLayout();
            this.pnlValues.SuspendLayout();
            this.pnlTextValue.SuspendLayout();
            this.pnlIntegerValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIntValue)).BeginInit();
            this.pnlDateTimeValue.SuspendLayout();
            this.pnlFloatingPoint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFloatingPoint)).BeginInit();
            this.pnlBooleanValue.SuspendLayout();
            this.pnlGuidValue.SuspendLayout();
            this.pnlBlobValue.SuspendLayout();
            this.pnlNull.SuspendLayout();
            this.tbpSQL.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcSearch
            // 
            this.tbcSearch.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tbcSearch.Controls.Add(this.tbpExact);
            this.tbcSearch.Controls.Add(this.tbpSQL);
            this.tbcSearch.Location = new System.Drawing.Point(12, 12);
            this.tbcSearch.Name = "tbcSearch";
            this.tbcSearch.SelectedIndex = 0;
            this.tbcSearch.Size = new System.Drawing.Size(395, 144);
            this.tbcSearch.TabIndex = 0;
            // 
            // tbpExact
            // 
            this.tbpExact.Controls.Add(this.cbxValue);
            this.tbpExact.Controls.Add(this.pnlValues);
            this.tbpExact.Controls.Add(this.cboColumnName);
            this.tbpExact.Controls.Add(this.label2);
            this.tbpExact.Location = new System.Drawing.Point(4, 25);
            this.tbpExact.Name = "tbpExact";
            this.tbpExact.Padding = new System.Windows.Forms.Padding(3);
            this.tbpExact.Size = new System.Drawing.Size(387, 115);
            this.tbpExact.TabIndex = 0;
            this.tbpExact.Text = "Exact";
            this.tbpExact.UseVisualStyleBackColor = true;
            // 
            // cbxValue
            // 
            this.cbxValue.AutoSize = true;
            this.cbxValue.Checked = true;
            this.cbxValue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxValue.Location = new System.Drawing.Point(18, 67);
            this.cbxValue.Name = "cbxValue";
            this.cbxValue.Size = new System.Drawing.Size(56, 17);
            this.cbxValue.TabIndex = 3;
            this.cbxValue.Text = "Value:";
            this.cbxValue.UseVisualStyleBackColor = true;
            this.cbxValue.CheckedChanged += new System.EventHandler(this.cbxValue_CheckedChanged);
            // 
            // pnlValues
            // 
            this.pnlValues.Controls.Add(this.pnlTextValue);
            this.pnlValues.Controls.Add(this.pnlIntegerValue);
            this.pnlValues.Controls.Add(this.pnlDateTimeValue);
            this.pnlValues.Controls.Add(this.pnlFloatingPoint);
            this.pnlValues.Controls.Add(this.pnlBooleanValue);
            this.pnlValues.Controls.Add(this.pnlGuidValue);
            this.pnlValues.Controls.Add(this.pnlBlobValue);
            this.pnlValues.Controls.Add(this.pnlNull);
            this.pnlValues.Location = new System.Drawing.Point(83, 55);
            this.pnlValues.Name = "pnlValues";
            this.pnlValues.SelectedPage = this.pnlBlobValue;
            this.pnlValues.Size = new System.Drawing.Size(292, 57);
            this.pnlValues.TabIndex = 3;
            this.pnlValues.TabStop = true;
            // 
            // pnlTextValue
            // 
            this.pnlTextValue.Controls.Add(this.txtFieldValue);
            this.pnlTextValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTextValue.Location = new System.Drawing.Point(0, 0);
            this.pnlTextValue.Name = "pnlTextValue";
            this.pnlTextValue.Size = new System.Drawing.Size(292, 57);
            this.pnlTextValue.TabIndex = 0;
            this.pnlTextValue.Text = "text value";
            // 
            // txtFieldValue
            // 
            this.txtFieldValue.Location = new System.Drawing.Point(12, 10);
            this.txtFieldValue.Name = "txtFieldValue";
            this.txtFieldValue.Size = new System.Drawing.Size(271, 20);
            this.txtFieldValue.TabIndex = 0;
            // 
            // pnlIntegerValue
            // 
            this.pnlIntegerValue.Controls.Add(this.numIntValue);
            this.pnlIntegerValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlIntegerValue.Location = new System.Drawing.Point(0, 0);
            this.pnlIntegerValue.Name = "pnlIntegerValue";
            this.pnlIntegerValue.Size = new System.Drawing.Size(292, 57);
            this.pnlIntegerValue.TabIndex = 1;
            this.pnlIntegerValue.Text = "integer value";
            // 
            // numIntValue
            // 
            this.numIntValue.Location = new System.Drawing.Point(12, 10);
            this.numIntValue.Name = "numIntValue";
            this.numIntValue.Size = new System.Drawing.Size(187, 20);
            this.numIntValue.TabIndex = 0;
            // 
            // pnlDateTimeValue
            // 
            this.pnlDateTimeValue.Controls.Add(this.dtpDate);
            this.pnlDateTimeValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDateTimeValue.Location = new System.Drawing.Point(0, 0);
            this.pnlDateTimeValue.Name = "pnlDateTimeValue";
            this.pnlDateTimeValue.Size = new System.Drawing.Size(292, 57);
            this.pnlDateTimeValue.TabIndex = 2;
            this.pnlDateTimeValue.Text = "datetime value";
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(12, 10);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.ShowUpDown = true;
            this.dtpDate.Size = new System.Drawing.Size(90, 20);
            this.dtpDate.TabIndex = 0;
            // 
            // pnlFloatingPoint
            // 
            this.pnlFloatingPoint.Controls.Add(this.numFloatingPoint);
            this.pnlFloatingPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFloatingPoint.Location = new System.Drawing.Point(0, 0);
            this.pnlFloatingPoint.Name = "pnlFloatingPoint";
            this.pnlFloatingPoint.Size = new System.Drawing.Size(292, 57);
            this.pnlFloatingPoint.TabIndex = 3;
            this.pnlFloatingPoint.Text = "floating point";
            // 
            // numFloatingPoint
            // 
            this.numFloatingPoint.DecimalPlaces = 16;
            this.numFloatingPoint.Location = new System.Drawing.Point(12, 10);
            this.numFloatingPoint.Name = "numFloatingPoint";
            this.numFloatingPoint.Size = new System.Drawing.Size(271, 20);
            this.numFloatingPoint.TabIndex = 0;
            // 
            // pnlBooleanValue
            // 
            this.pnlBooleanValue.Controls.Add(this.rbtnFalse);
            this.pnlBooleanValue.Controls.Add(this.rbtnTrue);
            this.pnlBooleanValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBooleanValue.Location = new System.Drawing.Point(0, 0);
            this.pnlBooleanValue.Name = "pnlBooleanValue";
            this.pnlBooleanValue.Size = new System.Drawing.Size(292, 57);
            this.pnlBooleanValue.TabIndex = 4;
            this.pnlBooleanValue.Text = "boolean value";
            // 
            // rbtnFalse
            // 
            this.rbtnFalse.AutoSize = true;
            this.rbtnFalse.Location = new System.Drawing.Point(86, 10);
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
            this.rbtnTrue.Location = new System.Drawing.Point(12, 10);
            this.rbtnTrue.Name = "rbtnTrue";
            this.rbtnTrue.Size = new System.Drawing.Size(55, 17);
            this.rbtnTrue.TabIndex = 0;
            this.rbtnTrue.TabStop = true;
            this.rbtnTrue.Text = "TRUE";
            this.rbtnTrue.UseVisualStyleBackColor = true;
            // 
            // pnlGuidValue
            // 
            this.pnlGuidValue.Controls.Add(this.txtGuid);
            this.pnlGuidValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGuidValue.Location = new System.Drawing.Point(0, 0);
            this.pnlGuidValue.Name = "pnlGuidValue";
            this.pnlGuidValue.Size = new System.Drawing.Size(292, 57);
            this.pnlGuidValue.TabIndex = 5;
            this.pnlGuidValue.Text = "guid value";
            // 
            // txtGuid
            // 
            this.txtGuid.AsciiOnly = true;
            this.txtGuid.Location = new System.Drawing.Point(12, 7);
            this.txtGuid.Name = "txtGuid";
            this.txtGuid.Size = new System.Drawing.Size(225, 20);
            this.txtGuid.TabIndex = 10;
            this.txtGuid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGuid_KeyDown);
            this.txtGuid.TextChanged += new System.EventHandler(this.txtGuid_TextChanged);
            // 
            // pnlBlobValue
            // 
            this.pnlBlobValue.Controls.Add(this.label5);
            this.pnlBlobValue.Controls.Add(this.txtBlob);
            this.pnlBlobValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBlobValue.Location = new System.Drawing.Point(0, 0);
            this.pnlBlobValue.Name = "pnlBlobValue";
            this.pnlBlobValue.Size = new System.Drawing.Size(292, 57);
            this.pnlBlobValue.TabIndex = 6;
            this.pnlBlobValue.Text = "blob value";
            // 
            // pnlNull
            // 
            this.pnlNull.Controls.Add(this.label3);
            this.pnlNull.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNull.Location = new System.Drawing.Point(0, 0);
            this.pnlNull.Name = "pnlNull";
            this.pnlNull.Size = new System.Drawing.Size(292, 57);
            this.pnlNull.TabIndex = 7;
            this.pnlNull.Text = "NULL value";
            // 
            // cboColumnName
            // 
            this.cboColumnName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboColumnName.FormattingEnabled = true;
            this.cboColumnName.Location = new System.Drawing.Point(95, 28);
            this.cboColumnName.Name = "cboColumnName";
            this.cboColumnName.Size = new System.Drawing.Size(271, 21);
            this.cboColumnName.TabIndex = 1;
            this.cboColumnName.SelectedIndexChanged += new System.EventHandler(this.cboColumnName_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Column name:";
            // 
            // tbpSQL
            // 
            this.tbpSQL.Controls.Add(this.label4);
            this.tbpSQL.Controls.Add(this.txtSQL);
            this.tbpSQL.Controls.Add(this.label1);
            this.tbpSQL.Location = new System.Drawing.Point(4, 25);
            this.tbpSQL.Name = "tbpSQL";
            this.tbpSQL.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSQL.Size = new System.Drawing.Size(387, 115);
            this.tbpSQL.TabIndex = 1;
            this.tbpSQL.Text = "SQL";
            this.tbpSQL.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(15, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(321, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "Example: \'objID >100 AND FirstName IS NOT NULL\'";
            // 
            // txtSQL
            // 
            this.txtSQL.Location = new System.Drawing.Point(18, 54);
            this.txtSQL.Multiline = true;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSQL.Size = new System.Drawing.Size(352, 50);
            this.txtSQL.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(15, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Type a SQL WHERE clause";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(328, 162);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(247, 162);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label3.Location = new System.Drawing.Point(10, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "NULL";
            // 
            // txtBlob
            // 
            this.txtBlob.Location = new System.Drawing.Point(12, 10);
            this.txtBlob.Name = "txtBlob";
            this.txtBlob.Size = new System.Drawing.Size(271, 20);
            this.txtBlob.TabIndex = 1;
            this.txtBlob.TextChanged += new System.EventHandler(this.txtBlob_TextChanged);
            this.txtBlob.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBlob_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label5.Location = new System.Drawing.Point(9, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(215, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Use hexadecimal format (e.g., \'aa5f\')";
            // 
            // SearchDataRowsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(418, 195);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tbcSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchDataRowsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search Rows";
            this.Shown += new System.EventHandler(this.SearchDataRowsDialog_Shown);
            this.tbcSearch.ResumeLayout(false);
            this.tbpExact.ResumeLayout(false);
            this.tbpExact.PerformLayout();
            this.pnlValues.ResumeLayout(false);
            this.pnlTextValue.ResumeLayout(false);
            this.pnlTextValue.PerformLayout();
            this.pnlIntegerValue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numIntValue)).EndInit();
            this.pnlDateTimeValue.ResumeLayout(false);
            this.pnlFloatingPoint.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numFloatingPoint)).EndInit();
            this.pnlBooleanValue.ResumeLayout(false);
            this.pnlBooleanValue.PerformLayout();
            this.pnlGuidValue.ResumeLayout(false);
            this.pnlGuidValue.PerformLayout();
            this.pnlBlobValue.ResumeLayout(false);
            this.pnlBlobValue.PerformLayout();
            this.pnlNull.ResumeLayout(false);
            this.pnlNull.PerformLayout();
            this.tbpSQL.ResumeLayout(false);
            this.tbpSQL.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcSearch;
        private System.Windows.Forms.TabPage tbpExact;
        private System.Windows.Forms.TabPage tbpSQL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.ComboBox cboColumnName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private Liron.Windows.Forms.MultiPanel pnlValues;
        private Liron.Windows.Forms.MultiPanelPage pnlTextValue;
        private System.Windows.Forms.TextBox txtFieldValue;
        private Liron.Windows.Forms.MultiPanelPage pnlIntegerValue;
        private System.Windows.Forms.NumericUpDown numIntValue;
        private Liron.Windows.Forms.MultiPanelPage pnlDateTimeValue;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private Liron.Windows.Forms.MultiPanelPage pnlFloatingPoint;
        private System.Windows.Forms.NumericUpDown numFloatingPoint;
        private Liron.Windows.Forms.MultiPanelPage pnlBooleanValue;
        private System.Windows.Forms.RadioButton rbtnFalse;
        private System.Windows.Forms.RadioButton rbtnTrue;
        private Liron.Windows.Forms.MultiPanelPage pnlGuidValue;
        private System.Windows.Forms.MaskedTextBox txtGuid;
        private Liron.Windows.Forms.MultiPanelPage pnlBlobValue;
        private System.Windows.Forms.CheckBox cbxValue;
        private Liron.Windows.Forms.MultiPanelPage pnlNull;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBlob;

    }
}