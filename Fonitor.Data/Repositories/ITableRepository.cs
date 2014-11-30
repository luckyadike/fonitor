namespace Fonitor.Data.Repositories
{
    using System.Collections.Generic;

    public interface ITableRepository<T>
    {
        void Add(T entity);

        T Retrieve(string partitionKey, string rowKey);

        IEnumerable<T> RetrievePartition(string partitionKey);
    }
}
