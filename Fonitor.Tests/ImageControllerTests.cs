namespace Fonitor.Tests
{
    using Fonitor.Controllers;
    using Fonitor.Tests.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

	[TestClass]
	public class ImageControllerTests
	{
        InMemoryBlobRepository BlobRepository;

        InMemoryQueueRepository QueueRepository;

        ImageController Controller;

        [TestInitialize]
        public void Setup()
        {
            BlobRepository = new InMemoryBlobRepository();

            QueueRepository = new InMemoryQueueRepository();

            Controller = new ImageController(BlobRepository, QueueRepository);
        }

		[TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
		public async Task JustMimeMultipartContentIsAllowed()
		{
            Controller.Request = new HttpRequestMessage(HttpMethod.Post, "/test/upload")
            {
                Content = new StreamContent(new MemoryStream())
            };

            await Controller.Upload();
		}

        [TestMethod]
        public async Task UploadedContentCannotBeEmpty()
        {
            Controller.Request = new HttpRequestMessage(HttpMethod.Post, "/test/upload");

            Controller.Request.Content = new MultipartContent();

            var response = await Controller.Upload();

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task UploadedImageIsPersisted()
        {
            var fileContents = File.ReadAllBytes("../../data/images-free-morning-glory.jpg");

            var byteArrayContent = new ByteArrayContent(fileContents);

            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");

            var multipartContent = new MultipartFormDataContent("----TestBoundary");

            multipartContent.Add(byteArrayContent);

            Controller.Request = new HttpRequestMessage(HttpMethod.Post, "/test/upload");

            Controller.Request.Content = multipartContent;

            Assert.IsTrue(BlobRepository.Count() == 0);

            Assert.IsTrue(QueueRepository.Count() == 0);

            await Controller.Upload();

            Assert.IsTrue(BlobRepository.Count() == 1);

            Assert.IsTrue(QueueRepository.Count() == 1);
        }
	}
}
