using System;
using System.Collections.Generic;

namespace MailCheck.Intelligence.Api.Domain
{
    public class AsInfo
    {
        public AsInfo(DateTime date, int asNumber, string description, string countryCode)
        {
            StartDate = date;
            EndDate = date;
            AsNumber = asNumber;
            Description = description;
            CountryCode = countryCode;
        }

        public DateTime StartDate { get; }
        public DateTime EndDate { get; set; }
        public int AsNumber { get; }
        public string Description { get; }
        public string CountryCode { get; }
    }
}