using NF.Tools.DataFlow;
using System;
using System.IO;
using System.Windows.Forms;

namespace DataFlowGUI
{
    public partial class DetailForm : Form
    {
        private DataFlowRunnerOption Opt { get; }

        public DetailForm(DataFlowRunnerOption opt)
        {
            InitializeComponent();

            Opt = opt;
            this.txt_namespace.Text = opt.@namespace;
            this.txt_password.Text = opt.password;
            this.txt_output_code_dir.Text = opt.out_csharp;
            this.check_datetime_as_ticks.Checked =  Opt.datetime_as_ticks;

            this.txt_namespace.TextChanged += OnNamespaceChanged;
            this.txt_password.TextChanged += OnPasswordChanged;
            this.txt_output_code_dir.TextChanged += OnOuputCodeDirChanged;
        }

        private void OnPasswordChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            Opt.password = tb.Text;
        }

        private void OnOuputCodeDirChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            Opt.out_csharp = tb.Text;
        }

        private void OnNamespaceChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            Opt.@namespace = tb.Text;
        }

        private void OnBtnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                this.txt_output_code_dir.Text = dlg.SelectedPath;
            }
        }

        // ===========================================================================================
        #region DragDrop
        private void OnTxtOutputCodeDir_DragDrop(object sender, DragEventArgs e)
        {
            string dir = GetOutputCodeDirOrNull(e);
            this.txt_output_code_dir.Text = dir;
        }

        private void OnTxtOutputCodeDir_DragEnter(object sender, DragEventArgs e)
        {
            string dir = GetOutputCodeDirOrNull(e);
            if (dir == null)
            {
                return;
            }
            e.Effect = DragDropEffects.Link;
        }
        #endregion // DragDrop
        // ===========================================================================================


        string GetOutputCodeDirOrNull(DragEventArgs e)
        {
            bool isFileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);
            if (!isFileDrop)
            {
                return null;
            }

            object dropData = e.Data.GetData(DataFormats.FileDrop);
            if (dropData == null)
            {
                return null;
            }

            string[] dropPaths = dropData as string[];
            if (dropPaths == null)
            {
                return null;
            }
            string path = dropPaths[0];
            if (!Directory.Exists(path))
            {
                return null;
            }
            return path;
        }

        private void OnCheck_datetime_as_ticks_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Opt.datetime_as_ticks = cb.Checked;
        }
    }
}
