namespace FonitorData.Repositories
{
    using FonitorData.Services;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.IO;

    public class BlobRepository
    {
        public BlobRepository(BlobStorageService service)
		{
			client = service.StorageAccount.CreateCloudBlobClient();
		}

        public void Add(Stream entity, string container, string key)
        {
			entity.Seek(0, SeekOrigin.Begin);

            GetReference(container);

            var blob = reference.GetBlockBlobReference(key);
            if (!blob.Exists())
            {
                blob.UploadFromStream(entity);
            }
            else
            {
                // Raise an exception here.
            }
        }

        public void AddOrReplace(Stream entity, string container, string key)
        {
			entity.Seek(0, SeekOrigin.Begin);

            GetReference(container);

            reference.GetBlockBlobReference(key).UploadFromStream(entity);
        }

        public Stream Retrieve(string container, string key)
        {
            GetReference(container);

            var blob = reference.GetBlockBlobReference(key);
            if (blob.Exists())
            {
                var result = new MemoryStream();

                blob.DownloadToStream(result);

                return result;
            }

            return null;
        }

        private void GetReference(string key)
        {
            client.GetContainerReference(key).CreateIfNotExists();

            reference = client.GetContainerReference(key);
        }

        private CloudBlobClient client;

        private CloudBlobContainer reference;
    }
}
