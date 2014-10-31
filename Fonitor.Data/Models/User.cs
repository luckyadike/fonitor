namespace Fonitor.Data.Models
{
	using Microsoft.WindowsAzure.Storage.Table;
	using System;

	public sealed class User : TableEntity
	{
		public User() { }

		public User(string partitionKey, string rowKey)
		{
			PartitionKey = partitionKey;

			RowKey = rowKey;
		}

		public string EmailAddress { get; set; }

		public string PhoneNumber { get; set; }

		public Guid VerificationId { get; set; }

		public string EncryptedPassword { get; set; }
	}
}
