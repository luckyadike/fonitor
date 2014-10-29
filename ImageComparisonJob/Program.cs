namespace ImageComparisonJob
{
    using FonitorData.Repositories;
    using Microsoft.Azure.WebJobs;
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
            [BlobTrigger("image/{name}")] Stream input,
            string name,
            [Blob("output/ShouldNotBeCreated")] Stream output)
        {
            Console.WriteLine("Triggered");

            // Latest key in container name
            var recent = "recent";

            // Retrieve the latest entry in the container.
            var baseImage = repository.Retrieve(name, recent);
            if (baseImage == null)
            {
                // Set the latest item.
                repository.AddOrReplace(input, name, recent);   
            }
            else
            {
                var threshold = int.Parse(ConfigurationManager.AppSettings["MaxImageDivergencePercent"]);

                // Compare this to the new image.
                if (threshold < ImageComparison.PercentageDifference(Image.FromStream(input), Image.FromStream(baseImage)))
                {
                    // Images are different.
                    Console.WriteLine("Different");
                }
                else
                {
                    Console.WriteLine("Same");
                }
            }

            repository.AddOrReplace(input, name, Guid.NewGuid().ToString("N"));
        }
    }
}
