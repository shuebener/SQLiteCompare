namespace SQLiteTurbo
{
    partial class TwoWayCompareEditDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TwoWayCompareEditDialog));
            this.tbcViews = new System.Windows.Forms.TabControl();
            this.tbpSchema = new System.Windows.Forms.TabPage();
            this.ucDiff = new DiffControl.DualDiffControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnUpdateSchema = new System.Windows.Forms.ToolStripButton();
            this.btnCompareData = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.btnClearAllChanges = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnReorderColumns = new System.Windows.Forms.ToolStripButton();
            this.tbpData = new System.Windows.Forms.TabPage();
            this.ucTableDiff = new SQLiteTurbo.TableDiffControl();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnRefreshComparison = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExistsInLeft = new System.Windows.Forms.ToolStripButton();
            this.btnExistsInRight = new System.Windows.Forms.ToolStripButton();
            this.btnDifferent = new System.Windows.Forms.ToolStripButton();
            this.btnSame = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSearchData = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExportDifferences = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.tbcViews.SuspendLayout();
            this.tbpSchema.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tbpData.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcViews
            // 
            this.tbcViews.Controls.Add(this.tbpSchema);
            this.tbcViews.Controls.Add(this.tbpData);
            this.tbcViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcViews.Location = new System.Drawing.Point(0, 49);
            this.tbcViews.Name = "tbcViews";
            this.tbcViews.SelectedIndex = 0;
            this.tbcViews.Size = new System.Drawing.Size(875, 432);
            this.tbcViews.TabIndex = 0;
            // 
            // tbpSchema
            // 
            this.tbpSchema.Controls.Add(this.ucDiff);
            this.tbpSchema.Controls.Add(this.toolStrip1);
            this.tbpSchema.Location = new System.Drawing.Point(4, 22);
            this.tbpSchema.Name = "tbpSchema";
            this.tbpSchema.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSchema.Size = new System.Drawing.Size(867, 406);
            this.tbpSchema.TabIndex = 0;
            this.tbpSchema.Text = "Schema";
            this.tbpSchema.UseVisualStyleBackColor = true;
            // 
            // ucDiff
            // 
            this.ucDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucDiff.Location = new System.Drawing.Point(6, 35);
            this.ucDiff.Name = "ucDiff";
            this.ucDiff.Size = new System.Drawing.Size(855, 366);
            this.ucDiff.TabIndex = 1;
            this.ucDiff.RightSaveRequested += new System.EventHandler(this.ucDiff_RightSaveRequested);
            this.ucDiff.LeftSaveRequested += new System.EventHandler(this.ucDiff_LeftSaveRequested);
            this.ucDiff.UndoStateChanged += new System.EventHandler(this.ucDiff_UndoStateChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUpdateSchema,
            this.btnCompareData,
            this.toolStripSeparator1,
            this.btnUndo,
            this.btnRedo,
            this.btnClearAllChanges,
            this.toolStripSeparator3,
            this.btnReorderColumns});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(861, 31);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnUpdateSchema
            // 
            this.btnUpdateSchema.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdateSchema.Image")));
            this.btnUpdateSchema.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnUpdateSchema.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUpdateSchema.Name = "btnUpdateSchema";
            this.btnUpdateSchema.Size = new System.Drawing.Size(109, 28);
            this.btnUpdateSchema.Text = "Update schema";
            this.btnUpdateSchema.Click += new System.EventHandler(this.btnUpdateSchema_Click);
            // 
            // btnCompareData
            // 
            this.btnCompareData.Image = ((System.Drawing.Image)(resources.GetObject("btnCompareData.Image")));
            this.btnCompareData.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnCompareData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompareData.Name = "btnCompareData";
            this.btnCompareData.Size = new System.Drawing.Size(103, 28);
            this.btnCompareData.Text = "Compare data";
            this.btnCompareData.Visible = false;
            this.btnCompareData.Click += new System.EventHandler(this.btnCompareData_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // btnUndo
            // 
            this.btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.Image")));
            this.btnUndo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(60, 28);
            this.btnUndo.Text = "Undo";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.Image = ((System.Drawing.Image)(resources.GetObject("btnRedo.Image")));
            this.btnRedo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(60, 28);
            this.btnRedo.Text = "Redo";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnClearAllChanges
            // 
            this.btnClearAllChanges.Image = ((System.Drawing.Image)(resources.GetObject("btnClearAllChanges.Image")));
            this.btnClearAllChanges.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnClearAllChanges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearAllChanges.Name = "btnClearAllChanges";
            this.btnClearAllChanges.Size = new System.Drawing.Size(103, 28);
            this.btnClearAllChanges.Text = "Clear changes";
            this.btnClearAllChanges.Click += new System.EventHandler(this.btnClearAllChanges_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
            // 
            // btnReorderColumns
            // 
            this.btnReorderColumns.Image = ((System.Drawing.Image)(resources.GetObject("btnReorderColumns.Image")));
            this.btnReorderColumns.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnReorderColumns.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReorderColumns.Name = "btnReorderColumns";
            this.btnReorderColumns.Size = new System.Drawing.Size(115, 28);
            this.btnReorderColumns.Text = "Reorder columns";
            this.btnReorderColumns.Click += new System.EventHandler(this.btnReorderColumns_Click);
            // 
            // tbpData
            // 
            this.tbpData.Controls.Add(this.ucTableDiff);
            this.tbpData.Controls.Add(this.toolStrip2);
            this.tbpData.Location = new System.Drawing.Point(4, 22);
            this.tbpData.Name = "tbpData";
            this.tbpData.Padding = new System.Windows.Forms.Padding(3);
            this.tbpData.Size = new System.Drawing.Size(867, 406);
            this.tbpData.TabIndex = 1;
            this.tbpData.Text = "Data";
            this.tbpData.UseVisualStyleBackColor = true;
            // 
            // ucTableDiff
            // 
            this.ucTableDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucTableDiff.Location = new System.Drawing.Point(3, 37);
            this.ucTableDiff.Name = "ucTableDiff";
            this.ucTableDiff.Size = new System.Drawing.Size(858, 363);
            this.ucTableDiff.TabIndex = 3;
            this.ucTableDiff.SearchRequested += new System.EventHandler(this.ucTableDiff_SearchRequested);
            this.ucTableDiff.StateChanged += new System.EventHandler(this.ucTableDiff_StateChanged);
            this.ucTableDiff.RowsChanged += new System.EventHandler(this.ucTableDiff_RowsChanged);
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefreshComparison,
            this.toolStripSeparator2,
            this.btnExistsInLeft,
            this.btnExistsInRight,
            this.btnDifferent,
            this.btnSame,
            this.toolStripSeparator4,
            this.btnSearchData,
            this.toolStripSeparator5,
            this.btnExportDifferences});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(861, 31);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnRefreshComparison
            // 
            this.btnRefreshComparison.Image = ((System.Drawing.Image)(resources.GetObject("btnRefreshComparison.Image")));
            this.btnRefreshComparison.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRefreshComparison.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefreshComparison.Name = "btnRefreshComparison";
            this.btnRefreshComparison.Size = new System.Drawing.Size(130, 28);
            this.btnRefreshComparison.Text = "Refresh comparison";
            this.btnRefreshComparison.Click += new System.EventHandler(this.btnRefreshComparison_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // btnExistsInLeft
            // 
            this.btnExistsInLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnExistsInLeft.Image")));
            this.btnExistsInLeft.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnExistsInLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExistsInLeft.Name = "btnExistsInLeft";
            this.btnExistsInLeft.Size = new System.Drawing.Size(28, 28);
            this.btnExistsInLeft.ToolTipText = "Show rows that exist only in the left database table";
            this.btnExistsInLeft.Click += new System.EventHandler(this.btnExistsInLeft_Click);
            // 
            // btnExistsInRight
            // 
            this.btnExistsInRight.Image = ((System.Drawing.Image)(resources.GetObject("btnExistsInRight.Image")));
            this.btnExistsInRight.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnExistsInRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExistsInRight.Name = "btnExistsInRight";
            this.btnExistsInRight.Size = new System.Drawing.Size(28, 28);
            this.btnExistsInRight.ToolTipText = "Show rows that exist only in the right database table";
            this.btnExistsInRight.Click += new System.EventHandler(this.btnExistsInRight_Click);
            // 
            // btnDifferent
            // 
            this.btnDifferent.Checked = true;
            this.btnDifferent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnDifferent.Image = ((System.Drawing.Image)(resources.GetObject("btnDifferent.Image")));
            this.btnDifferent.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnDifferent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDifferent.Name = "btnDifferent";
            this.btnDifferent.Size = new System.Drawing.Size(28, 28);
            this.btnDifferent.ToolTipText = "Show different rows";
            this.btnDifferent.Click += new System.EventHandler(this.btnDifferent_Click);
            // 
            // btnSame
            // 
            this.btnSame.Image = ((System.Drawing.Image)(resources.GetObject("btnSame.Image")));
            this.btnSame.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSame.Name = "btnSame";
            this.btnSame.Size = new System.Drawing.Size(28, 28);
            this.btnSame.ToolTipText = "Show equal rows";
            this.btnSame.Click += new System.EventHandler(this.btnSame_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
            // 
            // btnSearchData
            // 
            this.btnSearchData.Image = ((System.Drawing.Image)(resources.GetObject("btnSearchData.Image")));
            this.btnSearchData.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSearchData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSearchData.Name = "btnSearchData";
            this.btnSearchData.Size = new System.Drawing.Size(80, 28);
            this.btnSearchData.Text = "Search...";
            this.btnSearchData.ToolTipText = "Search data rows (CTRL-F)";
            this.btnSearchData.Click += new System.EventHandler(this.btnSearchData_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
            // 
            // btnExportDifferences
            // 
            this.btnExportDifferences.Image = ((System.Drawing.Image)(resources.GetObject("btnExportDifferences.Image")));
            this.btnExportDifferences.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnExportDifferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportDifferences.Name = "btnExportDifferences";
            this.btnExportDifferences.Size = new System.Drawing.Size(136, 28);
            this.btnExportDifferences.Text = "Export differences...";
            this.btnExportDifferences.ToolTipText = "Search data rows (CTRL-F)";
            this.btnExportDifferences.Click += new System.EventHandler(this.btnExportDifferences_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "key.png");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblErrorMessage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(875, 49);
            this.panel1.TabIndex = 1;
            this.panel1.Visible = false;
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblErrorMessage.BackColor = System.Drawing.Color.Maroon;
            this.lblErrorMessage.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblErrorMessage.ForeColor = System.Drawing.Color.White;
            this.lblErrorMessage.Location = new System.Drawing.Point(0, 0);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size(875, 42);
            this.lblErrorMessage.TabIndex = 2;
            this.lblErrorMessage.Text = "ERROR: data comparison failed (xxxxx)";
            this.lblErrorMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TwoWayCompareEditDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 481);
            this.Controls.Add(this.tbcViews);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TwoWayCompareEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Table [MyCalls]";
            this.Load += new System.EventHandler(this.TwoWayCompareEditDialog_Load);
            this.Shown += new System.EventHandler(this.TwoWayCompareEditDialog_Shown);
            this.tbcViews.ResumeLayout(false);
            this.tbpSchema.ResumeLayout(false);
            this.tbpSchema.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tbpData.ResumeLayout(false);
            this.tbpData.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcViews;
        private System.Windows.Forms.TabPage tbpSchema;
        private System.Windows.Forms.TabPage tbpData;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnUpdateSchema;
        private System.Windows.Forms.ToolStripButton btnCompareData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;
        private System.Windows.Forms.ToolStripButton btnClearAllChanges;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btnExistsInLeft;
        private System.Windows.Forms.ToolStripButton btnExistsInRight;
        private System.Windows.Forms.ToolStripButton btnDifferent;
        private System.Windows.Forms.ToolStripButton btnSame;
        private DiffControl.DualDiffControl ucDiff;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripButton btnRefreshComparison;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private TableDiffControl ucTableDiff;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnReorderColumns;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton btnSearchData;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblErrorMessage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton btnExportDifferences;
    }
}