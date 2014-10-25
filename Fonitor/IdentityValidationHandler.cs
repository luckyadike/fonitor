namespace Fonitor
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Security.Claims;
	using System.Threading;
	using System.Threading.Tasks;

	public class IdentityValidationHandler : DelegatingHandler
	{
		private Task<HttpResponseMessage> Response(HttpStatusCode code, string message)
		{
			return Task.Factory.StartNew(() =>
					{
						return new HttpResponseMessage(code)
						{
							Content = new StringContent(message)
						};
					});
		}
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			IEnumerable<string> apiKeyValues;
			request.Headers.TryGetValues("X-ApiKey", out apiKeyValues);

			IEnumerable<string> sensorIdValues;
			request.Headers.TryGetValues("X-SensorId", out sensorIdValues);

			// Validate?
			var apiKeyClaim = new Claim(ClaimTypes.Name, apiKeyValues.FirstOrDefault());

			// Validate?
			var sensorIdClaim = new Claim(ClaimTypes.Sid, sensorIdValues.FirstOrDefault());

			var identity = new ClaimsIdentity(new[] { apiKeyClaim, sensorIdClaim }, "ApiKey");

			var principal = new ClaimsPrincipal(identity);

			Thread.CurrentPrincipal = principal;

			// Create a better structure to hold this data.
			//		Load the user sensor settings (retention policy etc.)
			//		Load the user notification settings (phone, email etc.)
			//      Consider putting all user data in a single entry?

			return base.SendAsync(request, cancellationToken);
		}
	}
}