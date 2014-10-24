namespace Fonitor.Models
{
	using Microsoft.WindowsAzure.Storage.Table;
	using System;

	public sealed class Image : BlobEntity
	{
		public Image() { }

		public Image(byte[] blob, string apiKey, string sensorId)
			: base(blob, apiKey, sensorId)
		{
			PartitionKey = apiKey;

			RowKey = sensorId;
		}

		[IgnorePropertyAttribute]
		public byte[] Blob
		{
			get
			{
				return GetData();
			}
		}
	}
}