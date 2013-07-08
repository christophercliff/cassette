using System;
using System.IO;

namespace Cassette.DependencyGraphInteration.InterationResults
{
    public class StreamMetaDataResult : SimpleInteractionResult
    {
        public bool NotFound { get; set; }
        public string Hash { get; set; }
        public string ContentType { get; set; }
    }

    public class StreamInterationResult : Stream, IInterationResult
    {
        public bool NotFound { get; set; }
        public string Hash { get; set; }
        public string ContentType { get; set; }
        public Exception Exception { get; set; }

        public StreamInterationResult() { }

        Stream stream;
        public StreamInterationResult(Stream stream, StreamMetaDataResult metaData = null)
        {
            this.stream = stream;
            if(metaData != null)
            {
                NotFound = metaData.NotFound;
                Hash = metaData.Hash;
                ContentType = metaData.ContentType;
                Exception = metaData.Exception;
            }
        }

        public override bool CanRead
        {
            get { return stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return stream.CanWrite; }
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override long Length
        {
            get { return stream.Length; }
        }

        public override long Position
        {
            get { return stream.Position; }
            set { stream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                stream.Dispose();
            }
            catch (Exception)
            {
                stream = null;
            }

            base.Dispose(disposing);
        }
    }
}
