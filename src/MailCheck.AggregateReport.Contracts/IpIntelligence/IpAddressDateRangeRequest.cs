using System;

namespace MailCheck.AggregateReport.Contracts.IpIntelligence
{
    public class IpAddressDateRangeRequest
    {
        private DateTime _startDate;
        private DateTime _endDate;
        
        public IpAddressDateRangeRequest()
        {}

        public IpAddressDateRangeRequest(string ipAddress, DateTime startDate, DateTime endDate)
        {
            IpAddress = ipAddress;
            StartDate = startDate.Date;
            EndDate = endDate.Date;
        }

        public string IpAddress { get; set; }

        public DateTime StartDate
        {
            get => _startDate.Date;
            set => _startDate = value.Date;
        }

        public DateTime EndDate
        {
            get => _endDate.Date;
            set => _endDate = value.Date;
        }
    }
}