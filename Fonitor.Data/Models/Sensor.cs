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

		public Sensor(string partitionKey, string rowKey)
		{
			PartitionKey = partitionKey;

			RowKey = rowKey;
		}

		public string Name { get; set; }

		public string Description { get; set; }

		public SensorType Type { get; set; }

		public string Id { get; set; }

		public DeviceOS HostOS { get; set; }
	}
}
