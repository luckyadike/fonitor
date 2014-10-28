namespace Fonitor.Filters
{
	using System.Net;
	using System.Web.Http.Controllers;
	using System.Web.Http.Filters;

	public class RequireAPIKeyAndSensorId : AuthorizationFilterAttribute
	{
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var request = actionContext.Request;

			string apiKey;
			string sensorId;
			Constants.ExtractRequestIdentity(out apiKey, out sensorId);

			if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(sensorId))
			{
				actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.BadRequest);
			}

			// Add an extra step to check the store?

			base.OnAuthorization(actionContext);
		}
	}
}