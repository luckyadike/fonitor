namespace Fonitor.Data
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.Collections.Generic;
    using System.IO;

    public static class BlobExtensions
    {
        public static MemoryStream ExtractStream(this CloudBlockBlob blob)
        {
            var inputStream = new MemoryStream();

            blob.DownloadToStream(inputStream);
            return inputStream;
        }

        public static void AddMetadata(this CloudBlockBlob blob, IDictionary<string, string> metadata)
        {
            if (metadata != null)
            {
                foreach (var kv in metadata)
                {
                    blob.Metadata.Add(kv.Key, kv.Value);
                }
            }
        }
    }
}
