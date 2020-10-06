using System;

namespace MailCheck.AggregateReport.Contracts.IpIntelligence
{
    public class IpAddressDetailsRequest
    {
        private DateTime _date;

        public IpAddressDetailsRequest(string ipAddress, DateTime date)
        {
            IpAddress = ipAddress;
            Date = date.Date;
        }

        public string IpAddress { get; set; }

        public DateTime Date
        {
            get => _date.Date;
            set => _date = value.Date;
        }
    }
}