namespace DocIntelAnalyzer
{
    partial class MainForm
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
            this.groupBoxFile = new GroupBox();
            this.textBoxFilePath = new TextBox();
            this.buttonBrowse = new Button();
            this.labelFilePath = new Label();
            this.buttonAnalyze = new Button();
            this.buttonSignPDF = new Button();
            this.groupBoxFilter = new GroupBox();
            this.radioButtonAll = new RadioButton();
            this.radioButtonKeyword = new RadioButton();
            this.textBoxKeyword = new TextBox();
            this.labelKeyword = new Label();
            this.groupBoxResults = new GroupBox();
            this.tabControl = new TabControl();
            this.tabPageWords = new TabPage();
            this.dataGridViewWords = new DataGridView();
            this.tabPageLines = new TabPage();
            this.dataGridViewLines = new DataGridView();
            this.statusStrip = new StatusStrip();
            this.toolStripStatusLabel = new ToolStripStatusLabel();
            this.progressBar = new ProgressBar();
            
            this.groupBoxFile.SuspendLayout();
            this.groupBoxFilter.SuspendLayout();
            this.groupBoxResults.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageWords.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWords)).BeginInit();
            this.tabPageLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLines)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // groupBoxFile
            // 
            this.groupBoxFile.Controls.Add(this.textBoxFilePath);
            this.groupBoxFile.Controls.Add(this.buttonBrowse);
            this.groupBoxFile.Controls.Add(this.labelFilePath);
            this.groupBoxFile.Controls.Add(this.buttonAnalyze);
            this.groupBoxFile.Controls.Add(this.buttonSignPDF);
            this.groupBoxFile.Location = new Point(12, 12);
            this.groupBoxFile.Name = "groupBoxFile";
            this.groupBoxFile.Size = new Size(1000, 100);
            this.groupBoxFile.TabIndex = 0;
            this.groupBoxFile.TabStop = false;
            this.groupBoxFile.Text = "PDF File Selection";
            
            // 
            // labelFilePath
            // 
            this.labelFilePath.AutoSize = true;
            this.labelFilePath.Location = new Point(15, 30);
            this.labelFilePath.Name = "labelFilePath";
            this.labelFilePath.Size = new Size(61, 15);
            this.labelFilePath.TabIndex = 0;
            this.labelFilePath.Text = "File Path:";
            
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new Point(85, 27);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new Size(750, 23);
            this.textBoxFilePath.TabIndex = 1;
            
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new Point(845, 26);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new Size(75, 25);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "Browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new EventHandler(this.buttonBrowse_Click);
            
            // 
            // buttonAnalyze
            // 
            this.buttonAnalyze.Location = new Point(85, 60);
            this.buttonAnalyze.Name = "buttonAnalyze";
            this.buttonAnalyze.Size = new Size(120, 30);
            this.buttonAnalyze.TabIndex = 3;
            this.buttonAnalyze.Text = "Analyze Document";
            this.buttonAnalyze.UseVisualStyleBackColor = true;
            this.buttonAnalyze.Click += new EventHandler(this.buttonAnalyze_Click);
            
            // 
            // buttonSignPDF
            // 
            this.buttonSignPDF.Location = new Point(220, 60);
            this.buttonSignPDF.Name = "buttonSignPDF";
            this.buttonSignPDF.Size = new Size(120, 30);
            this.buttonSignPDF.TabIndex = 4;
            this.buttonSignPDF.Text = "Sign PDF";
            this.buttonSignPDF.UseVisualStyleBackColor = true;
            this.buttonSignPDF.Click += new EventHandler(this.buttonSignPDF_Click);
            
            // 
            // groupBoxFilter
            // 
            this.groupBoxFilter.Controls.Add(this.radioButtonAll);
            this.groupBoxFilter.Controls.Add(this.radioButtonKeyword);
            this.groupBoxFilter.Controls.Add(this.textBoxKeyword);
            this.groupBoxFilter.Controls.Add(this.labelKeyword);
            this.groupBoxFilter.Location = new Point(12, 125);
            this.groupBoxFilter.Name = "groupBoxFilter";
            this.groupBoxFilter.Size = new Size(1000, 80);
            this.groupBoxFilter.TabIndex = 1;
            this.groupBoxFilter.TabStop = false;
            this.groupBoxFilter.Text = "Filter Options";
            
            // 
            // radioButtonAll
            // 
            this.radioButtonAll.AutoSize = true;
            this.radioButtonAll.Checked = true;
            this.radioButtonAll.Location = new Point(15, 25);
            this.radioButtonAll.Name = "radioButtonAll";
            this.radioButtonAll.Size = new Size(82, 19);
            this.radioButtonAll.TabIndex = 0;
            this.radioButtonAll.TabStop = true;
            this.radioButtonAll.Text = "Show All";
            this.radioButtonAll.UseVisualStyleBackColor = true;
            this.radioButtonAll.CheckedChanged += new EventHandler(this.radioButton_CheckedChanged);
            
            // 
            // radioButtonKeyword
            // 
            this.radioButtonKeyword.AutoSize = true;
            this.radioButtonKeyword.Location = new Point(15, 50);
            this.radioButtonKeyword.Name = "radioButtonKeyword";
            this.radioButtonKeyword.Size = new Size(71, 19);
            this.radioButtonKeyword.TabIndex = 1;
            this.radioButtonKeyword.Text = "Keyword:";
            this.radioButtonKeyword.UseVisualStyleBackColor = true;
            this.radioButtonKeyword.CheckedChanged += new EventHandler(this.radioButton_CheckedChanged);
            
            // 
            // labelKeyword
            // 
            this.labelKeyword.AutoSize = true;
            this.labelKeyword.Location = new Point(100, 52);
            this.labelKeyword.Name = "labelKeyword";
            this.labelKeyword.Size = new Size(58, 15);
            this.labelKeyword.TabIndex = 2;
            this.labelKeyword.Text = "Search:";
            
            // 
            // textBoxKeyword
            // 
            this.textBoxKeyword.Enabled = false;
            this.textBoxKeyword.Location = new Point(165, 49);
            this.textBoxKeyword.Name = "textBoxKeyword";
            this.textBoxKeyword.Size = new Size(300, 23);
            this.textBoxKeyword.TabIndex = 3;
            this.textBoxKeyword.TextChanged += new EventHandler(this.textBoxKeyword_TextChanged);
            
            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(12, 215);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(1000, 10);
            this.progressBar.Style = ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 2;
            this.progressBar.Visible = false;
            
            // 
            // groupBoxResults
            // 
            this.groupBoxResults.Controls.Add(this.tabControl);
            this.groupBoxResults.Location = new Point(12, 235);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Size = new Size(1000, 450);
            this.groupBoxResults.TabIndex = 3;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Analysis Results";
            
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageWords);
            this.tabControl.Controls.Add(this.tabPageLines);
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new Point(3, 19);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(994, 428);
            this.tabControl.TabIndex = 0;
            
            // 
            // tabPageWords
            // 
            this.tabPageWords.Controls.Add(this.dataGridViewWords);
            this.tabPageWords.Location = new Point(4, 24);
            this.tabPageWords.Name = "tabPageWords";
            this.tabPageWords.Padding = new Padding(3);
            this.tabPageWords.Size = new Size(986, 400);
            this.tabPageWords.TabIndex = 0;
            this.tabPageWords.Text = "Words";
            this.tabPageWords.UseVisualStyleBackColor = true;
            
            // 
            // dataGridViewWords
            // 
            this.dataGridViewWords.AllowUserToAddRows = false;
            this.dataGridViewWords.AllowUserToDeleteRows = false;
            this.dataGridViewWords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewWords.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewWords.Dock = DockStyle.Fill;
            this.dataGridViewWords.Location = new Point(3, 3);
            this.dataGridViewWords.Name = "dataGridViewWords";
            this.dataGridViewWords.ReadOnly = true;
            this.dataGridViewWords.RowTemplate.Height = 25;
            this.dataGridViewWords.Size = new Size(980, 394);
            this.dataGridViewWords.TabIndex = 0;
            
            // 
            // tabPageLines
            // 
            this.tabPageLines.Controls.Add(this.dataGridViewLines);
            this.tabPageLines.Location = new Point(4, 24);
            this.tabPageLines.Name = "tabPageLines";
            this.tabPageLines.Padding = new Padding(3);
            this.tabPageLines.Size = new Size(986, 400);
            this.tabPageLines.TabIndex = 1;
            this.tabPageLines.Text = "Lines";
            this.tabPageLines.UseVisualStyleBackColor = true;
            
            // 
            // dataGridViewLines
            // 
            this.dataGridViewLines.AllowUserToAddRows = false;
            this.dataGridViewLines.AllowUserToDeleteRows = false;
            this.dataGridViewLines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewLines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLines.Dock = DockStyle.Fill;
            this.dataGridViewLines.Location = new Point(3, 3);
            this.dataGridViewLines.Name = "dataGridViewLines";
            this.dataGridViewLines.ReadOnly = true;
            this.dataGridViewLines.RowTemplate.Height = 25;
            this.dataGridViewLines.Size = new Size(980, 394);
            this.dataGridViewLines.TabIndex = 0;
            
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new ToolStripItem[] { this.toolStripStatusLabel });
            this.statusStrip.Location = new Point(0, 695);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(1024, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new Size(39, 17);
            this.toolStripStatusLabel.Text = "Ready";
            
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1024, 717);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBoxResults);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.groupBoxFilter);
            this.Controls.Add(this.groupBoxFile);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Document Intelligence Analyzer";
            this.groupBoxFile.ResumeLayout(false);
            this.groupBoxFile.PerformLayout();
            this.groupBoxFilter.ResumeLayout(false);
            this.groupBoxFilter.PerformLayout();
            this.groupBoxResults.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageWords.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWords)).EndInit();
            this.tabPageLines.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLines)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private GroupBox groupBoxFile;
        private TextBox textBoxFilePath;
        private Button buttonBrowse;
        private Label labelFilePath;
        private Button buttonAnalyze;
        private Button buttonSignPDF;
        private GroupBox groupBoxFilter;
        private RadioButton radioButtonAll;
        private RadioButton radioButtonKeyword;
        private TextBox textBoxKeyword;
        private Label labelKeyword;
        private ProgressBar progressBar;
        private GroupBox groupBoxResults;
        private TabControl tabControl;
        private TabPage tabPageWords;
        private DataGridView dataGridViewWords;
        private TabPage tabPageLines;
        private DataGridView dataGridViewLines;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
    }
}
