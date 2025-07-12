using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DocIntelAnalyzer
{
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
                                    // Extract full polygon coordinates
                                    var polygonCoords = polygon.Select(p => p.Value<double>()).ToArray();
                                    
                                    // Get first X,Y coordinate from polygon
                                    results.Add(new KeywordLocation
                                    {
                                        PageNumber = pageNumber,
                                        X = polygon[0].Value<double>(),
                                        Y = polygon[1].Value<double>(),
                                        MatchedText = content,
                                        Polygon = polygonCoords
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
                                    // Extract full polygon coordinates
                                    var polygonCoords = polygon.Select(p => p.Value<double>()).ToArray();
                                    
                                    // Get first X,Y coordinate from polygon
                                    results.Add(new KeywordLocation
                                    {
                                        PageNumber = pageNumber,
                                        X = polygon[0].Value<double>(),
                                        Y = polygon[1].Value<double>(),
                                        MatchedText = content,
                                        Polygon = polygonCoords
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

        public static List<KeywordLocation> FindKeywordCoordinates(string jsonData, string keyword, string findIn = "both", string matchType = "exactMatch", int nthOccurrence = 1)
        {
            var allResults = new List<KeywordLocation>();
            
            try
            {
                var json = JObject.Parse(jsonData);
                var pages = json["pages"] as JArray;
                
                if (pages == null) return allResults;
                
                foreach (var page in pages)
                {
                    var pageNumber = page["pageNumber"]?.Value<int>() ?? 1;
                    
                    // Search in words if specified
                    if (findIn == "words" || findIn == "both")
                    {
                        var words = page["words"] as JArray;
                        if (words != null)
                        {
                            foreach (var word in words)
                            {
                                var content = word["content"]?.Value<string>();
                                
                                if (!string.IsNullOrEmpty(content) && IsMatch(content, keyword, matchType))
                                {
                                    var polygon = word["polygon"] as JArray;
                                    if (polygon != null && polygon.Count >= 2)
                                    {
                                        // Extract full polygon coordinates
                                        var polygonCoords = polygon.Select(p => p.Value<double>()).ToArray();
                                        
                                        allResults.Add(new KeywordLocation
                                        {
                                            PageNumber = pageNumber,
                                            X = polygon[0].Value<double>(),
                                            Y = polygon[1].Value<double>(),
                                            MatchedText = content,
                                            Polygon = polygonCoords
                                        });
                                    }
                                }
                            }
                        }
                    }
                    
                    // Search in lines if specified
                    if (findIn == "lines" || findIn == "both")
                    {
                        var lines = page["lines"] as JArray;
                        if (lines != null)
                        {
                            foreach (var line in lines)
                            {
                                var content = line["content"]?.Value<string>();
                                
                                if (!string.IsNullOrEmpty(content) && IsMatch(content, keyword, matchType))
                                {
                                    var polygon = line["polygon"] as JArray;
                                    if (polygon != null && polygon.Count >= 2)
                                    {
                                        // Extract full polygon coordinates
                                        var polygonCoords = polygon.Select(p => p.Value<double>()).ToArray();
                                        
                                        allResults.Add(new KeywordLocation
                                        {
                                            PageNumber = pageNumber,
                                            X = polygon[0].Value<double>(),
                                            Y = polygon[1].Value<double>(),
                                            MatchedText = content,
                                            Polygon = polygonCoords
                                        });
                                    }
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
            
            // Apply Nth occurrence logic
            if (nthOccurrence > 1)
            {
                var groupedByPage = allResults.GroupBy(r => r.PageNumber);
                var filteredResults = new List<KeywordLocation>();
                
                foreach (var pageGroup in groupedByPage)
                {
                    var pageResults = pageGroup.ToList();
                    if (pageResults.Count >= nthOccurrence)
                    {
                        filteredResults.Add(pageResults[nthOccurrence - 1]);
                    }
                }
                
                return filteredResults;
            }
            
            return allResults;
        }

        private static bool IsMatch(string content, string keyword, string matchType)
        {
            if (matchType == "exactMatch")
            {
                return string.Equals(content.Trim(), keyword.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            else // "contains"
            {
                return content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }
    }
}
