using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Text.Json;

namespace DocIntelAnalyzer.Services
{
    public class DocumentIntelligenceService
    {
        private readonly DocumentAnalysisClient _client;

        public DocumentIntelligenceService(string endpoint, string apiKey)
        {
            var credential = new AzureKeyCredential(apiKey);
            _client = new DocumentAnalysisClient(new Uri(endpoint), credential);
        }

        public async Task<DocumentAnalysisResult> AnalyzeDocumentAsync(string filePath)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                
                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed, 
                    "prebuilt-document", 
                    stream
                );

                var result = operation.Value;
                return ConvertToDocumentAnalysisResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error analyzing document: {ex.Message}", ex);
            }
        }

        private DocumentAnalysisResult ConvertToDocumentAnalysisResult(AnalyzeResult result)
        {
            var pages = new List<PageInfo>();

            foreach (var page in result.Pages)
            {
                var words = new List<WordInfo>();
                var lines = new List<LineInfo>();

                // Convert words
                if (page.Words != null)
                {
                    foreach (var word in page.Words)
                    {
                        words.Add(new WordInfo
                        {
                            PageNumber = page.PageNumber,
                            Content = word.Content,
                            Polygon = string.Join(", ", word.BoundingPolygon.SelectMany(p => new[] { p.X.ToString("F2"), p.Y.ToString("F2") })),
                            Confidence = word.Confidence.ToString("F3")
                        });
                    }
                }

                // Convert lines
                if (page.Lines != null)
                {
                    foreach (var line in page.Lines)
                    {
                        lines.Add(new LineInfo
                        {
                            PageNumber = page.PageNumber,
                            Content = line.Content,
                            Polygon = string.Join(", ", line.BoundingPolygon.SelectMany(p => new[] { p.X.ToString("F2"), p.Y.ToString("F2") }))
                        });
                    }
                }

                pages.Add(new PageInfo
                {
                    PageNumber = page.PageNumber,
                    Words = words,
                    Lines = lines
                });
            }

            return new DocumentAnalysisResult
            {
                Pages = pages
            };
        }
    }

    public class DocumentAnalysisResult
    {
        public List<PageInfo> Pages { get; set; } = new List<PageInfo>();
    }

    public class PageInfo
    {
        public int PageNumber { get; set; }
        public List<WordInfo> Words { get; set; } = new List<WordInfo>();
        public List<LineInfo> Lines { get; set; } = new List<LineInfo>();
    }

    public class WordInfo
    {
        public int PageNumber { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Polygon { get; set; } = string.Empty;
        public string Confidence { get; set; } = string.Empty;
    }

    public class LineInfo
    {
        public int PageNumber { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Polygon { get; set; } = string.Empty;
    }
}
