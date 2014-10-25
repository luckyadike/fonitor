namespace Fonitor.Controllers
{
	using FonitorData.Repositories;
	using FonitorData.Services;
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
		private Repository<FonitorData.Models.Image> imageRepository { get; set; }

		public ImagesController()
		{
			imageRepository = new Repository<FonitorData.Models.Image>(new TableStorageService(), Constants.ImageTableName);
		}

		public ImagesController(Repository<FonitorData.Models.Image> repository)
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

			string apiKey;
			string sensorId;
			ExtractRequestIdentity(out apiKey, out sensorId);

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

					string message = string.Empty;

					// Incoming image.
					var stream = content.ReadAsStreamAsync().Result;

					if (stream.Length > Constants.MaxImageSize)
					{
						return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The image is too large.");
					}

					if (reset)
					{
						// Replace the base image.
						using (var ms = new MemoryStream())
						{
							stream.CopyTo(ms);

							FonitorData.Models.Image image = new FonitorData.Models.Image(ms.ToArray(), apiKey, sensorId);

							imageRepository.AddOrReplace(image);
						}
					}
					else if (!SimilarToBaseImage(stream, apiKey, sensorId))
					{
						// Notify the caller.
						// For now just add it to the response.

						message = "This image is different.";
					}

					return Request.CreateResponse(HttpStatusCode.OK, message);

				});

			return task;
		}

		private static void ExtractRequestIdentity(out string apiKey, out string sensorId)
		{
			var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

			apiKey = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).Single();

			sensorId = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).Single();
		}

		private bool SimilarToBaseImage(Stream newImage, string apiKey, string sensorId)
		{
			// Get base image.
			var baseImage = imageRepository.Retrieve(apiKey, sensorId);

			var threshold = int.Parse(ConfigurationManager.AppSettings["MaxImageDivergencePercent"]);

			var diff = 100.0;
			if (baseImage != null)
			{
				diff = ImageComparison.PercentageDifference(Image.FromStream(newImage), Image.FromStream(new MemoryStream(baseImage.Blob))) * 100;
			}

			return diff < threshold ? true : false;
		}
	}
}
