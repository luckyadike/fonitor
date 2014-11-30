namespace Fonitor.Controllers
{
	using Fonitor.Data.Repositories;
	using Fonitor.Data.Services;
	using Fonitor.Filters;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using System.Web.Http;

    /// <summary>
    /// Controller for image related actions.
    /// </summary>
	public class ImageController : ApiController
	{
		private IBlobRepository imageBlobRepository { get; set; }

		private QueueRepository imageQueueRepository { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
		public ImageController()
		{
			var storage = new StorageService();

			imageBlobRepository = new BlobRepository(storage);

			imageQueueRepository = new QueueRepository(storage);
		}

        /// <summary>
        /// Constructor with repository parameter.
        /// </summary>
        /// <param name="blobRepository">The data repository to use.</param>
		public ImageController(IBlobRepository blobRepository, QueueRepository queueRepository)
		{
			imageBlobRepository = blobRepository;

            imageQueueRepository = queueRepository;
		}

		// POST api/image/upload
		/// <summary>
		/// This entrypoint receives images from sensors.
		/// </summary>
		/// <returns>A HttpResponseMessage containing the operation status.</returns>
		[RequireAPIKeyAndSensorId]
		public Task<HttpResponseMessage> Upload()
		{
			if (!Request.Content.IsMimeMultipartContent())
			{
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}

			string apiKey;
			string sensorId;
			Constants.ExtractRequestIdentity(out apiKey, out sensorId);

			var task = Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(new MultipartMemoryStreamProvider()).
				ContinueWith(t =>
				{
					if (t.IsFaulted || t.IsCanceled)
					{
						throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception));
					}

					var provider = t.Result;
					if (provider.Contents.Count == 0)
					{
						return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The request is empty.");
					}

					var content = provider.Contents.First();

					var metadata = new Dictionary<string, string> { { "SensorId", sensorId }, { "ApiKey", apiKey } };

					var imageKey = Guid.NewGuid().ToString("N");

					// Add the image to the image container.
                    imageBlobRepository.AddWithMetadata(content.ReadAsStreamAsync().Result, Constants.ImageTableName, imageKey, metadata);

					// Also add it to the queue.
					imageQueueRepository.Enqueue(Constants.ImageTableName, imageKey);

					return Request.CreateResponse(HttpStatusCode.OK);

				});

			return task;
		}
	}
}
