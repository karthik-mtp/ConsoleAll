using DocIntelAnalyzer.Configuration;
using DocIntelAnalyzer.Services;

namespace DocIntelAnalyzer
{
    public partial class MainForm : Form
    {
        private DocumentIntelligenceService? _documentService;
        private DocumentAnalysisResult? _analysisResult;
        private AppConfiguration _config = new AppConfiguration();

        public MainForm()
        {
            InitializeComponent();
            LoadConfiguration();
            InitializeDataGridViews();
        }

        private void LoadConfiguration()
        {
            try
            {
                _config = ConfigurationHelper.LoadConfiguration();
                
                if (string.IsNullOrEmpty(_config.DocumentIntelligence.Endpoint) || 
                    string.IsNullOrEmpty(_config.DocumentIntelligence.ApiKey))
                {
                    MessageBox.Show("Please configure your Document Intelligence endpoint and API key in appsettings.json", 
                        "Configuration Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _documentService = new DocumentIntelligenceService(
                    _config.DocumentIntelligence.Endpoint,
                    _config.DocumentIntelligence.ApiKey);

                toolStripStatusLabel.Text = "Configuration loaded successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", 
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDataGridViews()
        {
            // Initialize Words DataGridView
            dataGridViewWords.Columns.Add("PageNumber", "Page");
            dataGridViewWords.Columns.Add("Content", "Word");
            dataGridViewWords.Columns.Add("Polygon", "Polygon Coordinates");
            dataGridViewWords.Columns.Add("Confidence", "Confidence");

            dataGridViewWords.Columns["PageNumber"].Width = 60;
            dataGridViewWords.Columns["Content"].Width = 200;
            dataGridViewWords.Columns["Polygon"].Width = 400;
            dataGridViewWords.Columns["Confidence"].Width = 80;

            // Initialize Lines DataGridView
            dataGridViewLines.Columns.Add("PageNumber", "Page");
            dataGridViewLines.Columns.Add("Content", "Line");
            dataGridViewLines.Columns.Add("Polygon", "Polygon Coordinates");

            dataGridViewLines.Columns["PageNumber"].Width = 60;
            dataGridViewLines.Columns["Content"].Width = 500;
            dataGridViewLines.Columns["Polygon"].Width = 400;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            openFileDialog.Title = "Select PDF file to analyze";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = openFileDialog.FileName;
            }
        }

        private async void buttonAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxFilePath.Text))
            {
                MessageBox.Show("Please select a PDF file to analyze.", "File Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(textBoxFilePath.Text))
            {
                MessageBox.Show("The selected file does not exist.", "File Not Found", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_documentService == null)
            {
                MessageBox.Show("Document Intelligence service is not configured.", "Service Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Show progress
                SetUIState(false);
                toolStripStatusLabel.Text = "Analyzing document...";
                progressBar.Visible = true;

                // Analyze document
                _analysisResult = await _documentService.AnalyzeDocumentAsync(textBoxFilePath.Text);

                // Update UI
                RefreshDataGridViews();
                toolStripStatusLabel.Text = $"Analysis complete. Found {GetTotalWords()} words and {GetTotalLines()} lines.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error analyzing document: {ex.Message}", "Analysis Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel.Text = "Analysis failed";
            }
            finally
            {
                SetUIState(true);
                progressBar.Visible = false;
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            textBoxKeyword.Enabled = radioButtonKeyword.Checked;
            
            if (radioButtonAll.Checked)
            {
                textBoxKeyword.Clear();
            }
            
            RefreshDataGridViews();
        }

        private void textBoxKeyword_TextChanged(object sender, EventArgs e)
        {
            if (radioButtonKeyword.Checked)
            {
                RefreshDataGridViews();
            }
        }

        private void RefreshDataGridViews()
        {
            if (_analysisResult == null) return;

            var keyword = radioButtonKeyword.Checked ? textBoxKeyword.Text.Trim() : string.Empty;
            
            RefreshWordsDataGridView(keyword);
            RefreshLinesDataGridView(keyword);
        }

        private void RefreshWordsDataGridView(string keyword)
        {
            dataGridViewWords.Rows.Clear();

            foreach (var page in _analysisResult!.Pages)
            {
                var filteredWords = string.IsNullOrEmpty(keyword) 
                    ? page.Words 
                    : page.Words.Where(w => w.Content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

                foreach (var word in filteredWords)
                {
                    dataGridViewWords.Rows.Add(
                        word.PageNumber,
                        word.Content,
                        word.Polygon,
                        word.Confidence
                    );
                }
            }

            // Update tab text with count
            tabPageWords.Text = $"Words ({dataGridViewWords.Rows.Count})";
        }

        private void RefreshLinesDataGridView(string keyword)
        {
            dataGridViewLines.Rows.Clear();

            foreach (var page in _analysisResult!.Pages)
            {
                var filteredLines = string.IsNullOrEmpty(keyword) 
                    ? page.Lines 
                    : page.Lines.Where(l => l.Content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

                foreach (var line in filteredLines)
                {
                    dataGridViewLines.Rows.Add(
                        line.PageNumber,
                        line.Content,
                        line.Polygon
                    );
                }
            }

            // Update tab text with count
            tabPageLines.Text = $"Lines ({dataGridViewLines.Rows.Count})";
        }

        private void SetUIState(bool enabled)
        {
            buttonAnalyze.Enabled = enabled;
            buttonBrowse.Enabled = enabled;
            textBoxFilePath.Enabled = enabled;
            radioButtonAll.Enabled = enabled;
            radioButtonKeyword.Enabled = enabled;
            textBoxKeyword.Enabled = enabled && radioButtonKeyword.Checked;
        }

        private int GetTotalWords()
        {
            return _analysisResult?.Pages.Sum(p => p.Words.Count) ?? 0;
        }

        private int GetTotalLines()
        {
            return _analysisResult?.Pages.Sum(p => p.Lines.Count) ?? 0;
        }

        private void buttonSignPDF_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxFilePath.Text))
            {
                MessageBox.Show("Please select a PDF file first.", "File Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(textBoxFilePath.Text))
            {
                MessageBox.Show("The selected file does not exist.", "File Not Found", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_analysisResult == null)
            {
                MessageBox.Show("Please analyze the document first by clicking 'Analyze'.", "Analysis Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var signForm = new SignPDFForm(textBoxFilePath.Text, _analysisResult);
            signForm.ShowDialog();
        }
    }
}
