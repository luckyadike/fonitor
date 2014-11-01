namespace Fonitor.Data.Services
{
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;

    public class StorageService
    {
        protected static readonly CloudStorageAccount storageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

        public CloudStorageAccount StorageAccount
        {
            get
            {
                return storageAccount;
            }
        }
    }
}
