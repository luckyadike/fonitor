namespace Fonitor.Controllers
{
	using Fonitor.Repositories;
	using Fonitor.Services;
	using System;
	using System.Configuration;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Security.Claims;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web.Http;
	using XnaFan.ImageComparison;

	public class ImagesController : ApiController
	{
		private Repository<Fonitor.Models.Image> imageRepository { get; set; }

		public ImagesController()
		{
			imageRepository = new ImageRepository(new TableStorageService());
		}

		public ImagesController(Repository<Fonitor.Models.Image> repository)
		{
			imageRepository = repository;
		}

		// POST api/images
		/// <summary>
		/// This entrypoint receives images and processes them.
		/// </summary>
		/// <param name="reset">Indicates if the base image is to be overwritten by the new one.</param>
		/// <returns>A HttpResponseMessage containing the operation status.</returns>
		public Task<HttpResponseMessage> Post(bool reset = false)
		{
			if (!Request.Content.IsMimeMultipartContent())
			{
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}

			// Consider making this better?
			var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

			var apiKey = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).Single();

			var sensorId = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).Single();

			var task = Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(new MultipartMemoryStreamProvider()).
				ContinueWith(t =>
				{
					if (t.IsFaulted || t.IsCanceled)
					{
						throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception));
					}

					var provider = t.Result;

					string message = string.Empty;
					foreach (var content in provider.Contents)
					{
						// Incoming image.
						var stream = content.ReadAsStreamAsync().Result;

						if (reset)
						{
							// Replace the base image.
							using (var ms = new MemoryStream())
							{
								stream.CopyTo(ms);

								Fonitor.Models.Image image = new Fonitor.Models.Image(ms.ToArray(), apiKey, sensorId, DateTime.Now);

								imageRepository.AddOrReplace(image);
							}
						}
						else if (!SimilarToBaseImage(stream, apiKey, sensorId))
						{
							// Notify the caller.
							// For now just add it to the response.

							message = "This image is different.";
						}

						// There should just be 1 image for request.
						break;
					}

					return Request.CreateResponse(HttpStatusCode.OK, message);
				});

			return task;
		}

		public bool SimilarToBaseImage(Stream newImage, string apiKey, string sensorId)
		{
			// Get base image.
			var baseImage = imageRepository.Retrieve(apiKey, sensorId);

			var threshold = int.Parse(ConfigurationManager.AppSettings["MaxImageDivergencePercent"]);

			var diff = ImageComparison.PercentageDifference(Image.FromStream(newImage), Image.FromStream(new MemoryStream(baseImage.Blob))) * 100;

			return diff < threshold ? true : false;
		}
	}
}
