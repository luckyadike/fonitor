﻿namespace ImageComparisonJob
{
	using FonitorData.Repositories;
	using Microsoft.Azure.WebJobs;
	using Microsoft.WindowsAzure.Storage.Blob;
	using System;
	using System.Configuration;
	using System.Drawing;
	using System.IO;
	using XnaFan.ImageComparison;

    public class Program
    {
        private static readonly BlobRepository repository =
            new BlobRepository(new FonitorData.Services.BlobStorageService());

        static void Main()
        {
            var host = new JobHost();

            host.RunAndBlock();
        }

        public static void CompareUploadedImage(
            [BlobTrigger("image/{name}")] CloudBlockBlob input,
            string name)
        {
			// Get metadata.
			if (!input.Metadata.ContainsKey("SensorId"))
			{
				Console.WriteLine("SensorId is missing from the metadata.");

				return;
			}

			var id = input.Metadata["SensorId"];

			// Get the input stream.
			var inputStream = new MemoryStream();

			input.DownloadToStream(inputStream);

            var baseImgKey = "base";

			var baseImage = repository.Retrieve(id, baseImgKey);
            if (baseImage == null)
            {
                // Set the base item.
				repository.Add(inputStream, id, baseImgKey);
            }
            else
            {
                var threshold = int.Parse(ConfigurationManager.AppSettings["MaxImageDivergencePercent"]);

			    // Compare this to the new image.
				var diff = ImageComparison.PercentageDifference(Image.FromStream(inputStream), Image.FromStream(baseImage)) * 100;
				if (diff > threshold)
				{
					// Images are different.
					// The timestamp at this point can be used to get all the different images.
					// Do something?
					Console.WriteLine("Different");
				}
				else
				{
					Console.WriteLine("Same");
				}
            }

			repository.Add(inputStream, id, name);
        }
    }
}
