namespace DiffControl
{
    partial class DualDiffControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DualDiffControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ucLeftDiff = new DiffControl.DiffEditBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mniCopyToRight = new System.Windows.Forms.ToolStripMenuItem();
            this.mniCopyFromRight = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblLeftPath = new System.Windows.Forms.Label();
            this.lblLeftEdit = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblRightPath = new System.Windows.Forms.Label();
            this.lblRightEdit = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.diffBar1 = new DiffControl.DiffBar();
            this.btnCopyRightToLeft = new System.Windows.Forms.Button();
            this.btnCopyLeftToRight = new System.Windows.Forms.Button();
            this.ucRightDiff = new DiffControl.DiffEditBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mniCopyToLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.mniCopyFromLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.scbVertical = new System.Windows.Forms.VScrollBar();
            this.scbHorizontal = new System.Windows.Forms.HScrollBar();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.ucLeftDiff);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Controls.Add(this.ucRightDiff);
            this.splitContainer1.Panel2.Controls.Add(this.scbVertical);
            this.splitContainer1.Size = new System.Drawing.Size(771, 411);
            this.splitContainer1.SplitterDistance = 348;
            this.splitContainer1.TabIndex = 0;
            // 
            // ucLeftDiff
            // 
            this.ucLeftDiff.ContextMenuStrip = this.contextMenuStrip1;
            this.ucLeftDiff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucLeftDiff.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.ucLeftDiff.Location = new System.Drawing.Point(0, 26);
            this.ucLeftDiff.Name = "ucLeftDiff";
            this.ucLeftDiff.Size = new System.Drawing.Size(348, 385);
            this.ucLeftDiff.TabIndex = 3;
            this.ucLeftDiff.Text = "diffEditBox2";
            this.ucLeftDiff.UndoRequested += new System.EventHandler(this.ucLeftDiff_UndoRequested);
            this.ucLeftDiff.ScrollNeedsUpdate += new System.EventHandler(this.ucLeftDiff_ScrollNeedsUpdate);
            this.ucLeftDiff.SaveRequested += new System.EventHandler(this.ucLeftDiff_SaveRequested);
            this.ucLeftDiff.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ucLeftDiff_MouseDown);
            this.ucLeftDiff.RedoRequested += new System.EventHandler(this.ucLeftDiff_RedoRequested);
            this.ucLeftDiff.LinesChanged += new System.EventHandler(this.ucLeftDiff_LinesChanged);
            this.ucLeftDiff.SnapshotChanging += new System.EventHandler(this.ucLeftDiff_SnapshotChanging);
            this.ucLeftDiff.SnapshotChanged += new System.EventHandler(this.ucLeftDiff_SnapshotChanged);
            this.ucLeftDiff.CursorMoved += new System.EventHandler(this.ucLeftDiff_CursorMoved);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCopyToRight,
            this.mniCopyFromRight});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.Size = new System.Drawing.Size(154, 48);
            // 
            // mniCopyToRight
            // 
            this.mniCopyToRight.Image = ((System.Drawing.Image)(resources.GetObject("mniCopyToRight.Image")));
            this.mniCopyToRight.Name = "mniCopyToRight";
            this.mniCopyToRight.Size = new System.Drawing.Size(153, 22);
            this.mniCopyToRight.Text = "Copy to right DB";
            this.mniCopyToRight.Click += new System.EventHandler(this.mniCopyToRight_Click);
            // 
            // mniCopyFromRight
            // 
            this.mniCopyFromRight.Image = ((System.Drawing.Image)(resources.GetObject("mniCopyFromRight.Image")));
            this.mniCopyFromRight.Name = "mniCopyFromRight";
            this.mniCopyFromRight.Size = new System.Drawing.Size(153, 22);
            this.mniCopyFromRight.Text = "Copy to left DB";
            this.mniCopyFromRight.Click += new System.EventHandler(this.mniCopyFromRight_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblLeftPath);
            this.panel1.Controls.Add(this.lblLeftEdit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(348, 26);
            this.panel1.TabIndex = 4;
            // 
            // lblLeftPath
            // 
            this.lblLeftPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeftPath.AutoEllipsis = true;
            this.lblLeftPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLeftPath.Location = new System.Drawing.Point(31, 3);
            this.lblLeftPath.Name = "lblLeftPath";
            this.lblLeftPath.Size = new System.Drawing.Size(315, 20);
            this.lblLeftPath.TabIndex = 2;
            this.lblLeftPath.Text = "C:\\files\\left.db";
            this.lblLeftPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLeftEdit
            // 
            this.lblLeftEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLeftEdit.ImageIndex = 0;
            this.lblLeftEdit.ImageList = this.imageList1;
            this.lblLeftEdit.Location = new System.Drawing.Point(3, 3);
            this.lblLeftEdit.Name = "lblLeftEdit";
            this.lblLeftEdit.Size = new System.Drawing.Size(22, 20);
            this.lblLeftEdit.TabIndex = 1;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Edit (Custom).png");
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblRightPath);
            this.panel2.Controls.Add(this.lblRightEdit);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(42, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(377, 26);
            this.panel2.TabIndex = 5;
            // 
            // lblRightPath
            // 
            this.lblRightPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRightPath.AutoEllipsis = true;
            this.lblRightPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRightPath.Location = new System.Drawing.Point(31, 3);
            this.lblRightPath.Name = "lblRightPath";
            this.lblRightPath.Size = new System.Drawing.Size(343, 20);
            this.lblRightPath.TabIndex = 4;
            this.lblRightPath.Text = "C:\\files\\left.db";
            this.lblRightPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRightEdit
            // 
            this.lblRightEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRightEdit.ImageIndex = 0;
            this.lblRightEdit.ImageList = this.imageList1;
            this.lblRightEdit.Location = new System.Drawing.Point(3, 3);
            this.lblRightEdit.Name = "lblRightEdit";
            this.lblRightEdit.Size = new System.Drawing.Size(22, 20);
            this.lblRightEdit.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.diffBar1);
            this.panel3.Controls.Add(this.btnCopyRightToLeft);
            this.panel3.Controls.Add(this.btnCopyLeftToRight);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(42, 411);
            this.panel3.TabIndex = 6;
            // 
            // diffBar1
            // 
            this.diffBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.diffBar1.BackColor = System.Drawing.Color.White;
            this.diffBar1.Location = new System.Drawing.Point(4, 112);
            this.diffBar1.Name = "diffBar1";
            this.diffBar1.Size = new System.Drawing.Size(32, 298);
            this.diffBar1.TabIndex = 6;
            this.diffBar1.Text = "diffBar1";
            this.diffBar1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.diffBar1_MouseClick);
            // 
            // btnCopyRightToLeft
            // 
            this.btnCopyRightToLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyRightToLeft.Image")));
            this.btnCopyRightToLeft.Location = new System.Drawing.Point(2, 68);
            this.btnCopyRightToLeft.Name = "btnCopyRightToLeft";
            this.btnCopyRightToLeft.Size = new System.Drawing.Size(36, 37);
            this.btnCopyRightToLeft.TabIndex = 5;
            this.btnCopyRightToLeft.UseVisualStyleBackColor = true;
            this.btnCopyRightToLeft.Click += new System.EventHandler(this.btnCopyRightToLeft_Click);
            // 
            // btnCopyLeftToRight
            // 
            this.btnCopyLeftToRight.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyLeftToRight.Image")));
            this.btnCopyLeftToRight.Location = new System.Drawing.Point(2, 25);
            this.btnCopyLeftToRight.Name = "btnCopyLeftToRight";
            this.btnCopyLeftToRight.Size = new System.Drawing.Size(36, 37);
            this.btnCopyLeftToRight.TabIndex = 4;
            this.btnCopyLeftToRight.UseVisualStyleBackColor = true;
            this.btnCopyLeftToRight.Click += new System.EventHandler(this.btnCopyLeftToRight_Click);
            // 
            // ucRightDiff
            // 
            this.ucRightDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucRightDiff.ContextMenuStrip = this.contextMenuStrip2;
            this.ucRightDiff.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.ucRightDiff.Location = new System.Drawing.Point(45, 26);
            this.ucRightDiff.Name = "ucRightDiff";
            this.ucRightDiff.Size = new System.Drawing.Size(357, 385);
            this.ucRightDiff.TabIndex = 2;
            this.ucRightDiff.Text = "diffEditBox1";
            this.ucRightDiff.UndoRequested += new System.EventHandler(this.ucRightDiff_UndoRequested);
            this.ucRightDiff.ScrollNeedsUpdate += new System.EventHandler(this.ucRightDiff_ScrollNeedsUpdate);
            this.ucRightDiff.SaveRequested += new System.EventHandler(this.ucRightDiff_SaveRequested);
            this.ucRightDiff.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ucRightDiff_MouseDown);
            this.ucRightDiff.RedoRequested += new System.EventHandler(this.ucRightDiff_RedoRequested);
            this.ucRightDiff.LinesChanged += new System.EventHandler(this.ucRightDiff_LinesChanged);
            this.ucRightDiff.SnapshotChanging += new System.EventHandler(this.ucRightDiff_SnapshotChanging);
            this.ucRightDiff.SnapshotChanged += new System.EventHandler(this.ucRightDiff_SnapshotChanged);
            this.ucRightDiff.CursorMoved += new System.EventHandler(this.ucRightDiff_CursorMoved);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCopyToLeft,
            this.mniCopyFromLeft});
            this.contextMenuStrip2.Name = "contextMenuStrip1";
            this.contextMenuStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip2.Size = new System.Drawing.Size(154, 48);
            // 
            // mniCopyToLeft
            // 
            this.mniCopyToLeft.Image = ((System.Drawing.Image)(resources.GetObject("mniCopyToLeft.Image")));
            this.mniCopyToLeft.Name = "mniCopyToLeft";
            this.mniCopyToLeft.Size = new System.Drawing.Size(153, 22);
            this.mniCopyToLeft.Text = "Copy to left DB";
            this.mniCopyToLeft.Click += new System.EventHandler(this.mniCopyToLeft_Click);
            // 
            // mniCopyFromLeft
            // 
            this.mniCopyFromLeft.Image = ((System.Drawing.Image)(resources.GetObject("mniCopyFromLeft.Image")));
            this.mniCopyFromLeft.Name = "mniCopyFromLeft";
            this.mniCopyFromLeft.Size = new System.Drawing.Size(153, 22);
            this.mniCopyFromLeft.Text = "Copy to right DB";
            this.mniCopyFromLeft.Click += new System.EventHandler(this.mniCopyFromLeft_Click);
            // 
            // scbVertical
            // 
            this.scbVertical.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scbVertical.Location = new System.Drawing.Point(402, 26);
            this.scbVertical.Name = "scbVertical";
            this.scbVertical.Size = new System.Drawing.Size(17, 385);
            this.scbVertical.TabIndex = 1;
            this.scbVertical.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scbVertical_Scroll);
            // 
            // scbHorizontal
            // 
            this.scbHorizontal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scbHorizontal.Location = new System.Drawing.Point(0, 411);
            this.scbHorizontal.Name = "scbHorizontal";
            this.scbHorizontal.Size = new System.Drawing.Size(771, 20);
            this.scbHorizontal.TabIndex = 2;
            this.scbHorizontal.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scbHorizontal_Scroll);
            // 
            // DualDiffControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scbHorizontal);
            this.Controls.Add(this.splitContainer1);
            this.Name = "DualDiffControl";
            this.Size = new System.Drawing.Size(771, 431);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.VScrollBar scbVertical;
        private DiffEditBox ucLeftDiff;
        private DiffEditBox ucRightDiff;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblLeftPath;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lblLeftEdit;
        private System.Windows.Forms.Label lblRightPath;
        private System.Windows.Forms.Label lblRightEdit;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mniCopyFromRight;
        private System.Windows.Forms.ToolStripMenuItem mniCopyToRight;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem mniCopyFromLeft;
        private System.Windows.Forms.ToolStripMenuItem mniCopyToLeft;
        private System.Windows.Forms.HScrollBar scbHorizontal;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnCopyRightToLeft;
        private System.Windows.Forms.Button btnCopyLeftToRight;
        private DiffBar diffBar1;
    }
}
