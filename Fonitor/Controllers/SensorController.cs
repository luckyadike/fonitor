﻿namespace Fonitor.Controllers
{
	using Fonitor.Filters;
	using FonitorData.Models;
	using FonitorData.Repositories;
	using FonitorData.Services;
	using FonitorData.ViewModels;
	using System.Linq;
	using System.Web.Http;
	using System.Web.Http.ModelBinding;

    /// <summary>
    /// Controller for sensor related actions.
    /// </summary>
	public class SensorController : ApiController
	{
		private Repository<Sensor> sensorRepository { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
		public SensorController()
		{
			sensorRepository = new Repository<Sensor>(new TableStorageService(), Constants.SensorTableName);
		}

        /// <summary>
        /// Constructor with repository parameter.
        /// </summary>
        /// <param name="repository">The data repository to use.</param>
		public SensorController(Repository<Sensor> repository)
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
		public IHttpActionResult Register(RegisterSensor sensorModel)
		{
			return ModelState.IsValid ? RegisterSensor(sensorModel) : BadRequest(ModelState);
		}

		private IHttpActionResult RegisterSensor(RegisterSensor sensorModel)
		{
			string apiKey;
			string sensorId;
			Constants.ExtractRequestIdentity(out apiKey, out sensorId);

			var uniqueSensorId = Security.SHA1Hash(sensorModel.Id + apiKey);

			// Check if the sensor exists for the requesting ApiKey.
			if (sensorRepository.RetrievePartition(uniqueSensorId).Any())
			{
				return BadRequest("This sensor has already been registered to the specified ApiKey.");
			}
			else
			{
				// This is a new sensor.
				// Add it!
				var sensor = new Sensor(uniqueSensorId, sensorModel.Name, sensorModel.Description, sensorModel.Sensor, sensorModel.HostOS);

				sensorRepository.Add(sensor);

				return Ok<string>(uniqueSensorId);
			}
		}
	}
}