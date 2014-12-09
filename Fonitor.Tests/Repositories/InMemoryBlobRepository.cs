namespace Fonitor.Tests.Repositories
{
    using Fonitor.Data.Repositories;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class InMemoryBlobRepository : IBlobRepository
    {
        IDictionary<string, Stream> Store;

        public InMemoryBlobRepository()
        {
            Store = new Dictionary<string, Stream>();
        }

        public void Add(System.IO.Stream entity, string container, string key)
        {
            Store.Add(container + key, entity);
        }

        public void AddWithMetadata(Stream entity, string container, string key, IDictionary<string, string> metadata)
        {
            Store.Add(container + key, entity);
        }

        public void AddOrReplace(Stream entity, string container, string key)
        {
            throw new NotImplementedException();
        }

        public void Delete(string container, string key)
        {
            throw new NotImplementedException();
        }

        public Stream RetrieveAsStream(string container, string key)
        {
            return Store[container + key];
        }

        public int Count()
        {
            return Store.Count;
        }
    }
}
