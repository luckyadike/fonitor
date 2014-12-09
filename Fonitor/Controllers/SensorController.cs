namespace Fonitor.Controllers
{
	using Fonitor.Filters;
	using Fonitor.Data.Models;
	using Fonitor.Data.Repositories;
	using Fonitor.Data.Services;
	using Fonitor.Data.ViewModels;
	using System.Linq;
	using System.Web.Http;
	using System.Web.Http.ModelBinding;

    /// <summary>
    /// Controller for sensor related actions.
    /// </summary>
	public class SensorController : ApiController
	{
		private ITableRepository<Sensor> sensorRepository { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
		public SensorController()
		{
			sensorRepository = new TableRepository<Sensor>(new TableStorageService(), Constants.SensorTableName);
		}

        /// <summary>
        /// Constructor with repository parameter.
        /// </summary>
        /// <param name="repository">The data repository to use.</param>
		public SensorController(ITableRepository<Sensor> repository)
		{
			sensorRepository = repository;
		}

		// POST api/sensor/register
        /// <summary>
        /// Registers a new sensor.
        /// </summary>
        /// <param name="sensorModel"></param>
        /// <returns>A HttpResponseMessage containing the operation status.</returns>
		[RequireAPIKey]
        [RequireValidViewModel]
		public IHttpActionResult Register(RegisterSensor sensorModel)
		{
			string apiKey;
			string sensorId;
			Constants.ExtractRequestIdentity(out apiKey, out sensorId);

			var uniqueSensorId = Security.SHA1Hash(sensorModel.Id + apiKey).ToLower();

			// Check if the sensor exists for the requesting ApiKey.
			if (sensorRepository.RetrievePartition(uniqueSensorId).Any())
			{
				return BadRequest("This sensor has already been registered to the specified ApiKey.");
			}
			else
			{
				// This is a new sensor.
				// Add it!
				sensorRepository.Add(new Sensor(uniqueSensorId, apiKey)
				{
					Name = sensorModel.Name,
					Description = sensorModel.Description,
					Id = sensorModel.Id,
					HostOS = sensorModel.HostOS,
					Type = sensorModel.Sensor
				});

				// Also add entry using apiKey as partitionKey
				sensorRepository.Add(new Sensor(apiKey, uniqueSensorId)
				{
					Name = sensorModel.Name,
					Description = sensorModel.Description,
					Id = sensorModel.Id,
					HostOS = sensorModel.HostOS,
					Type = sensorModel.Sensor
				});

				return Ok<string>(uniqueSensorId);
			}
		}
	}
}