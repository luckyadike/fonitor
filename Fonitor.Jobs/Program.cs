namespace Fonitor.Jobs
{
    using Fonitor.Data;
    using Fonitor.Data.Models;
    using Fonitor.Data.Repositories;
    using Fonitor.Data.Services;
    using Fonitor.Notification;
    using Microsoft.Azure.WebJobs;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Newtonsoft.Json;
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Linq;
    using XnaFan.ImageComparison;

    public class Program
    {
        static readonly string ApiKeyString = "ApiKey";

        static readonly string SensorIdString = "SensorId";

		static string UniqueString()
		{
			return Guid.NewGuid().ToString("N");
		}

        static readonly BlobRepository imageRepository =
            new BlobRepository(new StorageService());

		static readonly ITableRepository<User> userRepository =
			new TableRepository<User>(new TableStorageService(), "User");

        static readonly ITableRepository<Sensor> sensorRepository =
            new TableRepository<Sensor>(new TableStorageService(), "Sensor");

        static void Main()
        {
			var configuration = new JobHostConfiguration();
			configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(2);
			configuration.Queues.MaxDequeueCount = 2;

			// Increase this as an optimization.
			configuration.Queues.BatchSize = 1;

            var host = new JobHost(configuration);

            host.RunAndBlock();
        }

		/// <summary>
		/// Compare images added to the container with the base.
		/// Moves the new images from the generic image container to sensor specific ones.
		/// </summary>
		/// <param name="input">A reference to the image.</param>
        /// <param name="notification">A reference to the image in the notifications queue.</param>
		public static void CompareUploadedImage(
			[QueueTrigger("image")] string input,
			[Queue("notification")] out string notification)
		{
			notification = null;

			// Get the real image.
			// Consider caching it?
			var imageBlob = imageRepository.RetrieveAsBlob("image", input);
			if (!imageBlob.Exists())
			{
				Console.WriteLine("Could not retrieve image with key {0} from image container.", input);
				return;
			}

			// Get metadata.
			imageBlob.FetchAttributes();

			if (!imageBlob.Metadata.ContainsKey(Program.SensorIdString))
			{
				Console.WriteLine("SensorId is missing from the metadata.");
				return;
			}

			var sensorId = imageBlob.Metadata[Program.SensorIdString];

            if (!imageBlob.Metadata.ContainsKey(Program.ApiKeyString))
            {
                Console.WriteLine("ApiKey is missing from the metadata.");
                return;
            }

            var apiKey = imageBlob.Metadata[Program.ApiKeyString];

			var inputStream = imageBlob.ExtractStream();

			var baseImgKey = "base";

			var baseImage = imageRepository.RetrieveAsStream(sensorId, baseImgKey);
            if (baseImage.Length == 0)
            {
                // Set the base item.
                imageRepository.Add(inputStream, sensorId, baseImgKey);
            }
            else
            {
                var threshold = int.Parse(ConfigurationManager.AppSettings["MaxImageDivergencePercent"]);

                // Compare this to the new image.
                var diff = ImageComparison.PercentageDifference(Image.FromStream(inputStream), Image.FromStream(baseImage)) * 100;
                if (diff > threshold)
                {
                    // Images are different.
                    // Add this image to the notification queue.

                    notification = JsonConvert.SerializeObject(new NotificationQueueItem
                    {
                        Container = sensorId,
                        Key = input,
                        ApiKey = apiKey
                    });
                }
            }

            // Move this image to a sensor specific table.
			imageRepository.Add(inputStream, sensorId, input);
            
            // Delete the image from the global image repo.
            imageBlob.Delete(DeleteSnapshotsOption.None);
		}

		/// <summary>
		/// Sends notifications.
		/// </summary>
		/// <param name="input">The queue item representing the event.</param>
		public static void SendNotification(
			[QueueTrigger("notification")] NotificationQueueItem input)
		{
			// Get the real image.
			var imageBlob = imageRepository.RetrieveAsBlob(input.Container, input.Key);
			if (!imageBlob.Exists())
			{
				Console.WriteLine("Could not retrieve image with key {0} from image container.", input);
				return;
			}

			// Get the email address for the user.
			var user = userRepository.RetrievePartition(input.ApiKey);
			if (user == null || user.Count() == 0)
			{
				Console.WriteLine("No data found for the user.");
				return;
			}

			// Consider caching this stream?
            var inputStream = imageBlob.ExtractStream();

			// Get the sensor details.
			var sensor = sensorRepository.RetrievePartition(input.Container);
			if (sensor == null || sensor.Count() == 0)
			{
				Console.WriteLine("No data found for the sensor.");
				return;
			}

			// Send an email.
			Email.SendImageChangeNotification(user.First().EmailAddress, sensor.First().Name, inputStream);

            // Send a text.
            // Text or email or both? Give the user an option?
            Phone.SendImageChangeNotification(user.First().PhoneNumber, sensor.First().Name, inputStream);
		}
    }
}
