namespace Fonitor.Models
{
	using Microsoft.WindowsAzure.Storage.Table;
	using System;

	public class Entity : TableEntity
	{
		public Entity(string apiKey, string sensorId)
		{
			ApiKey = apiKey;

			SensorId = sensorId;
		}

		public string ApiKey { get; set; }

		public string SensorId { get; set; }
	}
}