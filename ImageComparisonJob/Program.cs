namespace ImageComparisonJob
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
            Console.WriteLine("Triggered");

			// Get metadata.
			if (!input.Metadata.ContainsKey("SensorId"))
			{
				Console.WriteLine("SensorId is missing from the metadata.");

				return;
			}

			var sensorContainer = input.Metadata["SensorId"];

			// Get the input stream.
			var inputStream = new MemoryStream();

			input.DownloadToStream(inputStream);

			Compare(sensorContainer, inputStream);

			repository.Add(inputStream, sensorContainer, name);
        }

		private static void Compare(string sensorContainer, MemoryStream inputStream)
		{
			// Latest key in container name
			var baseImgKey = "base";

			var baseImage = repository.Retrieve(sensorContainer, baseImgKey);
			if (baseImage == null)
			{
				// Set the base item.
				repository.Add(inputStream, sensorContainer, baseImgKey);
			}
			else
			{
				var threshold = int.Parse(ConfigurationManager.AppSettings["MaxImageDivergencePercent"]);

				// Compare this to the new image.
				if (ImageComparison.PercentageDifference(Image.FromStream(inputStream), Image.FromStream(baseImage)) > threshold)
				{
					// Images are different.
					// The timestamp at this point can be used to get all the different images.
					Console.WriteLine("Different");
				}
				else
				{
					Console.WriteLine("Same");
				}
			}
		}
    }
}
