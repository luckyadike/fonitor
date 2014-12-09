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

			IEnumerable<string> sensorIdValues;

            var claims = new List<Claim>();

            if (request.Headers.TryGetValues("X-ApiKey", out apiKeyValues))
            {
                claims.Add(new Claim(ClaimTypes.Name, apiKeyValues.FirstOrDefault()));
            }

            if (request.Headers.TryGetValues("X-SensorId", out sensorIdValues))
            {
                claims.Add(new Claim(ClaimTypes.Sid, sensorIdValues.FirstOrDefault()));
            }

            var identity = new ClaimsIdentity(claims, "ApiKey");

            var principal = new ClaimsPrincipal(identity);

            Thread.CurrentPrincipal = principal;

			return base.SendAsync(request, cancellationToken);
		}
	}
}