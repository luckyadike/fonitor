namespace FonitorData.Models
{
	using Microsoft.WindowsAzure.Storage.Table;
	using System;

	public sealed class User : TableEntity
	{
		public User() { }

		public User(string partitionKey, string rowKey, Guid id)
		{
			EmailAddress = partitionKey;

		    EncryptedPassword = rowKey;

			VerificationId = id;

			//
			PartitionKey = partitionKey;

			RowKey = rowKey;
		}

		public string EmailAddress { get; set; }

		public Guid VerificationId { get; set; }

		public string EncryptedPassword { get; set; }
	}
}
