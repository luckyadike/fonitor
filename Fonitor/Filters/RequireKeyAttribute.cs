namespace Fonitor.Filters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Web;
	using System.Web.Http.Filters;

	public class RequireKeyAttribute : AuthorizationFilterAttribute
	{
		public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
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