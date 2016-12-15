namespace SQLiteTurbo
{
    partial class TableDiffControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableDiffControl));
            this.scbRight = new System.Windows.Forms.VScrollBar();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel5 = new System.Windows.Forms.Panel();
            this.grdLeft = new FastGridApp.FastGrid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblLeftPath = new System.Windows.Forms.Label();
            this.lblLeftEdit = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.grdRight = new FastGridApp.FastGrid();
            this.panel3 = new System.Windows.Forms.Panel();
            this.selectionBar1 = new SQLiteTurbo.SelectionBar();
            this.btnEditRow = new System.Windows.Forms.Button();
            this.btnDeleteRows = new System.Windows.Forms.Button();
            this.btnCopyRightToLeft = new System.Windows.Forms.Button();
            this.btnCopyLeftToRight = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblRightPath = new System.Windows.Forms.Label();
            this.lblRightEdit = new System.Windows.Forms.Label();
            this.scbHorizontal = new System.Windows.Forms.HScrollBar();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scbRight
            // 
            this.scbRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.scbRight.Location = new System.Drawing.Point(370, 0);
            this.scbRight.Name = "scbRight";
            this.scbRight.Size = new System.Drawing.Size(17, 327);
            this.scbRight.TabIndex = 1;
            this.scbRight.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scbRight_Scroll);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "key.gif");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel5);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(820, 356);
            this.splitContainer1.SplitterDistance = 387;
            this.splitContainer1.TabIndex = 4;
            this.splitContainer1.TabStop = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.grdLeft);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 29);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(387, 327);
            this.panel5.TabIndex = 2;
            // 
            // grdLeft
            // 
            this.grdLeft.BackColor = System.Drawing.Color.White;
            this.grdLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdLeft.Location = new System.Drawing.Point(0, 0);
            this.grdLeft.Name = "grdLeft";
            this.grdLeft.Size = new System.Drawing.Size(387, 327);
            this.grdLeft.TabIndex = 0;
            this.grdLeft.Text = "fastGrid1";
            this.grdLeft.SearchRequested += new System.EventHandler(this.grdLeft_SearchRequested);
            this.grdLeft.SelectionChanged += new System.EventHandler(this.grdLeft_SelectionChanged);
            this.grdLeft.EditStarted += new System.EventHandler(this.grdLeft_EditStarted);
            this.grdLeft.ColumnResized += new FastGridApp.ColumnResizedEventHandler(this.grdLeft_ColumnResized);
            this.grdLeft.RowNeeded += new FastGridApp.RowNeededEventHandler(this.grdLeft_RowNeeded);
            this.grdLeft.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.grdLeft_MouseDoubleClick);
            this.grdLeft.LayoutChanged += new System.EventHandler(this.grdLeft_LayoutChanged);
            this.grdLeft.Scroll += new System.EventHandler(this.grdLeft_Scroll);
            this.grdLeft.Enter += new System.EventHandler(this.grdLeft_Enter);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblLeftPath);
            this.panel2.Controls.Add(this.lblLeftEdit);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(387, 29);
            this.panel2.TabIndex = 1;
            // 
            // lblLeftPath
            // 
            this.lblLeftPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeftPath.AutoEllipsis = true;
            this.lblLeftPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLeftPath.Location = new System.Drawing.Point(23, 4);
            this.lblLeftPath.Name = "lblLeftPath";
            this.lblLeftPath.Size = new System.Drawing.Size(362, 20);
            this.lblLeftPath.TabIndex = 3;
            this.lblLeftPath.Text = "C:\\files\\left.db";
            this.lblLeftPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLeftEdit
            // 
            this.lblLeftEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLeftEdit.ImageList = this.imageList1;
            this.lblLeftEdit.Location = new System.Drawing.Point(-2, 4);
            this.lblLeftEdit.Name = "lblLeftEdit";
            this.lblLeftEdit.Size = new System.Drawing.Size(22, 20);
            this.lblLeftEdit.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.grdRight);
            this.panel4.Controls.Add(this.scbRight);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(42, 29);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(387, 327);
            this.panel4.TabIndex = 2;
            // 
            // grdRight
            // 
            this.grdRight.BackColor = System.Drawing.Color.White;
            this.grdRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdRight.Location = new System.Drawing.Point(0, 0);
            this.grdRight.Name = "grdRight";
            this.grdRight.Size = new System.Drawing.Size(370, 327);
            this.grdRight.TabIndex = 2;
            this.grdRight.Text = "fastGrid1";
            this.grdRight.SearchRequested += new System.EventHandler(this.grdRight_SearchRequested);
            this.grdRight.SelectionChanged += new System.EventHandler(this.grdRight_SelectionChanged);
            this.grdRight.EditStarted += new System.EventHandler(this.grdRight_EditStarted);
            this.grdRight.ColumnResized += new FastGridApp.ColumnResizedEventHandler(this.grdRight_ColumnResized);
            this.grdRight.RowNeeded += new FastGridApp.RowNeededEventHandler(this.grdRight_RowNeeded);
            this.grdRight.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.grdRight_MouseDoubleClick);
            this.grdRight.LayoutChanged += new System.EventHandler(this.grdRight_LayoutChanged);
            this.grdRight.Scroll += new System.EventHandler(this.grdRight_Scroll);
            this.grdRight.Enter += new System.EventHandler(this.grdRight_Enter);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.selectionBar1);
            this.panel3.Controls.Add(this.btnEditRow);
            this.panel3.Controls.Add(this.btnDeleteRows);
            this.panel3.Controls.Add(this.btnCopyRightToLeft);
            this.panel3.Controls.Add(this.btnCopyLeftToRight);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 29);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(42, 327);
            this.panel3.TabIndex = 1;
            // 
            // selectionBar1
            // 
            this.selectionBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.selectionBar1.BackColor = System.Drawing.Color.White;
            this.selectionBar1.ForeColor = System.Drawing.Color.DarkGray;
            this.selectionBar1.Location = new System.Drawing.Point(3, 218);
            this.selectionBar1.Name = "selectionBar1";
            this.selectionBar1.Size = new System.Drawing.Size(32, 109);
            this.selectionBar1.TabIndex = 7;
            this.selectionBar1.Text = "selectionBar1";
            // 
            // btnEditRow
            // 
            this.btnEditRow.Image = ((System.Drawing.Image)(resources.GetObject("btnEditRow.Image")));
            this.btnEditRow.Location = new System.Drawing.Point(1, 175);
            this.btnEditRow.Name = "btnEditRow";
            this.btnEditRow.Size = new System.Drawing.Size(36, 37);
            this.btnEditRow.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnEditRow, "Edit selected cell");
            this.btnEditRow.UseVisualStyleBackColor = true;
            this.btnEditRow.Click += new System.EventHandler(this.btnEditRow_Click);
            // 
            // btnDeleteRows
            // 
            this.btnDeleteRows.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteRows.Image")));
            this.btnDeleteRows.Location = new System.Drawing.Point(1, 121);
            this.btnDeleteRows.Name = "btnDeleteRows";
            this.btnDeleteRows.Size = new System.Drawing.Size(36, 37);
            this.btnDeleteRows.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnDeleteRows, "Delete selected rows");
            this.btnDeleteRows.UseVisualStyleBackColor = true;
            this.btnDeleteRows.Click += new System.EventHandler(this.btnDeleteRows_Click);
            // 
            // btnCopyRightToLeft
            // 
            this.btnCopyRightToLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyRightToLeft.Image")));
            this.btnCopyRightToLeft.Location = new System.Drawing.Point(1, 42);
            this.btnCopyRightToLeft.Name = "btnCopyRightToLeft";
            this.btnCopyRightToLeft.Size = new System.Drawing.Size(36, 37);
            this.btnCopyRightToLeft.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnCopyRightToLeft, "Copy selected rows from the right database to the left database");
            this.btnCopyRightToLeft.UseVisualStyleBackColor = true;
            this.btnCopyRightToLeft.Click += new System.EventHandler(this.btnCopyRightToLeft_Click);
            // 
            // btnCopyLeftToRight
            // 
            this.btnCopyLeftToRight.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyLeftToRight.Image")));
            this.btnCopyLeftToRight.Location = new System.Drawing.Point(1, -1);
            this.btnCopyLeftToRight.Name = "btnCopyLeftToRight";
            this.btnCopyLeftToRight.Size = new System.Drawing.Size(36, 37);
            this.btnCopyLeftToRight.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnCopyLeftToRight, "Copy selected rows from the left database to the right database");
            this.btnCopyLeftToRight.UseVisualStyleBackColor = true;
            this.btnCopyLeftToRight.Click += new System.EventHandler(this.btnCopyLeftToRight_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblRightPath);
            this.panel1.Controls.Add(this.lblRightEdit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(429, 29);
            this.panel1.TabIndex = 0;
            // 
            // lblRightPath
            // 
            this.lblRightPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRightPath.AutoEllipsis = true;
            this.lblRightPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRightPath.Location = new System.Drawing.Point(66, 4);
            this.lblRightPath.Name = "lblRightPath";
            this.lblRightPath.Size = new System.Drawing.Size(360, 20);
            this.lblRightPath.TabIndex = 5;
            this.lblRightPath.Text = "C:\\files\\left.db";
            this.lblRightPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRightEdit
            // 
            this.lblRightEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRightEdit.ImageList = this.imageList1;
            this.lblRightEdit.Location = new System.Drawing.Point(39, 4);
            this.lblRightEdit.Name = "lblRightEdit";
            this.lblRightEdit.Size = new System.Drawing.Size(22, 20);
            this.lblRightEdit.TabIndex = 3;
            // 
            // scbHorizontal
            // 
            this.scbHorizontal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scbHorizontal.Location = new System.Drawing.Point(0, 358);
            this.scbHorizontal.Name = "scbHorizontal";
            this.scbHorizontal.Size = new System.Drawing.Size(820, 19);
            this.scbHorizontal.TabIndex = 4;
            this.scbHorizontal.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scbHorizontal_Scroll);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "edit_left.png");
            this.imageList2.Images.SetKeyName(1, "edit_right.png");
            this.imageList2.Images.SetKeyName(2, "delete_left.png");
            this.imageList2.Images.SetKeyName(3, "delete_right.png");
            // 
            // TableDiffControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scbHorizontal);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TableDiffControl";
            this.Size = new System.Drawing.Size(820, 377);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar scbRight;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblLeftEdit;
        private System.Windows.Forms.Label lblRightEdit;
        private System.Windows.Forms.Label lblLeftPath;
        private System.Windows.Forms.Label lblRightPath;
        private System.Windows.Forms.Button btnCopyRightToLeft;
        private System.Windows.Forms.Button btnCopyLeftToRight;
        private System.Windows.Forms.Button btnEditRow;
        private System.Windows.Forms.Button btnDeleteRows;
        private SelectionBar selectionBar1;
        private System.Windows.Forms.HScrollBar scbHorizontal;
        private FastGridApp.FastGrid grdRight;
        private FastGridApp.FastGrid grdLeft;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
