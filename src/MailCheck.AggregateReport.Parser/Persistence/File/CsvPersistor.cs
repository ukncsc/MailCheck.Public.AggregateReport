using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Serialisation;

namespace MailCheck.AggregateReport.Parser.Persistence.File
{
    internal class CsvPersistor : IDenormalisedRecordPersistor
    {
        private readonly ICommandLineArgs _args;
        private readonly ICsvDenormalisedRecordSerialiser _csvDenormalisedRecordSerialiser;
        private bool _inited;

        public CsvPersistor(
            ICommandLineArgs args, 
            ICsvDenormalisedRecordSerialiser csvDenormalisedRecordSerialiser)
        {
            _args = args;
            _csvDenormalisedRecordSerialiser = csvDenormalisedRecordSerialiser;
        }

        public Task Persist(List<DenormalisedRecord> records)
        {
            CreateDirectoryAndRemoveOldFiles(_args.CsvFile);
            using (FileStream fileStream = new FileStream(_args.CsvFile.FullName, FileMode.Append))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    foreach (var record in records)
                    {
                        streamWriter.WriteLine(_csvDenormalisedRecordSerialiser.Serialise(record));
                    }
                }
            }

            return Task.CompletedTask;
        }

        private void CreateDirectoryAndRemoveOldFiles(FileInfo location)
        {
            if (!_inited)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(location.DirectoryName);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                if (location.Exists)
                {
                    location.Delete();
                }

                _inited = true;
            }
        }
    }
}