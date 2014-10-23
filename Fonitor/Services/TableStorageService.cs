namespace Fonitor.Services
{
	using Microsoft.WindowsAzure;
	using Microsoft.WindowsAzure.Storage;

	public class TableStorageService
	{
		private static readonly CloudStorageAccount storageAccount =
			CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("TableStorageConnectionString"));

		public CloudStorageAccount StorageAccount
		{
			get
			{
				return storageAccount;
			}
		}

		public static void CreateTableIfNotExists(string tableName)
		{
			var tableClient = storageAccount.CreateCloudTableClient();

			tableClient.GetTableReference(tableName).CreateIfNotExists();
		}
	}
}