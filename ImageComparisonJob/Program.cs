namespace Fonitor.Jobs
{
	using Fonitor.Data.Models;
	using Fonitor.Data.Repositories;
	using Fonitor.Data.Services;
	using Microsoft.Azure.WebJobs;
	using Microsoft.WindowsAzure.Storage.Blob;
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using XnaFan.ImageComparison;

    public class Program
    {
		static string UniqueString()
		{
			return Guid.NewGuid().ToString("N");
		}

        static readonly BlobRepository imageRepository =
            new BlobRepository(new BlobStorageService());

		static readonly TableRepository<User> userRepository = 
			new TableRepository<User>(new TableStorageService(), "User");

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
			var sensorIdString = "SensorId";

			// Get metadata.
			if (!input.Metadata.ContainsKey(sensorIdString))
			{
				Console.WriteLine("SensorId is missing from the metadata.");
				return;
			}

			var id = input.Metadata[sensorIdString];

			// Get the input stream.
			var inputStream = new MemoryStream();

			input.DownloadToStream(inputStream);

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

		/// <summary>
		/// Sends notifications.
		/// </summary>
		/// <param name="input">The blob representing the event.</param>
		/// <param name="name">The bound name parameter of the blob.</param>
		public static void SendNotification(
			[BlobTrigger("notification/{name}")] CloudBlockBlob input,
			string name)
		{
			var apiKeyString = "ApiKey";

			// Get metadata.
			if (!input.Metadata.ContainsKey(apiKeyString))
			{
				Console.WriteLine("ApiKey is missing from the metadata.");
				return;
			}

			var key = input.Metadata[apiKeyString];

			// Get the email address / phone number for the user.
			var user = userRepository.RetrievePartition(key);
			if (user == null || user.Count() == 0)
			{
				Console.WriteLine("No data found for the user.");
				return;
			}

			// Send an email to user.First().EmailAddress
			// Add phone number to the model and use if here too.
			Console.WriteLine(string.Format("Hey {0} something has gone wrong!", user.First().EmailAddress));
		}
    }
}
