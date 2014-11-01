namespace Fonitor.Jobs
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.IO;

    public static class BlobExtensions
    {
        public static MemoryStream ExtractStream(this CloudBlockBlob blob)
        {
            var inputStream = new MemoryStream();

            blob.DownloadToStream(inputStream);
            return inputStream;
        }
    }
}
