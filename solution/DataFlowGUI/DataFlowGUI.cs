using NF.Tools.DataFlow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YamlDotNet.Serialization;

namespace DataFlowGUI
{
    public partial class DataFlowGUIForm : Form
    {
        const string DATAFLOW_YAML = "dataflow.yaml";

        DataFlowRunnerOption _opt { get; } = new DataFlowRunnerOption();
        HashSet<string> _srcItemSet { get; } = new HashSet<string>();

        public DataFlowGUIForm()
        {
            InitializeComponent();
            this.menu_browse_src.Click += new System.EventHandler(this.OnBtnBrowseSrc_Click);
            this.menu_browse_dst.Click += new System.EventHandler(this.OnBtnBrowseDst_Click);
            this.menu_go.Click += new System.EventHandler(this.OnBtnGo_Click);
            this.menu_exit.Click += new System.EventHandler((o, s) => this.Close());
            this.txt_dst.TextChanged += new EventHandler(this.OnDstChanged);

            if (File.Exists(DATAFLOW_YAML))
            {
                string configYamlStr = File.ReadAllText(DATAFLOW_YAML);
                IDeserializer deserializer = new DeserializerBuilder().Build();
                DataFlowRunnerOption yaml = deserializer.Deserialize<DataFlowRunnerOption>(configYamlStr);
                _opt = yaml;

                if (_opt != null)
                {
                    if (_opt.input_paths != null)
                    {
                        foreach (string inputPath in _opt.input_paths)
                        {
                            SrcItemAdd(inputPath);
                        }
                    }
                    txt_dst.Text = _opt.output_db_path;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StoreOption();

            try
            {
                ISerializer serializer = new SerializerBuilder().Build();
                string configYamlStr = serializer.Serialize(_opt);
                File.WriteAllText(DATAFLOW_YAML, configYamlStr);
            }
            finally
            {
                base.OnFormClosing(e);
            }
        }

        // ===========================================================================================
        #region Click 
        private void OnBtnRemove_Click(object sender, System.EventArgs e)
        {
            while (list_excel.SelectedItems.Count > 0)
            {
                SrcItemDel(list_excel.SelectedItem.ToString());
            }
        }

        private void OnBtnBrowseSrc_Click(object sender, System.EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                dlg.Filter = "excel files (*.xlsx)|*.xlsx";
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                foreach (string fileName in dlg.FileNames)
                {
                    SrcItemAdd(fileName);
                }
            }
        }

        private void OnBtnGo_Click(object sender, System.EventArgs e)
        {
            List<string> inputPaths = GetInputPathsOrNull(list_excel);
            if (inputPaths == null)
            {
                return;
            }

            StoreOption();

            progress_export.ForeColor = Color.Green;

            progress_export.Value = 0;
            int ret = DataFlowRunner.Run(_opt);
            if (ret != 0)
            {
                progress_export.ForeColor = Color.Red;
            }
            btn_reveal.Enabled = true;
            progress_export.Value = 100;
        }

        private void OnBtnBrowseDst_Click(object sender, System.EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.OverwritePrompt = false;
                dlg.FileName = "output.db";
                dlg.Filter = "db files (*.db)|*.db";
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                txt_dst.Text = dlg.FileName;
            }
        }

        private void OnBtnReveal_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(_opt.output_db_path))
            {
                return;
            }
            if (!File.Exists(_opt.output_db_path))
            {
                return;
            }
            string dirName = Path.GetDirectoryName(_opt.output_db_path);
            Process.Start("explorer.exe", dirName);
        }

        private void OnMenuInfo_Click(object sender, System.EventArgs e)
        {
            AboutForm f = new AboutForm();
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }
        #endregion // Click
        // ===========================================================================================
        // ===========================================================================================
        #region DragDrop
        private void OnListExcel_DragDrop(object sender, DragEventArgs e)
        {
            List<string> paths = GetSrcExcelPaths(e);
            foreach (string excelPath in paths)
            {
                SrcItemAdd(excelPath);
            }
        }

        private void OnListExcel_DragEnter(object sender, DragEventArgs e)
        {
            List<string> paths = GetSrcExcelPaths(e);
            if (paths.Count == 0)
            {
                return;
            }
            e.Effect = DragDropEffects.Link;
        }

        private void OnTxtDst_DragEnter(object sender, DragEventArgs e)
        {
            string dbPath = GetDstDbPathOrNull(e);
            if (dbPath == null)
            {
                return;
            }
            e.Effect = DragDropEffects.Link;
        }

        private void OnTxtDst_DragDrop(object sender, DragEventArgs e)
        {
            string dbPath = GetDstDbPathOrNull(e);
            txt_dst.Text = dbPath;
        }
        #endregion // DragDrop
        // ===========================================================================================
        private void SrcItemAdd(string path)
        {
            if (_srcItemSet.Add(path))
            {
                list_excel.Items.Add(path);
            }
        }

        private void SrcItemDel(string path)
        {
            if (_srcItemSet.Remove(path))
            {
                list_excel.Items.Remove(path);
            }
        }

        private void OnDstChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            string path = tb.Text;
            if (string.IsNullOrEmpty(path))
            {
                btn_reveal.Enabled = false;
                return;
            }
            if (Path.GetExtension(path) != ".db")
            {
                btn_reveal.Enabled = false;
                return;
            }
            if (!File.Exists(path))
            {
                btn_reveal.Enabled = false;
                return;
            }
            btn_reveal.Enabled = true;
        }

        void StoreOption()
        {
            _opt.input_paths = GetInputPathsOrNull(list_excel);
            _opt.@namespace = "AutoGenerated.DB";
            _opt.output_db_path = txt_dst.Text;
        }

        private List<string> GetSrcExcelPaths(DragEventArgs e)
        {
            bool isFileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);
            if (!isFileDrop)
            {
                return new();
            }

            object dropData = e.Data.GetData(DataFormats.FileDrop);
            if (dropData == null)
            {
                return new();
            }

            string[] dropPaths = dropData as string[];
            if (dropPaths == null)
            {
                return new();
            }

            return DataFlowRunner.GetExcelFpaths(dropPaths);
        }

        List<string> GetInputPathsOrNull(ListBox listBox)
        {
            int listCount = listBox.Items.Count;
            if (listCount == 0)
            {
                return null;
            }

            List<string> ret = new List<string>(listBox.Items.Count);
            foreach (object item in listBox.Items)
            {
                ret.Add(item.ToString());
            }
            return ret;
        }

        private string GetDstDbPathOrNull(DragEventArgs e)
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
            if (Path.GetExtension(path) != ".db")
            {
                return null;
            }
            return path;
        }
    }
}