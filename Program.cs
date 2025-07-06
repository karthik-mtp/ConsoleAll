using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


public class Program
{
    public static async Task Main()
    {
        string pythonfile = @"C:\Users\Gomathi\source\repos\ConsoleAll\pdf_signature_processor.py";
        var processor = new PDFSignatureProcessor(@"C:\Python313\python.exe", pythonfile, false); // Disable logging

        var configs = new List<SignatureConfig>
            {
                new SignatureConfig
                {
                    WorkingFolder = "C:\\Users\\Gomathi\\Downloads",
                    InputPdfFilename = "type1.pdf",
                    ItemId = "test_signature",
                    SignatureFilename = "signature.png",
                    Keywords = new List<string> { "AUTHORISED SIGNATURE" },
                    SignatureSize = new float[] { 120, 40 },
                    SkipNonEmpty = false  // Skip "By: someone" but allow "By:" (blank)
                }
                // ,
                // new SignatureConfig
                // {
                //     WorkingFolder = "C:\\Users\\Gomathi\\Downloads",
                //     InputPdfFilename = "ip1.pdf",
                //     ItemId = "test_signature_1",
                //     SignatureFilename = "signature.png",
                //     Keywords = new List<string> { "By:" },
                //     SignatureSize = new float[] { 120, 40 },
                //     SkipNonEmpty = true  // Skip "By: someone" but allow "By:" (blank)
                // },
                // new SignatureConfig
                // {
                //     WorkingFolder = "C:\\Users\\Gomathi\\Downloads",
                //     InputPdfFilename = "ip2.pdf",
                //     ItemId = "test_signature_2",
                //     SignatureFilename = "signature.png",
                //     Keywords = new List<string> { "By:" },
                //     SignatureSize = new float[] { 120, 40 },
                //     SkipNonEmpty = true  // Skip "By: someone" but allow "By:" (blank)
                // },
                // new SignatureConfig
                // {
                //     WorkingFolder = "C:\\Users\\Gomathi\\Downloads",
                //     InputPdfFilename = "ip3.pdf",
                //     ItemId = "test_signature_3",
                //     SignatureFilename = "signature.png",
                //     Keywords = new List<string> { "By:" },
                //     SignatureSize = new float[] { 120, 40 },
                //     SkipNonEmpty = true  // Skip "By: someone" but allow "By:" (blank)
                // },
                // new SignatureConfig
                // {
                //     WorkingFolder = "C:\\Users\\Gomathi\\Downloads",
                //     InputPdfFilename = "ip4.pdf",
                //     ItemId = "test_signature_4",
                //     SignatureFilename = "signature.png",
                //     Keywords = new List<string> { "By:" },
                //     SignatureSize = new float[] { 120, 40 },
                //     SkipNonEmpty = true  // Skip "By: someone" but allow "By:" (blank)
                // },
                // new SignatureConfig
                // {
                //     WorkingFolder = "C:\\Users\\Gomathi\\Downloads",
                //     InputPdfFilename = "ip5.pdf",
                //     ItemId = "test_signature_5",
                //     SignatureFilename = "signature.png",
                //     Keywords = new List<string> { "By:" },
                //     SignatureSize = new float[] { 120, 40 },
                //     SkipNonEmpty = true  // Skip "By: someone" but allow "By:" (blank)
                // }
            };

        try
        {
            var results = await processor.ProcessDocumentsAsync(configs);
            foreach (var result in results)
            {
                Console.WriteLine($"Processed {result.InputPdfPath}: " +
                    $"{(result.Success ? "Success" : $"Failed - {result.ErrorMessage}")} | Output: {result.OutputPdfPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.ReadLine();
    }
}



public class SignatureConfig
    {
        public string WorkingFolder { get; set; } = "";
        public string InputPdfFilename { get; set; } = "";
        public string ItemId { get; set; } = "";
        public string SignatureFilename { get; set; } = "";
        public List<string>? Keywords { get; set; }
        public float[]? SignatureSize { get; set; }
        public string? OutputPath { get; set; }
        public float? XCoord { get; set; }
        public float? YCoord { get; set; }
        public List<int>? PageNumbers { get; set; }
        public bool SkipNonEmpty { get; set; } = false;  // Skip matches that have content after the keyword
    }

    public class SignatureResult
    {
        public string InputPdfPath { get; set; } = "";
        public string ItemId { get; set; } = "";
        public string OutputPdfPath { get; set; } = "";
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PDFSignatureProcessor
    {
        private readonly string _pythonPath;
        private readonly string _scriptPath;
        private readonly bool _enableLogging;

        public PDFSignatureProcessor(string pythonPath = "python", string scriptPath = "pdf_signature_processor.py", bool enableLogging = true)
        {
            _pythonPath = pythonPath;
            _scriptPath = scriptPath;
            _enableLogging = enableLogging;
        }

        public async Task<List<SignatureResult>> ProcessDocumentsAsync(List<SignatureConfig> configs)
        {
            try
            {
                // Convert the configs to JSON
                var jsonConfigs = JsonSerializer.Serialize(configs, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                // Create a temporary file to pass the JSON data
                var tempFile = System.IO.Path.GetTempFileName();
                await File.WriteAllTextAsync(tempFile, jsonConfigs);

                if (_enableLogging)
                {
                    Console.WriteLine($"Executing: {_pythonPath} \"{_scriptPath}\" \"{tempFile}\"");
                }

                // Create process start info
                var startInfo = new ProcessStartInfo
                {
                    FileName = _pythonPath,
                    Arguments = $"\"{_scriptPath}\" \"{tempFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the process
                using var process = new Process { StartInfo = startInfo };
                var outputBuilder = new System.Text.StringBuilder();
                var errorBuilder = new System.Text.StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await process.WaitForExitAsync();

                var output = outputBuilder.ToString().Trim();
                var error = errorBuilder.ToString().Trim();

                if (_enableLogging)
                {
                    Console.WriteLine($"Python Exit Code: {process.ExitCode}");
                    Console.WriteLine($"Python Output: {output}");
                    Console.WriteLine($"Python Error: {error}");
                }

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Python script execution failed with exit code {process.ExitCode}. Error: {error}. Output: {output}");
                }

                // Parse the results
                var jsonResults = output;
                if (string.IsNullOrEmpty(jsonResults))
                {
                    throw new Exception("Python script returned empty output");
                }

                var results = JsonSerializer.Deserialize<List<SignatureResult>>(jsonResults,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                // Clean up temporary file
                try
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine($"Warning: Could not delete temporary file {tempFile}: {cleanupEx.Message}");
                }

                return results ?? new List<SignatureResult>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing PDF documents: {ex.Message}", ex);
            }
        }
    }

    // Example usage
 
