namespace DataFlowGUI;

partial class DataFlowGUIForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_remove = new System.Windows.Forms.Button();
            this.btn_browse_src = new System.Windows.Forms.Button();
            this.btn_go = new System.Windows.Forms.Button();
            this.txt_dst = new System.Windows.Forms.TextBox();
            this.progress_export = new System.Windows.Forms.ProgressBar();
            this.list_excel = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_browse_dst = new System.Windows.Forms.Button();
            this.btn_reveal = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_browse_src = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_browse_dst = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_go = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_info = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Gainsboro;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.28288F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 87.71712F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 197F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btn_remove, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_browse_src, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_go, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.txt_dst, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.progress_export, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.list_excel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btn_browse_dst, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.btn_reveal, 4, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 33);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.4026F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.5974F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1233, 646);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btn_remove
            // 
            this.btn_remove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_remove.Location = new System.Drawing.Point(1142, 3);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(88, 57);
            this.btn_remove.TabIndex = 1;
            this.btn_remove.Text = "Remove";
            this.btn_remove.UseVisualStyleBackColor = true;
            this.btn_remove.Click += new System.EventHandler(this.OnBtnRemove_Click);
            // 
            // btn_browse_src
            // 
            this.btn_browse_src.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_browse_src.Location = new System.Drawing.Point(1055, 3);
            this.btn_browse_src.Name = "btn_browse_src";
            this.btn_browse_src.Size = new System.Drawing.Size(81, 57);
            this.btn_browse_src.TabIndex = 0;
            this.btn_browse_src.Text = "Browse";
            this.btn_browse_src.UseVisualStyleBackColor = true;
            this.btn_browse_src.Click += new System.EventHandler(this.OnBtnBrowseSrc_Click);
            // 
            // btn_go
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.btn_go, 5);
            this.btn_go.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_go.Location = new System.Drawing.Point(3, 526);
            this.btn_go.Name = "btn_go";
            this.btn_go.Size = new System.Drawing.Size(1227, 54);
            this.btn_go.TabIndex = 2;
            this.btn_go.Text = "Go";
            this.btn_go.UseVisualStyleBackColor = true;
            this.btn_go.Click += new System.EventHandler(this.OnBtnGo_Click);
            // 
            // txt_dst
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txt_dst, 2);
            this.txt_dst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_dst.Location = new System.Drawing.Point(108, 412);
            this.txt_dst.MinimumSize = new System.Drawing.Size(0, 66);
            this.txt_dst.Name = "txt_dst";
            this.txt_dst.Size = new System.Drawing.Size(941, 66);
            this.txt_dst.TabIndex = 3;
            // 
            // progress_export
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progress_export, 5);
            this.progress_export.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progress_export.Location = new System.Drawing.Point(3, 586);
            this.progress_export.Name = "progress_export";
            this.progress_export.Size = new System.Drawing.Size(1227, 57);
            this.progress_export.TabIndex = 4;
            // 
            // list_excel
            // 
            this.list_excel.AllowDrop = true;
            this.tableLayoutPanel1.SetColumnSpan(this.list_excel, 2);
            this.list_excel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list_excel.FormattingEnabled = true;
            this.list_excel.ItemHeight = 25;
            this.list_excel.Location = new System.Drawing.Point(108, 3);
            this.list_excel.Name = "list_excel";
            this.tableLayoutPanel1.SetRowSpan(this.list_excel, 3);
            this.list_excel.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.list_excel.Size = new System.Drawing.Size(941, 403);
            this.list_excel.TabIndex = 5;
            this.list_excel.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnListExcel_DragDrop);
            this.list_excel.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnListExcel_DragEnter);
            this.list_excel.DragOver += new System.Windows.Forms.DragEventHandler(this.OnListExcel_DragOver);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 63);
            this.label1.TabIndex = 6;
            this.label1.Text = "src";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 409);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 74);
            this.label2.TabIndex = 7;
            this.label2.Text = "dst";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_browse_dst
            // 
            this.btn_browse_dst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_browse_dst.Location = new System.Drawing.Point(1055, 412);
            this.btn_browse_dst.Name = "btn_browse_dst";
            this.btn_browse_dst.Size = new System.Drawing.Size(81, 68);
            this.btn_browse_dst.TabIndex = 8;
            this.btn_browse_dst.Text = "Browse";
            this.btn_browse_dst.UseVisualStyleBackColor = true;
            this.btn_browse_dst.Click += new System.EventHandler(this.OnBtnBrowseDst_Click);
            // 
            // btn_reveal
            // 
            this.btn_reveal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_reveal.Enabled = false;
            this.btn_reveal.Location = new System.Drawing.Point(1142, 412);
            this.btn_reveal.Name = "btn_reveal";
            this.btn_reveal.Size = new System.Drawing.Size(88, 68);
            this.btn_reveal.TabIndex = 10;
            this.btn_reveal.Text = "Reveal";
            this.btn_reveal.UseVisualStyleBackColor = true;
            this.btn_reveal.Click += new System.EventHandler(this.OnBtnReveal_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1233, 33);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuItem
            // 
            this.menuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_browse_src,
            this.menu_browse_dst,
            this.toolStripSeparator1,
            this.menu_go,
            this.toolStripSeparator2,
            this.menu_exit});
            this.menuItem.Name = "menuItem";
            this.menuItem.Size = new System.Drawing.Size(73, 29);
            this.menuItem.Text = "Menu";
            // 
            // menu_browse_src
            // 
            this.menu_browse_src.Name = "menu_browse_src";
            this.menu_browse_src.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.menu_browse_src.Size = new System.Drawing.Size(330, 34);
            this.menu_browse_src.Text = "&Browse Source";
            // 
            // menu_browse_dst
            // 
            this.menu_browse_dst.Name = "menu_browse_dst";
            this.menu_browse_dst.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.menu_browse_dst.Size = new System.Drawing.Size(330, 34);
            this.menu_browse_dst.Text = "Browse &Destination";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(327, 6);
            // 
            // menu_go
            // 
            this.menu_go.Name = "menu_go";
            this.menu_go.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.menu_go.Size = new System.Drawing.Size(330, 34);
            this.menu_go.Text = "&Go";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(327, 6);
            // 
            // menu_exit
            // 
            this.menu_exit.Name = "menu_exit";
            this.menu_exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.menu_exit.Size = new System.Drawing.Size(330, 34);
            this.menu_exit.Text = "&Quit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_info});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(36, 29);
            this.helpToolStripMenuItem.Text = "?";
            // 
            // menu_info
            // 
            this.menu_info.Name = "menu_info";
            this.menu_info.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.menu_info.Size = new System.Drawing.Size(195, 34);
            this.menu_info.Text = "About";
            this.menu_info.Click += new System.EventHandler(this.OnMenuInfo_Click);
            // 
            // DataFlowGUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 679);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "DataFlowGUIForm";
            this.Text = "DataFlowGUI";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button btn_remove;
    private System.Windows.Forms.Button btn_browse_src;
    private System.Windows.Forms.Button btn_go;
    private System.Windows.Forms.TextBox txt_dst;
    private System.Windows.Forms.ProgressBar progress_export;
    private System.Windows.Forms.ListBox list_excel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btn_browse_dst;
    private System.Windows.Forms.Button btn_reveal;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem menuItem;
    private System.Windows.Forms.ToolStripMenuItem menu_browse_dst;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem menu_go;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem menu_exit;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem menu_info;
    private System.Windows.Forms.ToolStripMenuItem menu_browse_src;
}
