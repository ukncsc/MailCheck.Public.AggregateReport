using System.IO;

namespace MailCheck.AggregateReport.Parser.Compression
{
    public interface IDecompressor
    {
        string StreamType { get; }
        Stream Decompress(Stream stream);
    }
}