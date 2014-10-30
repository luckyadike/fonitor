namespace Fonitor.Data.Repositories
{
	using Fonitor.Data.Services;
	using Microsoft.WindowsAzure.Storage.Table;
	using System.Collections.Generic;

	public class TableRepository<T> where T : TableEntity , new()
	{
		public TableRepository(TableStorageService service, string tableName)
		{
			client = service.StorageAccount.CreateCloudTableClient();

			reference = client.GetTableReference(tableName);
		}

		public void Add(T entity)
		{
			var operation = TableOperation.Insert(entity);

			reference.Execute(operation);
		}

		public void AddMany(List<T> entities)
		{
			var batchOperation = new TableBatchOperation();

			foreach (T entity in entities)
			{
				batchOperation.Insert(entity);
			}

			reference.ExecuteBatch(batchOperation);
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

		public void Remove(string partitionKey, string rowKey)
		{
			var deletionCandidate = Retrieve(partitionKey, rowKey);

			if (deletionCandidate != null)
			{
				var deleteOperation = TableOperation.Delete(deletionCandidate);

				reference.Execute(deleteOperation);
			}

			// Consider making noise if the entry is not found?
		}

		public IEnumerable<T> RetrievePartition(string partitionKey)
		{
			var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

			return reference.ExecuteQuery(query);
		}

		protected readonly CloudTableClient client;

		protected readonly CloudTable reference;
	}
}