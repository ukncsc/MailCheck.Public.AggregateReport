using System.IO;
using MimeKit;

namespace MailCheck.AggregateReport.Parser.Factory
{
    public interface IMimeMessageFactory
    {
        MimeMessage Create(Stream stream);
    }

    internal class MimeMessageFactory : IMimeMessageFactory
    {
        public MimeMessage Create(Stream stream)
        {
            MimeParser parser = new MimeParser(stream, MimeFormat.Entity);
            return parser.ParseMessage();
        }
    }
}
