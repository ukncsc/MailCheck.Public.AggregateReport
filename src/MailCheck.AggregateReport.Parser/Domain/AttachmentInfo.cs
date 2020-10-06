using System.IO;

namespace MailCheck.AggregateReport.Parser.Domain
{
    public class AttachmentInfo
    {
        public static AttachmentInfo EmptyAttachmentInfo = new AttachmentInfo(AttachmentMetadata.EmptyAttachmentMetadata, Stream.Null);

        public AttachmentInfo(AttachmentMetadata attachmentMetadata, Stream attachmentStream)
        {
            AttachmentMetadata = attachmentMetadata;
            AttachmentStream = attachmentStream;
        }

        public AttachmentMetadata AttachmentMetadata { get; }

        public Stream AttachmentStream { get; }

        protected bool Equals(AttachmentInfo other)
        {
            return Equals(AttachmentMetadata, other.AttachmentMetadata);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AttachmentInfo)obj);
        }

        public override int GetHashCode()
        {
            return (AttachmentMetadata != null ? AttachmentMetadata.GetHashCode() : 0);
        }
    }
}