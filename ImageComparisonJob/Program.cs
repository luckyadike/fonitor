namespace ImageComparisonJob
{
    using FonitorData.Models;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    // To learn more about Microsoft Azure WebJobs, please see http://go.microsoft.com/fwlink/?LinkID=401557
    class Program
    {
        static void Main()
        {
            JobHost host = new JobHost();
            host.RunAndBlock();
        }

        public static void CompareUploadedImage()
        {

        }
    }
}
