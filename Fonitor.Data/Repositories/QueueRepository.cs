namespace Fonitor.Data.Repositories
{
	using Fonitor.Data.Services;
	using Microsoft.WindowsAzure.Storage.Queue;

	public class QueueRepository
	{
		public QueueRepository(StorageService service)
		{
			client = service.StorageAccount.CreateCloudQueueClient();
		}

		public void Enqueue(string name, string item)
		{
			reference = client.GetQueueReference(name);

			reference.CreateIfNotExists();

			var message = new CloudQueueMessage(item);

			reference.AddMessage(message);
		}

		public string DeQueue(string name)
		{
			reference = client.GetQueueReference(name);

			if (reference.Exists())
			{
				var message = reference.GetMessage();

				// Remove it immediately.
				reference.DeleteMessage(message);

				return message.AsString;
			}

			return string.Empty;
		}

		private CloudQueueClient client;

		private CloudQueue reference;
	}
}
