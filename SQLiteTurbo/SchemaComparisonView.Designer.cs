namespace SQLiteTurbo
{
    partial class SchemaComparisonView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemaComparisonView));
            this.grdSchemaDiffs = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLeft = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbxShowTableDifferences = new System.Windows.Forms.CheckBox();
            this.cbxShowIndexDifferences = new System.Windows.Forms.CheckBox();
            this.cbxShowViewDifferences = new System.Windows.Forms.CheckBox();
            this.cbxShowTriggerDifferences = new System.Windows.Forms.CheckBox();
            this.cbxShowOnlyDifferences = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblTableDataIsDifferent = new System.Windows.Forms.Label();
            this.lblTotalFound = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblComparisonError = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grdSchemaDiffs)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdSchemaDiffs
            // 
            this.grdSchemaDiffs.AllowUserToAddRows = false;
            this.grdSchemaDiffs.AllowUserToDeleteRows = false;
            this.grdSchemaDiffs.AllowUserToResizeColumns = false;
            this.grdSchemaDiffs.AllowUserToResizeRows = false;
            this.grdSchemaDiffs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdSchemaDiffs.BackgroundColor = System.Drawing.Color.White;
            this.grdSchemaDiffs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdSchemaDiffs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdSchemaDiffs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.colLeft,
            this.colRight});
            this.grdSchemaDiffs.Location = new System.Drawing.Point(6, 37);
            this.grdSchemaDiffs.MultiSelect = false;
            this.grdSchemaDiffs.Name = "grdSchemaDiffs";
            this.grdSchemaDiffs.ReadOnly = true;
            this.grdSchemaDiffs.RowHeadersVisible = false;
            this.grdSchemaDiffs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdSchemaDiffs.Size = new System.Drawing.Size(856, 503);
            this.grdSchemaDiffs.TabIndex = 0;
            this.grdSchemaDiffs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdSchemaDiffs_CellDoubleClick);
            this.grdSchemaDiffs.SelectionChanged += new System.EventHandler(this.grdSchemaDiffs_SelectionChanged);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.Width = 30;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Type";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colLeft
            // 
            this.colLeft.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colLeft.HeaderText = "C:\\dbfiles\\check1.db";
            this.colLeft.Name = "colLeft";
            this.colLeft.ReadOnly = true;
            this.colLeft.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colRight
            // 
            this.colRight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRight.HeaderText = "C:\\dbfiles\\check3.db";
            this.colRight.Name = "colRight";
            this.colRight.ReadOnly = true;
            this.colRight.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cbxShowTableDifferences
            // 
            this.cbxShowTableDifferences.AutoSize = true;
            this.cbxShowTableDifferences.Checked = true;
            this.cbxShowTableDifferences.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxShowTableDifferences.Location = new System.Drawing.Point(159, 11);
            this.cbxShowTableDifferences.Name = "cbxShowTableDifferences";
            this.cbxShowTableDifferences.Size = new System.Drawing.Size(58, 17);
            this.cbxShowTableDifferences.TabIndex = 1;
            this.cbxShowTableDifferences.Text = "Tables";
            this.cbxShowTableDifferences.UseVisualStyleBackColor = true;
            this.cbxShowTableDifferences.CheckedChanged += new System.EventHandler(this.cbxShowTableDifferences_CheckedChanged);
            // 
            // cbxShowIndexDifferences
            // 
            this.cbxShowIndexDifferences.AutoSize = true;
            this.cbxShowIndexDifferences.Checked = true;
            this.cbxShowIndexDifferences.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxShowIndexDifferences.Location = new System.Drawing.Point(223, 11);
            this.cbxShowIndexDifferences.Name = "cbxShowIndexDifferences";
            this.cbxShowIndexDifferences.Size = new System.Drawing.Size(63, 17);
            this.cbxShowIndexDifferences.TabIndex = 2;
            this.cbxShowIndexDifferences.Text = "Indexes";
            this.cbxShowIndexDifferences.UseVisualStyleBackColor = true;
            this.cbxShowIndexDifferences.CheckedChanged += new System.EventHandler(this.cbxShowIndexDifferences_CheckedChanged);
            // 
            // cbxShowViewDifferences
            // 
            this.cbxShowViewDifferences.AutoSize = true;
            this.cbxShowViewDifferences.Checked = true;
            this.cbxShowViewDifferences.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxShowViewDifferences.Location = new System.Drawing.Point(292, 11);
            this.cbxShowViewDifferences.Name = "cbxShowViewDifferences";
            this.cbxShowViewDifferences.Size = new System.Drawing.Size(54, 17);
            this.cbxShowViewDifferences.TabIndex = 3;
            this.cbxShowViewDifferences.Text = "Views";
            this.cbxShowViewDifferences.UseVisualStyleBackColor = true;
            this.cbxShowViewDifferences.CheckedChanged += new System.EventHandler(this.cbxShowViewDifferences_CheckedChanged);
            // 
            // cbxShowTriggerDifferences
            // 
            this.cbxShowTriggerDifferences.AutoSize = true;
            this.cbxShowTriggerDifferences.Checked = true;
            this.cbxShowTriggerDifferences.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxShowTriggerDifferences.Location = new System.Drawing.Point(352, 11);
            this.cbxShowTriggerDifferences.Name = "cbxShowTriggerDifferences";
            this.cbxShowTriggerDifferences.Size = new System.Drawing.Size(64, 17);
            this.cbxShowTriggerDifferences.TabIndex = 4;
            this.cbxShowTriggerDifferences.Text = "Triggers";
            this.cbxShowTriggerDifferences.UseVisualStyleBackColor = true;
            this.cbxShowTriggerDifferences.CheckedChanged += new System.EventHandler(this.cbxShowTriggerDifferences_CheckedChanged);
            // 
            // cbxShowOnlyDifferences
            // 
            this.cbxShowOnlyDifferences.AutoSize = true;
            this.cbxShowOnlyDifferences.Location = new System.Drawing.Point(6, 11);
            this.cbxShowOnlyDifferences.Name = "cbxShowOnlyDifferences";
            this.cbxShowOnlyDifferences.Size = new System.Drawing.Size(130, 17);
            this.cbxShowOnlyDifferences.TabIndex = 5;
            this.cbxShowOnlyDifferences.Text = "Show differences only";
            this.cbxShowOnlyDifferences.UseVisualStyleBackColor = true;
            this.cbxShowOnlyDifferences.CheckedChanged += new System.EventHandler(this.cbxShowOnlyDifferences_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.DarkGray;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label2.Location = new System.Drawing.Point(68, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 18);
            this.label2.TabIndex = 10;
            this.label2.Text = "Object does not exist in DB";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.Color.Khaki;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(226, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 18);
            this.label3.TabIndex = 11;
            this.label3.Text = "Object schema is different";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(16, 3);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(46, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "Legend:";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "db_table");
            this.imageList1.Images.SetKeyName(1, "db_trigger");
            this.imageList1.Images.SetKeyName(2, "db_view");
            this.imageList1.Images.SetKeyName(3, "db_index");
            // 
            // lblTableDataIsDifferent
            // 
            this.lblTableDataIsDifferent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTableDataIsDifferent.BackColor = System.Drawing.Color.LightBlue;
            this.lblTableDataIsDifferent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTableDataIsDifferent.Location = new System.Drawing.Point(372, 0);
            this.lblTableDataIsDifferent.Name = "lblTableDataIsDifferent";
            this.lblTableDataIsDifferent.Size = new System.Drawing.Size(140, 18);
            this.lblTableDataIsDifferent.TabIndex = 13;
            this.lblTableDataIsDifferent.Text = "Table data is different";
            this.lblTableDataIsDifferent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalFound
            // 
            this.lblTotalFound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalFound.AutoSize = true;
            this.lblTotalFound.Location = new System.Drawing.Point(3, 549);
            this.lblTotalFound.Name = "lblTotalFound";
            this.lblTotalFound.Size = new System.Drawing.Size(46, 13);
            this.lblTotalFound.TabIndex = 14;
            this.lblTotalFound.Text = "Legend:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(464, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Filter:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(501, 9);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(183, 20);
            this.txtSearch.TabIndex = 16;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.lblComparisonError);
            this.flowLayoutPanel1.Controls.Add(this.lblTableDataIsDifferent);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(204, 546);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(661, 20);
            this.flowLayoutPanel1.TabIndex = 17;
            // 
            // lblComparisonError
            // 
            this.lblComparisonError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblComparisonError.BackColor = System.Drawing.Color.LightCoral;
            this.lblComparisonError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblComparisonError.Location = new System.Drawing.Point(518, 0);
            this.lblComparisonError.Name = "lblComparisonError";
            this.lblComparisonError.Size = new System.Drawing.Size(140, 18);
            this.lblComparisonError.TabIndex = 14;
            this.lblComparisonError.Text = "Comparison error";
            this.lblComparisonError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SchemaComparisonView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblTotalFound);
            this.Controls.Add(this.cbxShowOnlyDifferences);
            this.Controls.Add(this.cbxShowTriggerDifferences);
            this.Controls.Add(this.cbxShowViewDifferences);
            this.Controls.Add(this.cbxShowIndexDifferences);
            this.Controls.Add(this.cbxShowTableDifferences);
            this.Controls.Add(this.grdSchemaDiffs);
            this.Name = "SchemaComparisonView";
            this.Size = new System.Drawing.Size(865, 569);
            ((System.ComponentModel.ISupportInitialize)(this.grdSchemaDiffs)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView grdSchemaDiffs;
        private System.Windows.Forms.CheckBox cbxShowTableDifferences;
        private System.Windows.Forms.CheckBox cbxShowIndexDifferences;
        private System.Windows.Forms.CheckBox cbxShowViewDifferences;
        private System.Windows.Forms.CheckBox cbxShowTriggerDifferences;
        private System.Windows.Forms.CheckBox cbxShowOnlyDifferences;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lblTableDataIsDifferent;
        private System.Windows.Forms.DataGridViewImageColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLeft;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRight;
        private System.Windows.Forms.Label lblTotalFound;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblComparisonError;
    }
}
