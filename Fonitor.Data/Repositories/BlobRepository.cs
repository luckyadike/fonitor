namespace Fonitor.Data.Repositories
{
	using Fonitor.Data.Services;
	using Microsoft.WindowsAzure.Storage.Blob;
	using System.Collections.Generic;
	using System.IO;

    public class BlobRepository
    {
        public BlobRepository(StorageService service)
		{
			client = service.StorageAccount.CreateCloudBlobClient();
		}

        public void Add(Stream entity, string container, string key)
        {
			AddWithMetadata(entity, container, key, null);
        }

		public void AddWithMetadata(Stream entity, string container, string key, IDictionary<string, string> metadata)
		{
			entity.Seek(0, SeekOrigin.Begin);

			GetContainerReference(container);

			var blob = reference.GetBlockBlobReference(key);
			if (!blob.Exists())
			{
                blob.AddMetadata(metadata);

				blob.UploadFromStream(entity);
			}
		}

        public void AddOrReplace(Stream entity, string container, string key)
        {
			entity.Seek(0, SeekOrigin.Begin);

            GetContainerReference(container);

            reference.GetBlockBlobReference(key).UploadFromStream(entity);
        }

		public CloudBlockBlob RetrieveAsBlob(string container, string key)
		{
			GetContainerReference(container);

			return reference.GetBlockBlobReference(key);
		}

        public Stream RetrieveAsStream(string container, string key)
        {
            GetContainerReference(container);

            var blob = reference.GetBlockBlobReference(key);
            if (blob.Exists())
            {
                using (var result = new MemoryStream())
                {
                    blob.DownloadToStream(result);

                    return result;
                }         
            }

            return null;
        }

        private void GetContainerReference(string container, bool shouldCreate = true)
        {
            reference = client.GetContainerReference(container);

            if (shouldCreate)
            {
                reference.CreateIfNotExists();
            }
        }

        private CloudBlobClient client;

        private CloudBlobContainer reference;
    }
}
