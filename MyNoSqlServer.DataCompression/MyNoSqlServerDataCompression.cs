using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Readers.Zip;
using SharpCompress.Writers;

namespace MyNoSqlServer.DataCompression
{
    public static class MyNoSqlServerDataCompression
    {

        private const string ZipEntryName = "d";

        public static byte[] ZipPayload(this ReadOnlyMemory<byte> source)
        {
            using var memoryStream = new MemoryStream(source.Length);
            memoryStream.Write(source.Span);
            memoryStream.Position = 0;

            return ZipPayload(memoryStream);
        }

        public static byte[] ZipPayload(this MemoryStream sourceStream)
        {
            var zipResultStream = new MemoryStream();

            var o = new WriterOptions(CompressionType.Deflate);

            var zipWriter = WriterFactory.Open(zipResultStream, ArchiveType.Zip, o);

            zipWriter.Write(ZipEntryName, sourceStream);
            
            zipWriter.Dispose();

            return zipResultStream.ToArray();
        }


        private static readonly ReaderOptions ReaderOptions = new ReaderOptions();

        public static byte[] UnZipPayload(this ReadOnlyMemory<byte> src)
        {
            
            var srcStream = new MemoryStream(src.Length);
            srcStream.Write(src.Span);
            srcStream.Position = 0;

            var reader = (ZipReader)ReaderFactory.Open(srcStream, ReaderOptions);

            reader.MoveToNextEntry();
            
            var resultStream = new MemoryStream();
            
            reader.WriteEntryTo(resultStream);

            return resultStream.ToArray();
        }
    }
}