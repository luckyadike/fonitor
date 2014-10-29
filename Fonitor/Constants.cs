namespace Fonitor
{
	using System.Linq;
	using System.Security.Claims;
	using System.Threading;

	public class Constants
	{
		public static string UserTableName = "user";

		public static string ImageTableName = "image";

		public static string SensorTableName = "sensor";

		public static void ExtractRequestIdentity(out string apiKey, out string sensorId)
		{
			var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

			apiKey = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Any() ? identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).Single() : string.Empty;

			sensorId = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Any() ? identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).Single() : string.Empty;
		}

	}
}