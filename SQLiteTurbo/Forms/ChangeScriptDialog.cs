using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using SQLiteParser;

namespace SQLiteTurbo
{
    public partial class ChangeScriptDialog : Form
    {
        public ChangeScriptDialog()
        {
            InitializeComponent();
        }

        public void Prepare(string sql)
        {
            txtSQL.Document.Text = sql;
            txtSQL.Document.ParseAll();
        }

        private void ChangeScriptDialog_Load(object sender, EventArgs e)
        {
            txtSQL.Document.SetSyntaxFromEmbeddedResource(Assembly.GetExecutingAssembly(), "SQLiteTurbo.sqlite.syn");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.DefaultExt = "sql";
            dlg.Filter = "SQL files (*.sql)|*.sql|All files|*.*";
            DialogResult res = dlg.ShowDialog(this);
            if (res == DialogResult.Cancel)
                return;

            string fpath = dlg.FileName;
            if (File.Exists(fpath))
                File.Delete(fpath);
            File.WriteAllText(fpath, txtSQL.Document.Text);
        }
    }
}