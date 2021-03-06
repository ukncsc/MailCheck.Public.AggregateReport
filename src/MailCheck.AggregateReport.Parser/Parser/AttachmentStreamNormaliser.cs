﻿using System;
using System.IO;
using System.Linq;
using MailCheck.AggregateReport.Parser.Compression;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Utils;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MailCheck.AggregateReport.Parser.Parser
{
    public interface IAttachmentStreamNormaliser
    {
        AttachmentInfo Normalise(MimePart mimePart);
    }

    public class AttachmentStreamNormaliser : IAttachmentStreamNormaliser
    {
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IGZipDecompressor _gZipDecompressor;
        private readonly IZipDecompressor _zipDecompressor;
        private readonly ILogger<AttachmentStreamNormaliser> _log;

        public AttachmentStreamNormaliser(
            IContentTypeProvider contentTypeProvider,
            IGZipDecompressor gZipDecompressor, 
            IZipDecompressor zipDecompressor,
            ILogger<AttachmentStreamNormaliser> log)
        {
            _contentTypeProvider = contentTypeProvider;
            _gZipDecompressor = gZipDecompressor;
            _zipDecompressor = zipDecompressor;
            _log = log;
        }

        public AttachmentInfo Normalise(MimePart mimePart)
        {
            var normalisedStream = NormaliseStream(mimePart);

            return normalisedStream == Stream.Null
                ? AttachmentInfo.EmptyAttachmentInfo
                : new AttachmentInfo(new AttachmentMetadata(mimePart.FileName), normalisedStream);
        }

        private Stream NormaliseStream(MimePart mimePart)
        {
            string contentType = _contentTypeProvider.GetContentType(mimePart);

            switch (contentType)
            {
                case ContentType.ApplicationGzip:
                    return Decompress(mimePart, _gZipDecompressor, _zipDecompressor);
                case ContentType.ApplicationXZipCompressed:
                case ContentType.ApplicationZip:
                    return Decompress(mimePart, _zipDecompressor, _gZipDecompressor);
                case ContentType.ApplicationOctetStream:
                    string extension = Path.GetExtension(mimePart.FileName.Split('!').LastOrDefault());
                    if (extension?.StartsWith(".z") ?? false)
                    {
                        return Decompress(mimePart, _zipDecompressor, _gZipDecompressor);
                    }
                    return Decompress(mimePart, _gZipDecompressor, _zipDecompressor);
                case ContentType.TextXml:
                    return mimePart.GetDecodedStream();
                default:
                    return Stream.Null;
            }
        }

        private Stream Decompress(MimePart mimePart, params IDecompressor[] decompressors)
        {
            foreach (var decompressor in decompressors)
            {
                try
                {
                    using (Stream decodedStream = mimePart.GetDecodedStream())
                    {
                        Stream decompressedStream = decompressor.Decompress(decodedStream);

                        _log.LogInformation(
                            $"Successfully decompressed {mimePart.FileName ?? "<null>"} as {decompressor.StreamType}.");

                        return decompressedStream;
                    }
                }
                catch (Exception)
                {
                    _log.LogInformation($"Failed to decompress {mimePart.FileName ?? "<null>"} as {decompressor.StreamType}.");
                }
            }
            return Stream.Null;
        }
    }
}