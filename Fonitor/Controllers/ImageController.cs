﻿namespace Fonitor.Controllers
{
	using Fonitor.Filters;
	using Fonitor.Data.Repositories;
	using Fonitor.Data.Services;
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
		private BlobRepository imageBlobRepository { get; set; }

		private QueueRepository imageQueueRepository { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
		public ImageController()
		{
            imageBlobRepository = new BlobRepository(new StorageService());
		}

        /// <summary>
        /// Constructor with repository parameter.
        /// </summary>
        /// <param name="repository">The data repository to use.</param>
		public ImageController(BlobRepository repository)
		{
			imageBlobRepository = repository;
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
