using System.Collections.Generic;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Messaging.Common.Exception;
using Newtonsoft.Json;

namespace MailCheck.AggregateReport.Parser.Publisher
{
    public static class BatchExtensions
    {
        public static IEnumerable<IEnumerable<TSource>> BatchByJsonBytes<TSource>(this IEnumerable<TSource> source, int maxSizeBytes) where TSource : Message
        {
            Bucket<TSource> bucket = new Bucket<TSource>(maxSizeBytes);
            foreach (TSource item in source)
            {
                if (bucket.TryAdd(item)) continue;

                yield return bucket.Records;

                bucket = new Bucket<TSource>(maxSizeBytes);
                bucket.TryAdd(item);
            }
            if (bucket.Records.Count > 0)
                yield return bucket.Records;
        }

        private class Bucket<TSource>
        {
            private const int ArrayDelimiterByteCount = 4;   // []
            private const int ElementDelimiterByteCount = 2; // ,
            
            private readonly int _maxSizeBytes;
            private int _currentSize = ArrayDelimiterByteCount;
            public readonly List<TSource> Records = new List<TSource>();

            public Bucket(int maxSizeBytes)
            {
                _maxSizeBytes = maxSizeBytes;
            }

            public bool TryAdd(TSource record)
            {
                string serializedRecord = JsonConvert.SerializeObject(record);
                int recordByteCount = System.Text.Encoding.Unicode.GetByteCount(serializedRecord);

                if (recordByteCount + ArrayDelimiterByteCount > _maxSizeBytes) throw new MailCheckException($"Element {(record as Message)?.Id} too large to batch");

                if (_currentSize + ElementDelimiterByteCount + recordByteCount > _maxSizeBytes) return false;

                _currentSize += ElementDelimiterByteCount + recordByteCount;
                Records.Add(record);
                return true;
            }
        }
    }
}