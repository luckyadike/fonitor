namespace Fonitor.Controllers
{
	using Fonitor.Repositories;
	using Fonitor.Services;
	using System;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Security.Claims;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web.Http;

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
		public Task<HttpResponseMessage> Post()
		{
			if (!Request.Content.IsMimeMultipartContent())
			{
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}

			// Consider making this better?
			var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

			var apiKey = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).Single();

			var sensorId = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c =>c.Value).Single();

			var task = Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(new MultipartMemoryStreamProvider()).
				ContinueWith(t =>
				{
					if (t.IsFaulted || t.IsCanceled)
					{
						throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception));
					}

					var provider = t.Result;

					foreach (var content in provider.Contents)
					{
						var stream = content.ReadAsStreamAsync().Result;

						// Write to store.
						using (var ms = new MemoryStream())
						{
							stream.CopyTo(ms);

							Fonitor.Models.Image image = new Fonitor.Models.Image(ms.ToArray(), apiKey, sensorId, DateTime.Now);

							imageRepository.AddOrReplace(image);
						}
					}

					return Request.CreateResponse(HttpStatusCode.OK);
				});

			return task;
		}
	}
}
