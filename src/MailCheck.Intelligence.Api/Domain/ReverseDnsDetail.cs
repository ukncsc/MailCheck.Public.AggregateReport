using System;

namespace MailCheck.Intelligence.Api.Domain
{
    public class ReverseDnsDetail
    {
        public ReverseDnsDetail(DateTime date, string host, string organisationalDomain, bool forwardMatches)
        {
            StartDate = date;
            EndDate = date;
            Host = host;
            OrganisationalDomain = organisationalDomain;
            ForwardMatches = forwardMatches;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Host { get; set; }
        public string OrganisationalDomain { get; set; }
        public bool ForwardMatches { get; set; }
    }
}