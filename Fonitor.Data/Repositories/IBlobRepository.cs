namespace Fonitor.Data.Repositories
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.Collections.Generic;
    using System.IO;

    public interface IBlobRepository
    {
        void Add(Stream entity, string container, string key);

        void AddWithMetadata(Stream entity, string container, string key, IDictionary<string, string> metadata);

        void AddOrReplace(Stream entity, string container, string key);

        void Delete(string container, string key);

        Stream RetrieveAsStream(string container, string key);
    }
}
