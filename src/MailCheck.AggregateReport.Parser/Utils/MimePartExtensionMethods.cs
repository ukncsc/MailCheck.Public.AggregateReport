using System.IO;
using MimeKit;

namespace MailCheck.AggregateReport.Parser.Utils
{
    public static class MimePartExtensionMethods
    {
        public static Stream GetDecodedStream(this MimePart mimePart)
        {
            MemoryStream memoryStream = new MemoryStream();
            mimePart.Content.DecodeTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}