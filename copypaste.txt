 namespace QET.RECONCILIATION.API.Models
{
    public class ReconReport
    {
        public string id { get; set; }
        public string? project { get; set; }
        public string? report_code { get; set; }
        public DateTime? creation_date { get; set; }
        public DateTime? last_modified_date { get; set; }
        public string? date_created { get; set; }
        public List<RecReportFilter>? filters { get; set; }
        public List<string>? reports { get; set; }
        public DateTime? expires_at { get; set; } = DateTime.Now.AddYears(3);
        public List<Record>? records { get; set; }
        public int? records_count { get; set; }
        public int? match { get; set; }
        public List<ColumnMatch>? columnmatches { get; set; }
        public string? blob_url { get; set; }
    }

    public class RecReportFilter
    {
        public string? filter_field { get; set; }
        public string? filter_value { get; set; }
    }

    public class ColumnMatch
    {
        public string? field_code { get; set; }
        public int? match { get; set; }
    }

    public class Record
    {
        public RowSets? rowsets { get; set; }
    }

    public class RowSets
    {
        public List<ColumnSet>? columnsets { get; set; }
        public int? match { get; set; }
    }

    public class ColumnSet
    {
        public string? field_code { get; set; }
        public DataSet? dataset { get; set; }
    }

    public class DataSet
    {
        public List<DataItem>? data { get; set; }
        public int? match { get; set; }
    }

    public class DataItem
    {
        public string? field_code { get; set; }
        public string? field_value { get; set; }
        public List<SubRecord>? records { get; set; }
    }

    public class SubRecord : Record
    {
        public string id { get; set; }

        public SubRecord()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
