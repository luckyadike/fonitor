namespace Fonitor.Data.Models
{
	using Microsoft.WindowsAzure.Storage.Table;

	public enum DeviceOS
	{
		Windows = 0,
		IOS,
		Android,
		BlackBerry
	}

	public enum SensorType
	{
		Camera = 0,
		Gyrometer,
		Accelerometer,
		Microphone,
		AmbientLight
	}

	public sealed class Sensor : TableEntity
	{
		public Sensor() { }

		public Sensor(string id, string name, string description, SensorType sensor, DeviceOS os)
		{
			Name = name;

			Description = description;

			Type = sensor;

			Id = id;

			HostOS = os;

			//
			PartitionKey = id;

			RowKey = name;
		}

		public string Name { get; set; }

		public string Description { get; set; }

		public SensorType Type { get; set; }

		public string Id { get; set; }

		public DeviceOS HostOS { get; set; }
	}
}
