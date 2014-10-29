namespace Fonitor
{
	using FonitorData.Services;
	using System.Web.Http;
	using System.Web.Mvc;
	using System.Web.Routing;

	public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			AreaRegistration.RegisterAllAreas();

			GlobalConfiguration.Configure(WebApiConfig.Register);

			RouteConfig.RegisterRoutes(RouteTable.Routes);

			GlobalConfiguration.Configuration.MessageHandlers.Add(new IdentityValidationHandler());

			TableStorageService.CreateTableIfNotExists(Constants.UserTableName);

			TableStorageService.CreateTableIfNotExists(Constants.SensorTableName);
        }
    }
}
