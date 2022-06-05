using System.Diagnostics;
using System.Windows.Forms;

namespace DataFlowGUI
{
    public partial class AboutForm : Form
    {
        const string VERSION = "0.0.0";

        public AboutForm()
        {
            InitializeComponent();
            lbl_version.Text = $"version: {VERSION}";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnLinkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/netpyoung/nf.data-flow") { UseShellExecute = true });
        }
    }
}
