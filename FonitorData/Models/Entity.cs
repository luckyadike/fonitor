namespace FonitorData.Models
{
	using Microsoft.WindowsAzure.Storage.Table;
	using System;

	public abstract class Entity : TableEntity
	{
		public Entity() { }

		public Entity(string apiKey, string sensorId)
		{
			ApiKey = apiKey;

			SensorId = sensorId;
		}

		public string ApiKey { get; set; }

		public string SensorId { get; set; }
	}
}