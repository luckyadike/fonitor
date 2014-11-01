namespace Fonitor.Data.Services
{
	using Microsoft.WindowsAzure;
	using Microsoft.WindowsAzure.Storage;

	public class TableStorageService : StorageService
	{
		public static void CreateTableIfNotExists(string tableName)
		{
			var tableClient = storageAccount.CreateCloudTableClient();

			tableClient.GetTableReference(tableName).CreateIfNotExists();
		}
	}
}