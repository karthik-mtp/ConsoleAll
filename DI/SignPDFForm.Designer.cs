namespace DocIntelAnalyzer
{
    partial class SignPDFForm
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

        private void InitializeComponent()
        {
            groupBoxConfiguration = new GroupBox();
            btnGeneratePayLoad = new Button();
            radioButtonAutoConfig = new RadioButton();
            radioButtonManualConfig = new RadioButton();
            groupBoxAutoConfig = new GroupBox();
            labelSearchKey = new Label();
            textBoxSearchKey = new TextBox();
            labelSignaturePlacement = new Label();
            comboBoxSignaturePlacement = new ComboBox();
            labelOffsetX = new Label();
            textBoxOffsetX = new TextBox();
            labelOffsetY = new Label();
            textBoxOffsetY = new TextBox();
            groupBoxFindIn = new GroupBox();
            radioButtonWords = new RadioButton();
            radioButtonLines = new RadioButton();
            groupBoxMatchType = new GroupBox();
            radioButtonContains = new RadioButton();
            radioButtonExactMatch = new RadioButton();
            labelNthOccurrence = new Label();
            numericUpDownNthOccurrence = new NumericUpDown();
            groupBoxManualConfig = new GroupBox();
            labelManualCoords = new Label();
            textBoxManualCoords = new TextBox();
            groupBoxPayload = new GroupBox();
            textBoxPayload = new TextBox();
            buttonSign = new Button();
            statusStripSign = new StatusStrip();
            toolStripStatusLabelSign = new ToolStripStatusLabel();
            groupBoxConfiguration.SuspendLayout();
            groupBoxAutoConfig.SuspendLayout();
            groupBoxFindIn.SuspendLayout();
            groupBoxMatchType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownNthOccurrence).BeginInit();
            groupBoxManualConfig.SuspendLayout();
            groupBoxPayload.SuspendLayout();
            statusStripSign.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxConfiguration
            // 
            groupBoxConfiguration.Controls.Add(btnGeneratePayLoad);
            groupBoxConfiguration.Controls.Add(radioButtonAutoConfig);
            groupBoxConfiguration.Controls.Add(radioButtonManualConfig);
            groupBoxConfiguration.Controls.Add(groupBoxAutoConfig);
            groupBoxConfiguration.Controls.Add(groupBoxManualConfig);
            groupBoxConfiguration.Controls.Add(groupBoxPayload);
            groupBoxConfiguration.Controls.Add(buttonSign);
            groupBoxConfiguration.Location = new Point(17, 20);
            groupBoxConfiguration.Margin = new Padding(4, 5, 4, 5);
            groupBoxConfiguration.Name = "groupBoxConfiguration";
            groupBoxConfiguration.Padding = new Padding(4, 5, 4, 5);
            groupBoxConfiguration.Size = new Size(1214, 933);
            groupBoxConfiguration.TabIndex = 1;
            groupBoxConfiguration.TabStop = false;
            groupBoxConfiguration.Text = "Signature Placement Configuration";
            // 
            // btnGeneratePayLoad
            // 
            btnGeneratePayLoad.Location = new Point(97, 873);
            btnGeneratePayLoad.Name = "btnGeneratePayLoad";
            btnGeneratePayLoad.Size = new Size(214, 34);
            btnGeneratePayLoad.TabIndex = 7;
            btnGeneratePayLoad.Text = "Generate Payload";
            btnGeneratePayLoad.UseVisualStyleBackColor = true;
            btnGeneratePayLoad.Click += btnGeneratePayLoad_Click;
            // 
            // radioButtonAutoConfig
            // 
            radioButtonAutoConfig.AutoSize = true;
            radioButtonAutoConfig.Checked = true;
            radioButtonAutoConfig.Location = new Point(21, 42);
            radioButtonAutoConfig.Margin = new Padding(4, 5, 4, 5);
            radioButtonAutoConfig.Name = "radioButtonAutoConfig";
            radioButtonAutoConfig.Size = new Size(331, 29);
            radioButtonAutoConfig.TabIndex = 0;
            radioButtonAutoConfig.TabStop = true;
            radioButtonAutoConfig.Text = "Auto Configuration (Keyword Search)";
            radioButtonAutoConfig.UseVisualStyleBackColor = true;
            radioButtonAutoConfig.CheckedChanged += radioButtonConfig_CheckedChanged;
            // 
            // radioButtonManualConfig
            // 
            radioButtonManualConfig.AutoSize = true;
            radioButtonManualConfig.Location = new Point(357, 42);
            radioButtonManualConfig.Margin = new Padding(4, 5, 4, 5);
            radioButtonManualConfig.Name = "radioButtonManualConfig";
            radioButtonManualConfig.Size = new Size(249, 29);
            radioButtonManualConfig.TabIndex = 1;
            radioButtonManualConfig.Text = "Manual Configuration (X,Y)";
            radioButtonManualConfig.UseVisualStyleBackColor = true;
            radioButtonManualConfig.CheckedChanged += radioButtonConfig_CheckedChanged;
            // 
            // groupBoxAutoConfig
            // 
            groupBoxAutoConfig.Controls.Add(labelSearchKey);
            groupBoxAutoConfig.Controls.Add(textBoxSearchKey);
            groupBoxAutoConfig.Controls.Add(labelSignaturePlacement);
            groupBoxAutoConfig.Controls.Add(comboBoxSignaturePlacement);
            groupBoxAutoConfig.Controls.Add(labelOffsetX);
            groupBoxAutoConfig.Controls.Add(textBoxOffsetX);
            groupBoxAutoConfig.Controls.Add(labelOffsetY);
            groupBoxAutoConfig.Controls.Add(textBoxOffsetY);
            groupBoxAutoConfig.Controls.Add(groupBoxFindIn);
            groupBoxAutoConfig.Controls.Add(groupBoxMatchType);
            groupBoxAutoConfig.Controls.Add(labelNthOccurrence);
            groupBoxAutoConfig.Controls.Add(numericUpDownNthOccurrence);
            groupBoxAutoConfig.Location = new Point(21, 83);
            groupBoxAutoConfig.Margin = new Padding(4, 5, 4, 5);
            groupBoxAutoConfig.Name = "groupBoxAutoConfig";
            groupBoxAutoConfig.Padding = new Padding(4, 5, 4, 5);
            groupBoxAutoConfig.Size = new Size(1100, 291);
            groupBoxAutoConfig.TabIndex = 2;
            groupBoxAutoConfig.TabStop = false;
            groupBoxAutoConfig.Text = "Keyword-Based Configuration";
            // 
            // labelSearchKey
            // 
            labelSearchKey.AutoSize = true;
            labelSearchKey.Location = new Point(21, 50);
            labelSearchKey.Margin = new Padding(4, 0, 4, 0);
            labelSearchKey.Name = "labelSearchKey";
            labelSearchKey.Size = new Size(101, 25);
            labelSearchKey.TabIndex = 0;
            labelSearchKey.Text = "Search Key:";
            // 
            // textBoxSearchKey
            // 
            textBoxSearchKey.Location = new Point(143, 45);
            textBoxSearchKey.Margin = new Padding(4, 5, 4, 5);
            textBoxSearchKey.Name = "textBoxSearchKey";
            textBoxSearchKey.Size = new Size(284, 31);
            textBoxSearchKey.TabIndex = 1;
            textBoxSearchKey.Text = "signature";
            // 
            // labelSignaturePlacement
            // 
            labelSignaturePlacement.AutoSize = true;
            labelSignaturePlacement.Location = new Point(457, 50);
            labelSignaturePlacement.Margin = new Padding(4, 0, 4, 0);
            labelSignaturePlacement.Name = "labelSignaturePlacement";
            labelSignaturePlacement.Size = new Size(177, 25);
            labelSignaturePlacement.TabIndex = 2;
            labelSignaturePlacement.Text = "Signature Placement:";
            // 
            // comboBoxSignaturePlacement
            // 
            comboBoxSignaturePlacement.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSignaturePlacement.FormattingEnabled = true;
            comboBoxSignaturePlacement.Items.AddRange(new object[] { "Top Left", "Top Right", "Bottom Left", "Bottom Right" });
            comboBoxSignaturePlacement.Location = new Point(643, 45);
            comboBoxSignaturePlacement.Margin = new Padding(4, 5, 4, 5);
            comboBoxSignaturePlacement.Name = "comboBoxSignaturePlacement";
            comboBoxSignaturePlacement.SelectedIndex = 0; // Default to "Top Left"
            comboBoxSignaturePlacement.Size = new Size(170, 33);
            comboBoxSignaturePlacement.TabIndex = 3;
            // 
            // labelOffsetX
            // 
            labelOffsetX.AutoSize = true;
            labelOffsetX.Location = new Point(843, 50);
            labelOffsetX.Margin = new Padding(4, 0, 4, 0);
            labelOffsetX.Name = "labelOffsetX";
            labelOffsetX.Size = new Size(81, 25);
            labelOffsetX.TabIndex = 4;
            labelOffsetX.Text = "Offset X:";
            // 
            // textBoxOffsetX
            // 
            textBoxOffsetX.Location = new Point(929, 45);
            textBoxOffsetX.Margin = new Padding(4, 5, 4, 5);
            textBoxOffsetX.Name = "textBoxOffsetX";
            textBoxOffsetX.Size = new Size(70, 31);
            textBoxOffsetX.TabIndex = 5;
            textBoxOffsetX.Text = "0.2";
            // 
            // labelOffsetY
            // 
            labelOffsetY.AutoSize = true;
            labelOffsetY.Location = new Point(843, 100);
            labelOffsetY.Margin = new Padding(4, 0, 4, 0);
            labelOffsetY.Name = "labelOffsetY";
            labelOffsetY.Size = new Size(80, 25);
            labelOffsetY.TabIndex = 6;
            labelOffsetY.Text = "Offset Y:";
            // 
            // textBoxOffsetY
            // 
            textBoxOffsetY.Location = new Point(929, 95);
            textBoxOffsetY.Margin = new Padding(4, 5, 4, 5);
            textBoxOffsetY.Name = "textBoxOffsetY";
            textBoxOffsetY.Size = new Size(70, 31);
            textBoxOffsetY.TabIndex = 7;
            textBoxOffsetY.Text = "0.2";
            // 
            // groupBoxFindIn
            // 
            groupBoxFindIn.Controls.Add(radioButtonWords);
            groupBoxFindIn.Controls.Add(radioButtonLines);
            groupBoxFindIn.Location = new Point(21, 150);
            groupBoxFindIn.Margin = new Padding(4, 5, 4, 5);
            groupBoxFindIn.Name = "groupBoxFindIn";
            groupBoxFindIn.Padding = new Padding(4, 5, 4, 5);
            groupBoxFindIn.Size = new Size(286, 133);
            groupBoxFindIn.TabIndex = 8;
            groupBoxFindIn.TabStop = false;
            groupBoxFindIn.Text = "Find Text In";
            // 
            // radioButtonWords
            // 
            radioButtonWords.AutoSize = true;
            radioButtonWords.Location = new Point(21, 42);
            radioButtonWords.Margin = new Padding(4, 5, 4, 5);
            radioButtonWords.Name = "radioButtonWords";
            radioButtonWords.Size = new Size(89, 29);
            radioButtonWords.TabIndex = 0;
            radioButtonWords.Text = "Words";
            radioButtonWords.UseVisualStyleBackColor = true;
            // 
            // radioButtonLines
            // 
            radioButtonLines.AutoSize = true;
            radioButtonLines.Checked = true;
            radioButtonLines.Location = new Point(21, 83);
            radioButtonLines.Margin = new Padding(4, 5, 4, 5);
            radioButtonLines.Name = "radioButtonLines";
            radioButtonLines.Size = new Size(76, 29);
            radioButtonLines.TabIndex = 1;
            radioButtonLines.TabStop = true;
            radioButtonLines.Text = "Lines";
            radioButtonLines.UseVisualStyleBackColor = true;
            // 
            // groupBoxMatchType
            // 
            groupBoxMatchType.Controls.Add(radioButtonContains);
            groupBoxMatchType.Controls.Add(radioButtonExactMatch);
            groupBoxMatchType.Location = new Point(343, 150);
            groupBoxMatchType.Margin = new Padding(4, 5, 4, 5);
            groupBoxMatchType.Name = "groupBoxMatchType";
            groupBoxMatchType.Padding = new Padding(4, 5, 4, 5);
            groupBoxMatchType.Size = new Size(286, 133);
            groupBoxMatchType.TabIndex = 9;
            groupBoxMatchType.TabStop = false;
            groupBoxMatchType.Text = "Match Type";
            // 
            // radioButtonContains
            // 
            radioButtonContains.AutoSize = true;
            radioButtonContains.Location = new Point(21, 42);
            radioButtonContains.Margin = new Padding(4, 5, 4, 5);
            radioButtonContains.Name = "radioButtonContains";
            radioButtonContains.Size = new Size(106, 29);
            radioButtonContains.TabIndex = 0;
            radioButtonContains.Text = "Contains";
            radioButtonContains.UseVisualStyleBackColor = true;
            // 
            // radioButtonExactMatch
            // 
            radioButtonExactMatch.AutoSize = true;
            radioButtonExactMatch.Checked = true;
            radioButtonExactMatch.Location = new Point(21, 83);
            radioButtonExactMatch.Margin = new Padding(4, 5, 4, 5);
            radioButtonExactMatch.Name = "radioButtonExactMatch";
            radioButtonExactMatch.Size = new Size(131, 29);
            radioButtonExactMatch.TabIndex = 1;
            radioButtonExactMatch.TabStop = true;
            radioButtonExactMatch.Text = "Exact Match";
            radioButtonExactMatch.UseVisualStyleBackColor = true;
            // 
            // labelNthOccurrence
            // 
            labelNthOccurrence.AutoSize = true;
            labelNthOccurrence.Location = new Point(657, 200);
            labelNthOccurrence.Margin = new Padding(4, 0, 4, 0);
            labelNthOccurrence.Name = "labelNthOccurrence";
            labelNthOccurrence.Size = new Size(134, 50);
            labelNthOccurrence.TabIndex = 10;
            labelNthOccurrence.Text = "Nth Occurrence\r\n in a Page:\r\n";
            // 
            // numericUpDownNthOccurrence
            // 
            numericUpDownNthOccurrence.Location = new Point(814, 197);
            numericUpDownNthOccurrence.Margin = new Padding(4, 5, 4, 5);
            numericUpDownNthOccurrence.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownNthOccurrence.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownNthOccurrence.Name = "numericUpDownNthOccurrence";
            numericUpDownNthOccurrence.Size = new Size(86, 31);
            numericUpDownNthOccurrence.TabIndex = 11;
            numericUpDownNthOccurrence.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // groupBoxManualConfig
            // 
            groupBoxManualConfig.Controls.Add(labelManualCoords);
            groupBoxManualConfig.Controls.Add(textBoxManualCoords);
            groupBoxManualConfig.Enabled = false;
            groupBoxManualConfig.Location = new Point(21, 400);
            groupBoxManualConfig.Margin = new Padding(4, 5, 4, 5);
            groupBoxManualConfig.Name = "groupBoxManualConfig";
            groupBoxManualConfig.Padding = new Padding(4, 5, 4, 5);
            groupBoxManualConfig.Size = new Size(1100, 117);
            groupBoxManualConfig.TabIndex = 3;
            groupBoxManualConfig.TabStop = false;
            groupBoxManualConfig.Text = "Manual Coordinates";
            // 
            // labelManualCoords
            // 
            labelManualCoords.AutoSize = true;
            labelManualCoords.Location = new Point(21, 42);
            labelManualCoords.Margin = new Padding(4, 0, 4, 0);
            labelManualCoords.Name = "labelManualCoords";
            labelManualCoords.Size = new Size(228, 25);
            labelManualCoords.TabIndex = 0;
            labelManualCoords.Text = "Format: page=x,y|page=x,y";
            // 
            // textBoxManualCoords
            // 
            textBoxManualCoords.Location = new Point(21, 75);
            textBoxManualCoords.Margin = new Padding(4, 5, 4, 5);
            textBoxManualCoords.Name = "textBoxManualCoords";
            textBoxManualCoords.Size = new Size(1055, 31);
            textBoxManualCoords.TabIndex = 1;
            textBoxManualCoords.Text = "1=100.5,200.3|2=150.7,250.9";
            // 
            // groupBoxPayload
            // 
            groupBoxPayload.Controls.Add(textBoxPayload);
            groupBoxPayload.Location = new Point(21, 529);
            groupBoxPayload.Margin = new Padding(4, 5, 4, 5);
            groupBoxPayload.Name = "groupBoxPayload";
            groupBoxPayload.Padding = new Padding(4, 5, 4, 5);
            groupBoxPayload.Size = new Size(1171, 317);
            groupBoxPayload.TabIndex = 5;
            groupBoxPayload.TabStop = false;
            groupBoxPayload.Text = "Signature Placement Info (API Payload)";
            // 
            // textBoxPayload
            // 
            textBoxPayload.BackColor = Color.LightYellow;
            textBoxPayload.Dock = DockStyle.Fill;
            textBoxPayload.Font = new Font("Consolas", 9F);
            textBoxPayload.Location = new Point(4, 29);
            textBoxPayload.Margin = new Padding(4, 5, 4, 5);
            textBoxPayload.Multiline = true;
            textBoxPayload.Name = "textBoxPayload";
            textBoxPayload.ReadOnly = true;
            textBoxPayload.ScrollBars = ScrollBars.Both;
            textBoxPayload.Size = new Size(1163, 283);
            textBoxPayload.TabIndex = 0;
            textBoxPayload.WordWrap = false;
            // 
            // buttonSign
            // 
            buttonSign.BackColor = Color.LightGreen;
            buttonSign.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            buttonSign.Location = new Point(521, 856);
            buttonSign.Margin = new Padding(4, 5, 4, 5);
            buttonSign.Name = "buttonSign";
            buttonSign.Size = new Size(171, 67);
            buttonSign.TabIndex = 6;
            buttonSign.Text = "Sign PDF";
            buttonSign.UseVisualStyleBackColor = false;
            buttonSign.Click += buttonSign_Click;
            // 
            // 
            // statusStripSign
            // 
            statusStripSign.ImageScalingSize = new Size(24, 24);
            statusStripSign.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelSign });
            statusStripSign.Location = new Point(0, 980);
            statusStripSign.Name = "statusStripSign";
            statusStripSign.Padding = new Padding(1, 0, 20, 0);
            statusStripSign.Size = new Size(1924, 32);
            statusStripSign.TabIndex = 3;
            statusStripSign.Text = "statusStrip1";
            // 
            // toolStripStatusLabelSign
            // 
            toolStripStatusLabelSign.Name = "toolStripStatusLabelSign";
            toolStripStatusLabelSign.Size = new Size(60, 25);
            toolStripStatusLabelSign.Text = "Ready";
            // 
            // SignPDFForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1250, 1012);
            Controls.Add(statusStripSign);
            Controls.Add(groupBoxConfiguration);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            Name = "SignPDFForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Sign PDF - DocuSign Integration";
            groupBoxConfiguration.ResumeLayout(false);
            groupBoxConfiguration.PerformLayout();
            groupBoxAutoConfig.ResumeLayout(false);
            groupBoxAutoConfig.PerformLayout();
            groupBoxFindIn.ResumeLayout(false);
            groupBoxFindIn.PerformLayout();
            groupBoxMatchType.ResumeLayout(false);
            groupBoxMatchType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownNthOccurrence).EndInit();
            groupBoxManualConfig.ResumeLayout(false);
            groupBoxManualConfig.PerformLayout();
            groupBoxPayload.ResumeLayout(false);
            groupBoxPayload.PerformLayout();
            statusStripSign.ResumeLayout(false);
            statusStripSign.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBoxConfiguration;
        private GroupBox groupBoxAutoConfig;
        private Label labelSearchKey;
        private TextBox textBoxSearchKey;
        private Label labelSignaturePlacement;
        private ComboBox comboBoxSignaturePlacement;
        private Label labelOffsetX;
        private TextBox textBoxOffsetX;
        private Label labelOffsetY;
        private TextBox textBoxOffsetY;
        private GroupBox groupBoxFindIn;
        private RadioButton radioButtonWords;
        private RadioButton radioButtonLines;
        private GroupBox groupBoxMatchType;
        private RadioButton radioButtonContains;
        private RadioButton radioButtonExactMatch;
        private Label labelNthOccurrence;
        private NumericUpDown numericUpDownNthOccurrence;
        private GroupBox groupBoxManualConfig;
        private Label labelManualCoords;
        private TextBox textBoxManualCoords;
        private RadioButton radioButtonAutoConfig;
        private RadioButton radioButtonManualConfig;
        private Button buttonSign;
        private GroupBox groupBoxPayload;
        private TextBox textBoxPayload;
        private StatusStrip statusStripSign;
        private ToolStripStatusLabel toolStripStatusLabelSign;
        private Button btnGeneratePayLoad;
    }
}
