using System.Text.Json;
using System.Text;
using System.Diagnostics;

namespace DocIntelAnalyzer
{
    public partial class SignPDFForm : Form
    {
        private string _pdfFilePath;
        private Services.DocumentAnalysisResult _analysisResult;

        public SignPDFForm(string pdfFilePath, Services.DocumentAnalysisResult analysisResult)
        {
            InitializeComponent();
            _pdfFilePath = pdfFilePath;
            _analysisResult = analysisResult;
            this.Text = $"Sign PDF - {Path.GetFileName(pdfFilePath)}";

            // Wire up event handlers for real-time payload updates
            SetupPayloadEventHandlers();

            // Generate initial payload
            UpdatePayloadPreview();
        }

        private void radioButtonConfig_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxAutoConfig.Enabled = radioButtonAutoConfig.Checked;
            groupBoxManualConfig.Enabled = radioButtonManualConfig.Checked;
            UpdatePayloadPreview();
        }

        private void SetupPayloadEventHandlers()
        {
            // Auto configuration event handlers
            textBoxSearchKey.TextChanged += (s, e) => UpdatePayloadPreview();
            comboBoxSignaturePlacement.SelectedIndexChanged += (s, e) => UpdatePayloadPreview();
            textBoxOffsetX.TextChanged += (s, e) => UpdatePayloadPreview();
            textBoxOffsetY.TextChanged += (s, e) => UpdatePayloadPreview();
            radioButtonWords.CheckedChanged += (s, e) => UpdatePayloadPreview();
            radioButtonLines.CheckedChanged += (s, e) => UpdatePayloadPreview();
            radioButtonContains.CheckedChanged += (s, e) => UpdatePayloadPreview();
            radioButtonExactMatch.CheckedChanged += (s, e) => UpdatePayloadPreview();
            numericUpDownNthOccurrence.ValueChanged += (s, e) => UpdatePayloadPreview();

            // Manual configuration event handlers
            textBoxManualCoords.TextChanged += (s, e) => UpdatePayloadPreview();
        }

        private void UpdatePayloadPreview()
        {
            try
            {
                var signaturePlacementInfo = GetSignaturePlacementInfo();
                var json = JsonSerializer.Serialize(signaturePlacementInfo, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                textBoxPayload.Text = json;
            }
            catch (Exception ex)
            {
                textBoxPayload.Text = $"Error generating payload: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets the signature placement information based on current form configuration
        /// </summary>
        /// <returns>Array of signature placement positions</returns>
        public object[] GetSignaturePlacementInfo()
        {
            try
            {
                // Create parameters from form inputs
                var parameters = CreateSignaturePlacementParameters();
                
                // Use the new service to get signature placement info
                var results = SignaturePlacementService.GetSignaturePlacementInfo(parameters, _analysisResult);
                
                // Convert to object array for JSON serialization
                return results.Select(r => new
                {
                    pageNumber = r.PageNumber,
                    x = r.X,
                    y = r.Y
                }).ToArray();
            }
            catch (Exception ex)
            {
                return new[] { new { error = ex.Message } };
            }
        }

        /// <summary>
        /// Create SignaturePlacementParameters from current form inputs
        /// </summary>
        private SignaturePlacementParameters CreateSignaturePlacementParameters()
        {
            return new SignaturePlacementParameters
            {
                Mode = radioButtonAutoConfig.Checked ? SignaturePlacementMode.Auto : SignaturePlacementMode.Manual,
                
                // Auto configuration parameters
                SearchKeyword = textBoxSearchKey.Text.Trim(),
                ContentType = radioButtonWords.Checked ? SearchContentType.Words : SearchContentType.Lines,
                MatchType = radioButtonExactMatch.Checked ? SearchMatchType.ExactMatch : SearchMatchType.Contains,
                NthOccurrence = (int)numericUpDownNthOccurrence.Value,
                Placement = GetSignaturePlacementFromComboBox(),
                OffsetX = GetDoubleFromTextBox(textBoxOffsetX.Text),
                OffsetY = GetDoubleFromTextBox(textBoxOffsetY.Text),
                
                // Manual configuration parameters
                ManualCoordinates = textBoxManualCoords.Text.Trim()
            };
        }

        /// <summary>
        /// Get SignaturePlacement enum from combo box selection
        /// </summary>
        private SignaturePlacement GetSignaturePlacementFromComboBox()
        {
            string placement = comboBoxSignaturePlacement.SelectedItem?.ToString() ?? "Top Left";
            return placement switch
            {
                "Top Left" => SignaturePlacement.TopLeft,
                "Top Right" => SignaturePlacement.TopRight,
                "Bottom Left" => SignaturePlacement.BottomLeft,
                "Bottom Right" => SignaturePlacement.BottomRight,
                _ => SignaturePlacement.TopLeft
            };
        }

        /// <summary>
        /// Safely parse double from text box
        /// </summary>
        private double GetDoubleFromTextBox(string text)
        {
            return double.TryParse(text, out double value) ? value : 0.0;
        }

        private async void buttonSign_Click(object sender, EventArgs e)
        {
            try
            {
                SetUIState(false);
                toolStripStatusLabelSign.Text = "Processing signature placement...";

                // Use the unified service to get signature positions
                var signaturePositions = GetSignaturePositions();

                if (!signaturePositions.Any())
                {
                    MessageBox.Show("No signature positions found. Please check your configuration.",
                        "No Positions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                toolStripStatusLabelSign.Text = "Opening signature URL in browser...";
                string signatureUrl = await GenerateSignatureURL(signaturePositions);

                if (!string.IsNullOrEmpty(signatureUrl))
                {
                    // Open URL in default browser
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = signatureUrl,
                        UseShellExecute = true
                    });
                    
                    toolStripStatusLabelSign.Text = $"Signature URL opened in browser. Found {signaturePositions.Count} signature positions.";
                    
                    MessageBox.Show($"Signature URL has been opened in your default browser.\n\nFound {signaturePositions.Count} signature position(s).",
                        "URL Opened", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to generate signature URL.", "URL Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toolStripStatusLabelSign.Text = "URL generation failed.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing signature: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabelSign.Text = "Error occurred.";
            }
            finally
            {
                SetUIState(true);
            }
        }

        /// <summary>
        /// Get signature positions using the unified SignaturePlacementService
        /// </summary>
        private List<SignaturePosition> GetSignaturePositions()
        {
            try
            {
                // Create parameters from form inputs
                var parameters = CreateSignaturePlacementParameters();
                
                // Use the new service to get signature placement info
                var results = SignaturePlacementService.GetSignaturePlacementInfo(parameters, _analysisResult);
                
                // Convert to SignaturePosition list
                return results.Select(r => new SignaturePosition
                {
                    PageNumber = r.PageNumber,
                    X = r.X,
                    Y = r.Y
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting signature positions: {ex.Message}", ex);
            }
        }

        private async Task<string> GenerateSignatureURL(List<SignaturePosition> positions)
        {
            try
            {
                // Prepare the signature data
                var signatureData = new
                {
                    documentPath = _pdfFilePath,
                    signaturePositions = positions.Select(p => new
                    {
                        pageNumber = p.PageNumber,
                        x = p.X,
                        y = p.Y
                    }).ToArray(),
                    signerEmail = "signer@example.com", // You can make this configurable
                    signerName = "Document Signer"
                };

                string jsonData = JsonSerializer.Serialize(signatureData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // For demonstration, show the data and return a mock URL
                // In real implementation, you'd call your signature service API

                var result = MessageBox.Show(
                    $"Signature data:\n\n{jsonData}\n\nProceed with opening signature URL?",
                    "Signature Service",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    // Mock signature URL - replace with actual signature service URL
                    await Task.Delay(500); // Simulate processing delay
                    return "https://example.com/signature-service?document=" + Uri.EscapeDataString(Path.GetFileName(_pdfFilePath));
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception($"Signature URL generation error: {ex.Message}", ex);
            }
        }

        private void SetUIState(bool enabled)
        {
            buttonSign.Enabled = enabled;
            groupBoxAutoConfig.Enabled = enabled && radioButtonAutoConfig.Checked;
            groupBoxManualConfig.Enabled = enabled && radioButtonManualConfig.Checked;
        }

        private void btnGeneratePayLoad_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the signature placement information based on current form configuration
                var payloadArray = GetSignaturePlacementInfo();
                
                if (payloadArray == null || payloadArray.Length == 0)
                {
                    MessageBox.Show("No payload data found. Please check your configuration.", 
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Convert to JSON format for display
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                string payloadJson = JsonSerializer.Serialize(payloadArray, jsonOptions);
                
                // Update the payload text box
                if (textBoxPayload != null)
                {
                    textBoxPayload.Text = payloadJson;
                }
                
                // Show success message with payload summary
                int pageCount = payloadArray.Length;
                string message = $"Payload generated successfully!\n\nFound {pageCount} signature position(s):\n\n";
                
                foreach (var item in payloadArray)
                {
                    var itemType = item.GetType();
                    var pageNumberProp = itemType.GetProperty("pageNumber");
                    var xProp = itemType.GetProperty("x");
                    var yProp = itemType.GetProperty("y");
                    
                    if (pageNumberProp != null && xProp != null && yProp != null)
                    {
                        var pageNumber = pageNumberProp.GetValue(item);
                        var x = xProp.GetValue(item);
                        var y = yProp.GetValue(item);
                        message += $"Page {pageNumber}: X={x}, Y={y}\n";
                    }
                }
                
                MessageBox.Show(message, "Payload Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Copy to clipboard for easy access
                Clipboard.SetText(payloadJson);
                toolStripStatusLabelSign.Text = $"Payload generated with {pageCount} position(s) and copied to clipboard.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating payload: {ex.Message}", 
                    "Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabelSign.Text = "Payload generation failed.";
            }
        }
    }
}
