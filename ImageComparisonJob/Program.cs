namespace ImageComparisonJob
{
    using Microsoft.Azure.WebJobs;
    using System;
    using System.Configuration;
    using System.IO;

    public class Program
    {
        static void Main()
        {
            var host = new JobHost();
            host.RunAndBlock();
        }

        public static void CompareUploadedImage(
            [Blob("image/{name}", FileAccess.Read)] Stream input,
            string name,
            IBinder binder)
        {
            var output = binder.Bind<Stream>(new BlobAttribute(string.Format("{0}/{1}", name, Guid.NewGuid().ToString("N")), FileAccess.Write));
            output = input;
        }
    }
}
