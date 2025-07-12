using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DocIntelAnalyzer
{
    public class KeywordLocation
    {
        public int PageNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string MatchedText { get; set; } = string.Empty;
    }

    public class KeywordSearcher
    {
        public static List<KeywordLocation> FindKeywordCoordinates(string jsonData, string keyword)
        {
            var results = new List<KeywordLocation>();
            
            try
            {
                var json = JObject.Parse(jsonData);
                var pages = json["pages"] as JArray;
                
                if (pages == null) return results;
                
                foreach (var page in pages)
                {
                    var pageNumber = page["pageNumber"]?.Value<int>() ?? 1;
                    
                    // Search in words
                    var words = page["words"] as JArray;
                    if (words != null)
                    {
                        foreach (var word in words)
                        {
                            var content = word["content"]?.Value<string>();
                            
                            if (!string.IsNullOrEmpty(content) && 
                                string.Equals(content.Trim(), keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                var polygon = word["polygon"] as JArray;
                                if (polygon != null && polygon.Count >= 2)
                                {
                                    // Get first X,Y coordinate from polygon
                                    results.Add(new KeywordLocation
                                    {
                                        PageNumber = pageNumber,
                                        X = polygon[0].Value<double>(),
                                        Y = polygon[1].Value<double>(),
                                        MatchedText = content
                                    });
                                }
                            }
                        }
                    }
                    
                    // Search in lines for multi-word keywords
                    var lines = page["lines"] as JArray;
                    if (lines != null)
                    {
                        foreach (var line in lines)
                        {
                            var content = line["content"]?.Value<string>();
                            
                            if (!string.IsNullOrEmpty(content) && 
                                string.Equals(content.Trim(), keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                var polygon = line["polygon"] as JArray;
                                if (polygon != null && polygon.Count >= 2)
                                {
                                    // Get first X,Y coordinate from polygon
                                    results.Add(new KeywordLocation
                                    {
                                        PageNumber = pageNumber,
                                        X = polygon[0].Value<double>(),
                                        Y = polygon[1].Value<double>(),
                                        MatchedText = content
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing Document Intelligence JSON: {ex.Message}", ex);
            }
            
            return results;
        }
    }
}
