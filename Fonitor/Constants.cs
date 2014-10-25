namespace Fonitor
{
	using System.Linq;
	using System.Security.Claims;
	using System.Threading;

	public class Constants
	{
		/// <summary>
		/// This is the maximum size allowable for an image.
		/// </summary>
		public static int MaxImageSize = (960 * 1024 * 3) / 4;

		public static string UserTableName = "User";

		public static string ImageTableName = "Image";

		public static void ExtractRequestIdentity(out string apiKey, out string sensorId)
		{
			var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

			apiKey = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Any() ? identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).Single() : string.Empty;

			sensorId = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Any() ? identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).Single() : string.Empty;
		}

	}
}