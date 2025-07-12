using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DocIntelAnalyzer.Services;

namespace DocIntelAnalyzer
{
    /// <summary>
    /// Parameters for signature placement configuration
    /// </summary>
    public class SignaturePlacementParameters
    {
        // Configuration mode
        public SignaturePlacementMode Mode { get; set; } = SignaturePlacementMode.Auto;

        // Auto configuration parameters
        public string SearchKeyword { get; set; } = string.Empty;
        public SearchContentType ContentType { get; set; } = SearchContentType.Words;
        public SearchMatchType MatchType { get; set; } = SearchMatchType.ExactMatch;
        public int NthOccurrence { get; set; } = 1;
        public SignaturePlacement Placement { get; set; } = SignaturePlacement.TopLeft;
        public double OffsetX { get; set; } = 0;
        public double OffsetY { get; set; } = 0;

        // Manual configuration parameters
        public string ManualCoordinates { get; set; } = string.Empty; // Format: "1=100.5,200.3|2=150.7,250.9"
    }

    /// <summary>
    /// Signature placement modes
    /// </summary>
    public enum SignaturePlacementMode
    {
        Auto,
        Manual
    }

    /// <summary>
    /// Search content types
    /// </summary>
    public enum SearchContentType
    {
        Words,
        Lines,
        Both
    }

    /// <summary>
    /// Search match types
    /// </summary>
    public enum SearchMatchType
    {
        ExactMatch,
        Contains
    }

    /// <summary>
    /// Signature placement positions relative to keyword
    /// </summary>
    public enum SignaturePlacement
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    /// <summary>
    /// Result of signature placement calculation
    /// </summary>
    public class SignaturePlacementResult
    {
        public int PageNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    /// <summary>
    /// Service for calculating signature placements based on document analysis and configuration parameters
    /// </summary>
    public class SignaturePlacementService
    {
        /// <summary>
        /// Master function to get signature placement information based on input parameters and document analysis
        /// </summary>
        /// <param name="parameters">Configuration parameters for signature placement</param>
        /// <param name="analysisResult">Document analysis result from Document Intelligence</param>
        /// <returns>Array of signature placement positions with page number, x, and y coordinates</returns>
        public static SignaturePlacementResult[] GetSignaturePlacementInfo(
            SignaturePlacementParameters parameters, 
            DocumentAnalysisResult analysisResult)
        {
            try
            {
                if (parameters == null)
                    throw new ArgumentNullException(nameof(parameters));

                if (analysisResult == null && parameters.Mode == SignaturePlacementMode.Auto)
                    throw new ArgumentNullException(nameof(analysisResult));

                return parameters.Mode == SignaturePlacementMode.Auto
                    ? GetAutoSignaturePlacement(parameters, analysisResult)
                    : GetManualSignaturePlacement(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating signature placement: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Generate signature placement using auto configuration (keyword search)
        /// </summary>
        private static SignaturePlacementResult[] GetAutoSignaturePlacement(
            SignaturePlacementParameters parameters, 
            DocumentAnalysisResult analysisResult)
        {
            if (string.IsNullOrEmpty(parameters.SearchKeyword))
            {
                return new SignaturePlacementResult[0];
            }

            try
            {
                // Convert analysis result to JSON format for KeywordSearcher
                string jsonData = ConvertAnalysisResultToJson(analysisResult);

                // Configure search parameters
                string findIn = GetFindInParameter(parameters.ContentType);
                string matchType = GetMatchTypeParameter(parameters.MatchType);

                // Find keyword positions using KeywordSearcher
                var keywordLocations = KeywordSearcher.FindKeywordCoordinates(
                    jsonData,
                    parameters.SearchKeyword,
                    findIn,
                    matchType,
                    parameters.NthOccurrence);

                if (!keywordLocations.Any())
                {
                    return new SignaturePlacementResult[0];
                }

                // Group by page and select only one signature per page
                var signaturePositions = new List<SignaturePlacementResult>();
                var groupedByPage = keywordLocations.GroupBy(kl => kl.PageNumber);
                
                foreach (var pageGroup in groupedByPage)
                {
                    // Take the first occurrence from each page (Nth occurrence logic already applied in KeywordSearcher)
                    var keywordLocation = pageGroup.First();
                    var signaturePos = CalculateSignaturePosition(keywordLocation, parameters);
                    signaturePositions.Add(new SignaturePlacementResult
                    {
                        PageNumber = signaturePos.PageNumber,
                        X = Math.Round(signaturePos.X * 72, 1), // Convert to points (inch to points conversion)
                        Y = Math.Round(signaturePos.Y * 72, 1)  // Convert to points (inch to points conversion)
                    });
                }

                return signaturePositions.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Auto configuration error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Generate signature placement using manual configuration (direct coordinates)
        /// </summary>
        private static SignaturePlacementResult[] GetManualSignaturePlacement(SignaturePlacementParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.ManualCoordinates))
            {
                return new SignaturePlacementResult[0];
            }

            try
            {
                var positions = new List<SignaturePlacementResult>();
                var coordPairs = parameters.ManualCoordinates.Split('|', StringSplitOptions.RemoveEmptyEntries);

                foreach (var coordPair in coordPairs)
                {
                    var parts = coordPair.Split('=', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        if (int.TryParse(parts[0].Trim(), out int pageNumber))
                        {
                            var xyParts = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (xyParts.Length == 2 &&
                                double.TryParse(xyParts[0].Trim(), out double x) &&
                                double.TryParse(xyParts[1].Trim(), out double y))
                            {
                                positions.Add(new SignaturePlacementResult
                                {
                                    PageNumber = pageNumber,
                                    X = Math.Round(x * 72, 1), // Convert to points (inch to points conversion)
                                    Y = Math.Round(y * 72, 1)  // Convert to points (inch to points conversion)
                                });
                            }
                        }
                    }
                }

                return positions.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Manual configuration error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculate signature position based on keyword location and placement parameters
        /// </summary>
        private static SignaturePosition CalculateSignaturePosition(
            KeywordLocation keywordLocation, 
            SignaturePlacementParameters parameters)
        {
            // Get polygon bounds from the keyword location
            var polygonBounds = GetPolygonBounds(keywordLocation.Polygon);
            
            double x, y;

            // Apply placement logic relative to keyword polygon bounds
            switch (parameters.Placement)
            {
                case SignaturePlacement.TopLeft:
                    x = polygonBounds.Left;
                    y = polygonBounds.Top;
                    break;
                case SignaturePlacement.TopRight:
                    x = polygonBounds.Right;
                    y = polygonBounds.Top;
                    break;
                case SignaturePlacement.BottomLeft:
                    x = polygonBounds.Left;
                    y = polygonBounds.Bottom;
                    break;
                case SignaturePlacement.BottomRight:
                    x = polygonBounds.Right;
                    y = polygonBounds.Bottom;
                    break;
                default:
                    x = polygonBounds.Left;
                    y = polygonBounds.Top;
                    break;
            }

            // Apply manual offsets to the polygon coordinates
            x += parameters.OffsetX;
            y += parameters.OffsetY;

            return new SignaturePosition
            {
                PageNumber = keywordLocation.PageNumber,
                X = x,
                Y = y
            };
        }

        /// <summary>
        /// Calculate bounding box from polygon coordinates
        /// </summary>
        private static PolygonBounds GetPolygonBounds(double[] polygon)
        {
            if (polygon == null || polygon.Length < 8) // Minimum 4 points (x,y pairs)
            {
                // Return default bounds if polygon is invalid
                return new PolygonBounds { Left = 0, Top = 0, Right = 100, Bottom = 20 };
            }

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            // Process polygon coordinates in pairs (x, y)
            for (int i = 0; i < polygon.Length - 1; i += 2)
            {
                double x = polygon[i];
                double y = polygon[i + 1];

                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
            }

            return new PolygonBounds
            {
                Left = minX,
                Top = minY,
                Right = maxX,
                Bottom = maxY
            };
        }

        /// <summary>
        /// Convert analysis result to JSON format for KeywordSearcher
        /// </summary>
        private static string ConvertAnalysisResultToJson(DocumentAnalysisResult result)
        {
            var pages = result.Pages.Select(page => new
            {
                pageNumber = page.PageNumber,
                words = page.Words.Select(word => new
                {
                    content = word.Content,
                    polygon = ParsePolygonString(word.Polygon)
                }).ToArray(),
                lines = page.Lines.Select(line => new
                {
                    content = line.Content,
                    polygon = ParsePolygonString(line.Polygon)
                }).ToArray()
            }).ToArray();

            var jsonObject = new { pages };
            return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        /// <summary>
        /// Parse polygon string to double array
        /// </summary>
        private static double[] ParsePolygonString(string polygonString)
        {
            try
            {
                return polygonString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => double.Parse(s.Trim()))
                                  .ToArray();
            }
            catch
            {
                return new double[] { 0, 0, 100, 0, 100, 20, 0, 20 }; // Default rectangle
            }
        }

        /// <summary>
        /// Convert SearchContentType enum to string parameter for KeywordSearcher
        /// </summary>
        private static string GetFindInParameter(SearchContentType contentType)
        {
            return contentType switch
            {
                SearchContentType.Words => "words",
                SearchContentType.Lines => "lines",
                SearchContentType.Both => "both",
                _ => "words"
            };
        }

        /// <summary>
        /// Convert SearchMatchType enum to string parameter for KeywordSearcher
        /// </summary>
        private static string GetMatchTypeParameter(SearchMatchType matchType)
        {
            return matchType switch
            {
                SearchMatchType.ExactMatch => "exactMatch",
                SearchMatchType.Contains => "contains",
                _ => "exactMatch"
            };
        }
    }
}
