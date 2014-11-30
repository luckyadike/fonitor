namespace Fonitor.Data.Models
{
    /// <summary>
    /// POCO for notification queue.
    /// </summary>
    public class NotificationQueueItem
    {
        public string Container { get; set; }

        public string Key { get; set; }
    }
}
