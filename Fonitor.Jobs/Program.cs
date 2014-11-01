namespace Fonitor.Jobs
{
    using Fonitor.Data.Models;
    using Fonitor.Data.Repositories;
    using Fonitor.Data.Services;
    using Fonitor.Notification;
    using Microsoft.Azure.WebJobs;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
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

		static readonly TableRepository<User> userRepository =
			new TableRepository<User>(new TableStorageService(), "User");

        static readonly TableRepository<Sensor> sensorRepository =
            new TableRepository<Sensor>(new TableStorageService(), "Sensor");

        static void Main()
        {
            var host = new JobHost();

            host.RunAndBlock();
        }

		/// <summary>
		/// Compare images added to the container with the base.
		/// Moves the new images from the generic image container to sensor specific ones.
		/// </summary>
		/// <param name="input">The new image.</param>
		/// <param name="name">The bound name parameter of the image.</param>
        public static void CompareUploadedImage(
            [BlobTrigger("image/{name}")] CloudBlockBlob input,
            string name)
        {
			// Get metadata.
			if (!input.Metadata.ContainsKey(Program.SensorIdString))
			{
				Console.WriteLine("SensorId is missing from the metadata.");
				return;
			}

            var id = input.Metadata[Program.SensorIdString];

            var inputStream = ExtractStream(input);

            var baseImgKey = "base";

			var baseImage = imageRepository.Retrieve(id, baseImgKey);
            if (baseImage == null)
            {
                // Set the base item.
				imageRepository.Add(inputStream, id, baseImgKey);
            }
            else
            {
                var threshold = int.Parse(ConfigurationManager.AppSettings["MaxImageDivergencePercent"]);

			    // Compare this to the new image.
				var diff = ImageComparison.PercentageDifference(Image.FromStream(inputStream), Image.FromStream(baseImage)) * 100;
				if (diff > threshold)
				{
					// Images are different.
					// Add this image to the notification container.
					imageRepository.AddWithMetadata(inputStream, "notification", UniqueString(), input.Metadata);
				}
            }

			imageRepository.Add(inputStream, id, name);
        }

        private static MemoryStream ExtractStream(CloudBlockBlob input)
        {
            // Get the input stream.
            var inputStream = new MemoryStream();

            input.DownloadToStream(inputStream);
            return inputStream;
        }

		/// <summary>
		/// Sends notifications.
		/// </summary>
		/// <param name="input">The blob representing the event.</param>
		/// <param name="name">The bound name parameter of the blob.</param>
		public static void SendNotification(
			[BlobTrigger("notification/{name}")] CloudBlockBlob input,
			string name)
		{
			// Get metadata.
            if (!input.Metadata.ContainsKey(Program.ApiKeyString))
			{
				Console.WriteLine("ApiKey is missing from the metadata.");
				return;
			}

            var key = input.Metadata[Program.ApiKeyString];

			// Get the email address for the user.
			var user = userRepository.RetrievePartition(key);
			if (user == null || user.Count() == 0)
			{
				Console.WriteLine("No data found for the user.");
				return;
			}

            // Consider caching this stream?
            var inputStream = ExtractStream(input);

            var id = input.Metadata[Program.SensorIdString];

            // Get the sensor details.
            var sensor = sensorRepository.RetrievePartition(id);
            if (sensor == null || sensor.Count() == 0)
            {
                Console.WriteLine("No data found for the sensor.");
                return;
            }

			// Send an email.
            // Send a text? (Base this on the user's settings)
            Email.SendImageChangeNotification(user.First().EmailAddress, sensor.First().Name, inputStream);
		}
    }
}
