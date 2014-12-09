namespace Fonitor.Filters
{
	using Fonitor.Data.Models;
	using Fonitor.Data.Repositories;
	using Fonitor.Data.Services;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Filters;

	public class RequireAPIKey : AuthorizationFilterAttribute
	{
		private ITableRepository<User> userRepository { get; set; }

		public RequireAPIKey()
		{
			userRepository = new TableRepository<User>(new TableStorageService(), Constants.UserTableName);
		}

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var request = actionContext.Request;

			string apiKey;
			string sensorId;
			Constants.ExtractRequestIdentity(out apiKey, out sensorId);

			if (string.IsNullOrEmpty(apiKey))
			{
				actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.BadRequest);
			}

			// Is this key registered?
			if (userRepository.RetrievePartition(apiKey).Any())
			{
				base.OnAuthorization(actionContext);
			}
			else
			{
				actionContext.Response = new HttpResponseMessage
				{
					Content = new StringContent("Unknown ApiKey"),
					StatusCode = HttpStatusCode.Unauthorized
				};
			}
		}
	}
}