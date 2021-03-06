﻿namespace Fonitor.Filters
{
	using Fonitor.Data.Models;
	using Fonitor.Data.Repositories;
	using Fonitor.Data.Services;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Filters;

	public class RequireAPIKeyAndSensorId : AuthorizationFilterAttribute
	{
		private ITableRepository<User> userRepository { get; set; }

		private ITableRepository<Sensor> sensorRepository { get; set; }

		public RequireAPIKeyAndSensorId()
		{
			userRepository = new TableRepository<User>(new TableStorageService(), Constants.UserTableName);

			sensorRepository = new TableRepository<Sensor>(new TableStorageService(), Constants.SensorTableName);
		}

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var request = actionContext.Request;

			string apiKey;
			string sensorId;
			Constants.ExtractRequestIdentity(out apiKey, out sensorId);

			if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(sensorId))
			{
				actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
			}

			// Is this key registered?
			if (userRepository.RetrievePartition(apiKey).Any() && sensorRepository.RetrievePartition(sensorId).Any())
			{
				base.OnAuthorization(actionContext);
			}
			else
			{
				actionContext.Response = new HttpResponseMessage
				{
					Content = new StringContent("Unknown ApiKey or SensorId"),
					StatusCode = HttpStatusCode.Unauthorized
				};
			}
		}
	}
}