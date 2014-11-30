namespace Fonitor.Tests.Repositories
{
    using Fonitor.Data.Repositories;
    using System;
    using System.Collections.Generic;

    public class InMemoryTableRepository<T> : ITableRepository<T>
    {
        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public T Retrieve(string partitionKey, string rowKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> RetrievePartition(string partitionKey)
        {
            throw new NotImplementedException();
        }
    }
}
