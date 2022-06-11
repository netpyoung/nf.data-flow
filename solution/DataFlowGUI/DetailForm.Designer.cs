namespace DataFlowGUI
{
    partial class DetailForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_browse = new System.Windows.Forms.Button();
            this.txt_password = new System.Windows.Forms.TextBox();
            this.txt_namespace = new System.Windows.Forms.TextBox();
            this.txt_output_code_dir = new System.Windows.Forms.TextBox();
            this.check_datetime_as_ticks = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.625F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.375F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btn_browse, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.txt_password, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txt_namespace, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txt_output_code_dir, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.check_datetime_as_ticks, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.64865F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.35135F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(771, 151);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 36);
            this.label1.TabIndex = 0;
            this.label1.Text = "password";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 38);
            this.label2.TabIndex = 1;
            this.label2.Text = "namespace";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 42);
            this.label3.TabIndex = 2;
            this.label3.Text = "out_csharp";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_browse
            // 
            this.btn_browse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_browse.Location = new System.Drawing.Point(680, 77);
            this.btn_browse.Name = "btn_browse";
            this.btn_browse.Size = new System.Drawing.Size(88, 36);
            this.btn_browse.TabIndex = 3;
            this.btn_browse.Text = "Browse";
            this.btn_browse.UseVisualStyleBackColor = true;
            this.btn_browse.Click += new System.EventHandler(this.OnBtnBrowse_Click);
            // 
            // txt_password
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txt_password, 2);
            this.txt_password.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_password.Location = new System.Drawing.Point(176, 3);
            this.txt_password.Name = "txt_password";
            this.txt_password.Size = new System.Drawing.Size(592, 31);
            this.txt_password.TabIndex = 4;
            // 
            // txt_namespace
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txt_namespace, 2);
            this.txt_namespace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_namespace.Location = new System.Drawing.Point(176, 39);
            this.txt_namespace.Name = "txt_namespace";
            this.txt_namespace.Size = new System.Drawing.Size(592, 31);
            this.txt_namespace.TabIndex = 5;
            // 
            // txt_output_code_dir
            // 
            this.txt_output_code_dir.AllowDrop = true;
            this.txt_output_code_dir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_output_code_dir.Location = new System.Drawing.Point(176, 77);
            this.txt_output_code_dir.Name = "txt_output_code_dir";
            this.txt_output_code_dir.Size = new System.Drawing.Size(498, 31);
            this.txt_output_code_dir.TabIndex = 6;
            this.txt_output_code_dir.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnTxtOutputCodeDir_DragDrop);
            this.txt_output_code_dir.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnTxtOutputCodeDir_DragEnter);
            // 
            // check_datetime_as_ticks
            // 
            this.check_datetime_as_ticks.AutoSize = true;
            this.check_datetime_as_ticks.Location = new System.Drawing.Point(176, 119);
            this.check_datetime_as_ticks.Name = "check_datetime_as_ticks";
            this.check_datetime_as_ticks.Size = new System.Drawing.Size(22, 21);
            this.check_datetime_as_ticks.TabIndex = 8;
            this.check_datetime_as_ticks.UseVisualStyleBackColor = true;
            this.check_datetime_as_ticks.CheckedChanged += new System.EventHandler(this.OnCheck_datetime_as_ticks_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(167, 35);
            this.label4.TabIndex = 9;
            this.label4.Text = "datetime_as_ticks";
            // 
            // DetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 151);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DetailForm";
            this.Text = "DetailForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_browse;
        private System.Windows.Forms.TextBox txt_password;
        private System.Windows.Forms.TextBox txt_namespace;
        private System.Windows.Forms.TextBox txt_output_code_dir;
        private System.Windows.Forms.CheckBox check_datetime_as_ticks;
        private System.Windows.Forms.Label label4;
    }
}