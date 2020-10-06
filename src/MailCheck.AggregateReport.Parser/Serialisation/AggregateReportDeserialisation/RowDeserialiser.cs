using System;
using System.Xml.Linq;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.AggregateReport.Parser.Utils;

namespace MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation
{
    public interface IRowDeserialiser
    {
        Row Deserialise(XElement row);
    }

    public class RowDeserialiser : IRowDeserialiser
    {
        private readonly IPolicyEvaluatedDeserialiser _policyEvaluatedDeserialiser;

        public RowDeserialiser(IPolicyEvaluatedDeserialiser policyEvaluatedDeserialiser)
        {
            _policyEvaluatedDeserialiser = policyEvaluatedDeserialiser;
        }

        public Row Deserialise(XElement row)
        {
            if (row.Name != "row")
            {
                throw new ArgumentException("Root element must be row");
            }

            string sourceIp = row.Single("source_ip").Value;

            int count = int.Parse(row.Single("count").Value);

            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(row.Single("policy_evaluated"));

            return new Row(sourceIp, count, policyEvaluated);
        }
    }
}