namespace Fonitor.Repositories
{
	using Fonitor.Services;
	using Microsoft.WindowsAzure.Storage.Table;
	using System.Collections.Generic;

	public abstract class Repository<T> where T : TableEntity 
	{
		public Repository(TableStorageService service, string tableName)
		{
			tableClient = service.StorageAccount.CreateCloudTableClient();

			tableReference = tableClient.GetTableReference(tableName);
		}

		public void AddOrReplace(T entity)
		{
			var operation = TableOperation.InsertOrReplace(entity);

			tableReference.Execute(operation);
		}

		public void Add(List<T> entities)
		{
			var batchOperation = new TableBatchOperation();

			foreach (var entity in entities)
			{
				batchOperation.Insert(entity);
			}

			tableReference.ExecuteBatch(batchOperation);
		}

		protected readonly CloudTableClient tableClient;

		protected readonly CloudTable tableReference;
	}
}