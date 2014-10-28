namespace Fonitor
{
	using FonitorData.Repositories;
	using FonitorData.Services;
	using System.Web.Http;

	public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

			GlobalConfiguration.Configuration.MessageHandlers.Add(new IdentityValidationHandler());

			TableStorageService.CreateTableIfNotExists(Constants.ImageTableName);

			TableStorageService.CreateTableIfNotExists(Constants.UserTableName);

			TableStorageService.CreateTableIfNotExists(Constants.SensorTableName);
        }
    }
}
