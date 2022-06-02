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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.list_excel = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_browse_dst = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.28288F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 87.71712F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 197F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btn_remove, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_browse_src, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_go, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.txt_dst, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.list_excel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btn_browse_dst, 3, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.4026F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.5974F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1233, 679);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btn_remove
            // 
            this.btn_remove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_remove.Location = new System.Drawing.Point(1142, 3);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(88, 62);
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
            this.btn_browse_src.Size = new System.Drawing.Size(81, 62);
            this.btn_browse_src.TabIndex = 0;
            this.btn_browse_src.Text = "Browse";
            this.btn_browse_src.UseVisualStyleBackColor = true;
            this.btn_browse_src.Click += new System.EventHandler(this.OnBtnBrowseSrc_Click);
            // 
            // btn_go
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.btn_go, 2);
            this.btn_go.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_go.Location = new System.Drawing.Point(1055, 398);
            this.btn_go.Name = "btn_go";
            this.btn_go.Size = new System.Drawing.Size(175, 41);
            this.btn_go.TabIndex = 2;
            this.btn_go.Text = "Go";
            this.btn_go.UseVisualStyleBackColor = true;
            this.btn_go.Click += new System.EventHandler(this.OnBtnGo_Click);
            // 
            // txt_dst
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txt_dst, 2);
            this.txt_dst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_dst.Location = new System.Drawing.Point(108, 519);
            this.txt_dst.Name = "txt_dst";
            this.txt_dst.Size = new System.Drawing.Size(941, 31);
            this.txt_dst.TabIndex = 3;
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 5);
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(3, 661);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1227, 15);
            this.progressBar1.TabIndex = 4;
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
            this.list_excel.Size = new System.Drawing.Size(941, 436);
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
            this.label1.Size = new System.Drawing.Size(99, 68);
            this.label1.TabIndex = 6;
            this.label1.Text = "src";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 516);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 60);
            this.label2.TabIndex = 7;
            this.label2.Text = "dst";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_browse_dst
            // 
            this.btn_browse_dst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_browse_dst.Location = new System.Drawing.Point(1055, 519);
            this.btn_browse_dst.Name = "btn_browse_dst";
            this.btn_browse_dst.Size = new System.Drawing.Size(81, 54);
            this.btn_browse_dst.TabIndex = 8;
            this.btn_browse_dst.Text = "Browse";
            this.btn_browse_dst.UseVisualStyleBackColor = true;
            this.btn_browse_dst.Click += new System.EventHandler(this.OnBtnBrowseDst_Click);
            // 
            // DataFlowGUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 679);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DataFlowGUIForm";
            this.Text = "DataFlowGUI";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button btn_remove;
    private System.Windows.Forms.Button btn_browse_src;
    private System.Windows.Forms.Button btn_go;
    private System.Windows.Forms.TextBox txt_dst;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.ListBox list_excel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btn_browse_dst;
}
