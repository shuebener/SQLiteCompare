namespace SQLiteTurbo
{
    partial class ChangeScriptDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeScriptDialog));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.txtSQL = new Alsing.Windows.Forms.SyntaxBoxControl();
            this.syntaxDocument1 = new Alsing.SourceCode.SyntaxDocument(this.components);
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(682, 478);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(99, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAs.Location = new System.Drawing.Point(577, 478);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(99, 23);
            this.btnSaveAs.TabIndex = 4;
            this.btnSaveAs.Text = "Save as...";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // txtSQL
            // 
            this.txtSQL.ActiveView = Alsing.Windows.Forms.ActiveView.BottomRight;
            this.txtSQL.AllowBreakPoints = false;
            this.txtSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSQL.AutoListPosition = null;
            this.txtSQL.AutoListSelectedText = "a123";
            this.txtSQL.AutoListVisible = false;
            this.txtSQL.BackColor = System.Drawing.Color.White;
            this.txtSQL.BorderStyle = Alsing.Windows.Forms.BorderStyle.None;
            this.txtSQL.CopyAsRTF = false;
            this.txtSQL.Document = this.syntaxDocument1;
            this.txtSQL.FontName = "Courier new";
            this.txtSQL.HighLightActiveLine = true;
            this.txtSQL.HighLightedLineColor = System.Drawing.Color.Khaki;
            this.txtSQL.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtSQL.InfoTipCount = 1;
            this.txtSQL.InfoTipPosition = null;
            this.txtSQL.InfoTipSelectedIndex = 1;
            this.txtSQL.InfoTipVisible = false;
            this.txtSQL.Location = new System.Drawing.Point(12, 12);
            this.txtSQL.LockCursorUpdate = false;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.ShowGutterMargin = false;
            this.txtSQL.ShowScopeIndicator = false;
            this.txtSQL.ShowTabGuides = true;
            this.txtSQL.Size = new System.Drawing.Size(769, 460);
            this.txtSQL.SmoothScroll = false;
            this.txtSQL.SplitView = false;
            this.txtSQL.SplitviewH = -4;
            this.txtSQL.SplitviewV = -4;
            this.txtSQL.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(243)))), ((int)(((byte)(234)))));
            this.txtSQL.TabIndex = 5;
            this.txtSQL.Text = "syntaxBoxControl1";
            this.txtSQL.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            // 
            // syntaxDocument1
            // 
            this.syntaxDocument1.Lines = new string[] {
        ""};
            this.syntaxDocument1.MaxUndoBufferSize = 1000;
            this.syntaxDocument1.Modified = false;
            this.syntaxDocument1.UndoStep = 0;
            // 
            // ChangeScriptDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 509);
            this.Controls.Add(this.txtSQL);
            this.Controls.Add(this.btnSaveAs);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "ChangeScriptDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Script Editor";
            this.Load += new System.EventHandler(this.ChangeScriptDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSaveAs;
        private Alsing.Windows.Forms.SyntaxBoxControl txtSQL;
        private Alsing.SourceCode.SyntaxDocument syntaxDocument1;
    }
}