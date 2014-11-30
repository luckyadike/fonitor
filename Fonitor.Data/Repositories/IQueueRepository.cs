namespace Fonitor.Data.Repositories
{
    public interface IQueueRepository
    {
        void Enqueue(string name, string item);

        string DeQueue(string name);
    }
}
