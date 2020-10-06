using System;

namespace MailCheck.AggregateReport.Api.V2.Domain
{
    public class DataExportRequest
    {
        public string Domain { get; set; }
        public string Provider { get; set; }
        public string Ip { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeSubdomains { get; set; }
        public bool CsvDownload { get; set; }
        public bool JsonDownload { get; set; }
        public bool JsonRender { get; set; }
    }
}