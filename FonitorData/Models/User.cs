namespace FonitorData.Models
{
	using Microsoft.WindowsAzure.Storage.Table;
	using System;

	public sealed class User : TableEntity
	{
		public User() { }

		public User(string emailAddress, Guid id, string password)
		{
			EmailAddress = emailAddress;

		    EncryptedPassword = password;

			VerificationId = id;

			PartitionKey = emailAddress;

			RowKey = password;
		}

		public string EmailAddress { get; set; }

		public Guid VerificationId { get; set; }

		public string EncryptedPassword { get; set; }
	}
}
