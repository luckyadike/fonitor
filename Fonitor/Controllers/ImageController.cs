namespace Fonitor.Controllers
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
		private BlobRepository imageRepository { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
		public ImageController()
		{
            imageRepository = new BlobRepository(new BlobStorageService());
		}

        /// <summary>
        /// Constructor with repository parameter.
        /// </summary>
        /// <param name="repository">The data repository to use.</param>
		public ImageController(BlobRepository repository)
		{
			imageRepository = repository;
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

					var metadata = new Dictionary<string, string> { { "SensorId", sensorId } };

                    imageRepository.AddWithMetadata(content.ReadAsStreamAsync().Result, Constants.ImageTableName, Guid.NewGuid().ToString("N"), metadata);

					return Request.CreateResponse(HttpStatusCode.OK);

				});

			return task;
		}
	}
}
