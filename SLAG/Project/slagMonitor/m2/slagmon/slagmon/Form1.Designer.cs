namespace slagmon
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.textBox1_log = new System.Windows.Forms.TextBox();
            this.textBox3_input = new System.Windows.Forms.TextBox();
            this.label1_log = new System.Windows.Forms.Label();
            this.label2_source = new System.Windows.Forms.Label();
            this.label3_input = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1clear = new System.Windows.Forms.Button();
            this.label1_filename = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.buttonFolder = new System.Windows.Forms.Button();
            this.textBoxVar = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxFiles = new System.Windows.Forms.ComboBox();
            this.dataSource = new System.Windows.Forms.DataGridView();
            this.bp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Line = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataSource)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1_log
            // 
            this.textBox1_log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1_log.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox1_log.Location = new System.Drawing.Point(2, 24);
            this.textBox1_log.Multiline = true;
            this.textBox1_log.Name = "textBox1_log";
            this.textBox1_log.ReadOnly = true;
            this.textBox1_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1_log.Size = new System.Drawing.Size(398, 328);
            this.textBox1_log.TabIndex = 0;
            this.textBox1_log.WordWrap = false;
            // 
            // textBox3_input
            // 
            this.textBox3_input.AllowDrop = true;
            this.textBox3_input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3_input.Location = new System.Drawing.Point(2, 370);
            this.textBox3_input.Multiline = true;
            this.textBox3_input.Name = "textBox3_input";
            this.textBox3_input.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox3_input.Size = new System.Drawing.Size(1034, 96);
            this.textBox3_input.TabIndex = 2;
            this.textBox3_input.WordWrap = false;
            this.textBox3_input.TextChanged += new System.EventHandler(this.textBox3_input_TextChanged);
            this.textBox3_input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox3_input_KeyDown);
            this.textBox3_input.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox3_input_KeyPress);
            // 
            // label1_log
            // 
            this.label1_log.AutoSize = true;
            this.label1_log.Location = new System.Drawing.Point(0, 9);
            this.label1_log.Name = "label1_log";
            this.label1_log.Size = new System.Drawing.Size(20, 12);
            this.label1_log.TabIndex = 3;
            this.label1_log.Text = "log";
            // 
            // label2_source
            // 
            this.label2_source.AutoSize = true;
            this.label2_source.Location = new System.Drawing.Point(404, 9);
            this.label2_source.Name = "label2_source";
            this.label2_source.Size = new System.Drawing.Size(39, 12);
            this.label2_source.TabIndex = 4;
            this.label2_source.Text = "source";
            // 
            // label3_input
            // 
            this.label3_input.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3_input.AutoSize = true;
            this.label3_input.Location = new System.Drawing.Point(0, 355);
            this.label3_input.Name = "label3_input";
            this.label3_input.Size = new System.Drawing.Size(30, 12);
            this.label3_input.TabIndex = 5;
            this.label3_input.Text = "input";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1clear
            // 
            this.button1clear.Location = new System.Drawing.Point(26, 6);
            this.button1clear.Name = "button1clear";
            this.button1clear.Size = new System.Drawing.Size(50, 18);
            this.button1clear.TabIndex = 6;
            this.button1clear.Text = "clear";
            this.button1clear.UseVisualStyleBackColor = true;
            this.button1clear.Click += new System.EventHandler(this.button1clear_Click);
            // 
            // label1_filename
            // 
            this.label1_filename.AutoSize = true;
            this.label1_filename.Location = new System.Drawing.Point(449, 9);
            this.label1_filename.Name = "label1_filename";
            this.label1_filename.Size = new System.Drawing.Size(52, 12);
            this.label1_filename.TabIndex = 7;
            this.label1_filename.Text = "filename ";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(710, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Edit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1037, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // buttonConfig
            // 
            this.buttonConfig.Location = new System.Drawing.Point(325, 1);
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.Size = new System.Drawing.Size(75, 23);
            this.buttonConfig.TabIndex = 10;
            this.buttonConfig.Text = "Config";
            this.buttonConfig.UseVisualStyleBackColor = true;
            this.buttonConfig.Click += new System.EventHandler(this.buttonConfig_Click);
            // 
            // buttonFolder
            // 
            this.buttonFolder.Location = new System.Drawing.Point(629, 1);
            this.buttonFolder.Name = "buttonFolder";
            this.buttonFolder.Size = new System.Drawing.Size(75, 23);
            this.buttonFolder.TabIndex = 11;
            this.buttonFolder.Text = "Folder";
            this.buttonFolder.UseVisualStyleBackColor = true;
            this.buttonFolder.Click += new System.EventHandler(this.buttonFolder_Click);
            // 
            // textBoxVar
            // 
            this.textBoxVar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxVar.Location = new System.Drawing.Point(791, 24);
            this.textBoxVar.Multiline = true;
            this.textBoxVar.Name = "textBoxVar";
            this.textBoxVar.ReadOnly = true;
            this.textBoxVar.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxVar.Size = new System.Drawing.Size(234, 328);
            this.textBoxVar.TabIndex = 12;
            this.textBoxVar.WordWrap = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(791, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "variables";
            // 
            // comboBoxFiles
            // 
            this.comboBoxFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFiles.FormattingEnabled = true;
            this.comboBoxFiles.Location = new System.Drawing.Point(444, 3);
            this.comboBoxFiles.Name = "comboBoxFiles";
            this.comboBoxFiles.Size = new System.Drawing.Size(183, 20);
            this.comboBoxFiles.TabIndex = 14;
            this.comboBoxFiles.SelectedIndexChanged += new System.EventHandler(this.comboBoxFiles_SelectedIndexChanged);
            // 
            // dataSource
            // 
            this.dataSource.AllowDrop = true;
            this.dataSource.AllowUserToAddRows = false;
            this.dataSource.AllowUserToDeleteRows = false;
            this.dataSource.AllowUserToResizeColumns = false;
            this.dataSource.AllowUserToResizeRows = false;
            this.dataSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataSource.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dataSource.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataSource.ColumnHeadersVisible = false;
            this.dataSource.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.bp,
            this.Line,
            this.Code});
            this.dataSource.Location = new System.Drawing.Point(406, 29);
            this.dataSource.Name = "dataSource";
            this.dataSource.ReadOnly = true;
            this.dataSource.RowHeadersVisible = false;
            this.dataSource.RowTemplate.Height = 12;
            this.dataSource.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataSource.Size = new System.Drawing.Size(379, 323);
            this.dataSource.TabIndex = 15;
            this.dataSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataSource_DragDrop);
            this.dataSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataSource_DragEnter);
            this.dataSource.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataSource_KeyDown);
            // 
            // bp
            // 
            this.bp.Frozen = true;
            this.bp.HeaderText = "bp";
            this.bp.Name = "bp";
            this.bp.ReadOnly = true;
            this.bp.Width = 20;
            // 
            // Line
            // 
            this.Line.Frozen = true;
            this.Line.HeaderText = "Line";
            this.Line.Name = "Line";
            this.Line.ReadOnly = true;
            this.Line.Width = 30;
            // 
            // Code
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Code.DefaultCellStyle = dataGridViewCellStyle1;
            this.Code.HeaderText = "Code";
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            this.Code.Width = 500;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(404, 355);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 11);
            this.label2.TabIndex = 16;
            this.label2.Text = "F6 Step Over | F7 Step In | F9 Flip Bp";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1037, 469);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataSource);
            this.Controls.Add(this.comboBoxFiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxVar);
            this.Controls.Add(this.buttonFolder);
            this.Controls.Add(this.buttonConfig);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1_filename);
            this.Controls.Add(this.button1clear);
            this.Controls.Add(this.label3_input);
            this.Controls.Add(this.label2_source);
            this.Controls.Add(this.label1_log);
            this.Controls.Add(this.textBox3_input);
            this.Controls.Add(this.textBox1_log);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "slag monitor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1_log;
        private System.Windows.Forms.TextBox textBox3_input;
        private System.Windows.Forms.Label label1_log;
        private System.Windows.Forms.Label label2_source;
        private System.Windows.Forms.Label label3_input;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1clear;
        private System.Windows.Forms.Label label1_filename;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button buttonConfig;
        private System.Windows.Forms.Button buttonFolder;
        private System.Windows.Forms.TextBox textBoxVar;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboBoxFiles;
        public System.Windows.Forms.DataGridView dataSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn bp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Line;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.Label label2;
    }
}

