namespace DocIntelAnalyzer
{
    public class KeywordLocation
    {
        public int PageNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string MatchedText { get; set; } = string.Empty;
        public double[] Polygon { get; set; } = new double[0]; // Full polygon coordinates
    }

    public class SignaturePosition
    {
        public int PageNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class PolygonBounds
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
    }
}
