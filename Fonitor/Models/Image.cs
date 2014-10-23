namespace Fonitor.Models
{
	using System;

	public sealed class Image : Entity
	{
		public Image(byte[] blob, string apiKey, string sensorId, DateTime creationTime)
			: base(apiKey, sensorId)
		{
			PartitionKey = apiKey;

			RowKey = sensorId;

			Blob = blob;

			CreationTime = creationTime;
		}

		public byte[] Blob { get; set; }

		public DateTime CreationTime { get; set; }
	}
}