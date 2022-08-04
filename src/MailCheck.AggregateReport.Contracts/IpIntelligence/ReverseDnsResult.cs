using System;
using System.Collections.Generic;

namespace MailCheck.AggregateReport.Contracts.IpIntelligence
{
    public class ReverseDnsResult
    {
        public ReverseDnsResult(string originalIpAddress, List<ReverseDnsResponse> forwardResponses)
        {
            OriginalIpAddress = originalIpAddress;
            ForwardResponses = forwardResponses ?? new List<ReverseDnsResponse>();
        }

        private ReverseDnsResult(string originalIpAddress)
        {
            OriginalIpAddress = originalIpAddress;
        }

        public string OriginalIpAddress { get; }

        public List<ReverseDnsResponse> ForwardResponses { get; }

        public bool IsInconclusive { get; private set; }

        protected bool Equals(ReverseDnsResult other)
        {
            return string.Equals(OriginalIpAddress, other.OriginalIpAddress) 
                   && Equals(ForwardResponses, other.ForwardResponses);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ReverseDnsResult) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((OriginalIpAddress != null ? OriginalIpAddress.GetHashCode() : 0) * 397) 
                       ^ (ForwardResponses != null ? ForwardResponses.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{nameof(OriginalIpAddress)}: {OriginalIpAddress}, {nameof(ForwardResponses)}: {string.Join(Environment.NewLine, ForwardResponses)}";
        }

        public static ReverseDnsResult Inconclusive(string originalIpAddress)
        {
            return new ReverseDnsResult(originalIpAddress)
            {
                IsInconclusive = true
            };
        }
    }
}
