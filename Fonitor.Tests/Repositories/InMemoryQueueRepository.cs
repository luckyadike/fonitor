namespace Fonitor.Tests.Repositories
{
    using Fonitor.Data.Repositories;
    using System;
    using System.Collections;

    public class InMemoryQueueRepository : IQueueRepository
    {
        Queue Store;

        public InMemoryQueueRepository()
        {
            Store = new Queue();
        }

        public void Enqueue(string name, string item)
        {
            Store.Enqueue(item);
        }

        public string DeQueue(string name)
        {
            return Store.Count == 0 ? string.Empty : Store.Dequeue().ToString();
        }

        public int Count()
        {
            return Store.Count;
        }
    }
}
