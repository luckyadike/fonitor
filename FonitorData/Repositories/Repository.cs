namespace FonitorData.Repositories
{
	using FonitorData.Services;
	using Microsoft.WindowsAzure.Storage.Table;
	using System.Collections.Generic;

	public class Repository<T> where T : TableEntity , new()
	{
		public Repository(TableStorageService service, string tableName)
		{
			client = service.StorageAccount.CreateCloudTableClient();

			reference = client.GetTableReference(tableName);
		}

		public void Add(T entity)
		{
			var operation = TableOperation.Insert(entity);

			reference.Execute(operation);
		}

		public void AddOrReplace(T entity)
		{
			var operation = TableOperation.InsertOrReplace(entity);

			reference.Execute(operation);
		}

		public T Retrieve(string partitionKey, string rowKey)
		{
			var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);

			var result = reference.Execute(operation);

			return (T)result.Result;
		}

		public IEnumerable<T> RetrievePartition(string partitionKey)
		{
			var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

			return reference.ExecuteQuery(query);
		}

		public void Add(List<T> entities)
		{
			var batchOperation = new TableBatchOperation();

			foreach (var entity in entities)
			{
				batchOperation.Insert(entity);
			}

			reference.ExecuteBatch(batchOperation);
		}

		protected readonly CloudTableClient client;

		protected readonly CloudTable reference;
	}
}