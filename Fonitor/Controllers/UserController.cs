﻿namespace Fonitor.Controllers
{
	using Fonitor.Filters;
	using Fonitor.Data.Repositories;
	using Fonitor.Data.Services;
	using Fonitor.Data.ViewModels;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http;

    /// <summary>
    /// Controller for user related actions.
    /// </summary>
	public class UserController : ApiController
	{
		private ITableRepository<Fonitor.Data.Models.User> userRepository { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
		public UserController()
		{
			userRepository = new TableRepository<Fonitor.Data.Models.User>(new TableStorageService(), Constants.UserTableName);
		}

        /// <summary>
        /// Constructor with repository parameter.
        /// </summary>
        /// <param name="repository">The data repository to use.</param>
		public UserController(ITableRepository<Fonitor.Data.Models.User> repository)
		{
			userRepository = repository;
		}

		// POST api/user/register
        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns>A HttpResponseMessage containing the operation status.</returns>
        [RequireValidViewModel]
		public IHttpActionResult Register(RegisterUser userModel)
		{
			var guid = Guid.NewGuid();

			var password = Security.SHA1Hash(userModel.Password + guid);

			// Check if the user exists.
			if (userRepository.RetrievePartition(userModel.EmailAddress).Any())
			{
				// There is a user with this email address.
				return BadRequest("The email address has already been taken.");
			}
			else
			{
				// This is a new user.
				// Add it!
				userRepository.Add(new Fonitor.Data.Models.User(userModel.EmailAddress, password)
				{
					VerificationId = guid,
					EmailAddress = userModel.EmailAddress,
					EncryptedPassword = password,
					PhoneNumber = userModel.PhoneNumber
				});

				var apiKey = guid.ToString("N");

				// Also add entry using apiKey as partitionKey					
				userRepository.Add(new Fonitor.Data.Models.User(apiKey, password)
				{
					VerificationId = guid,
					EmailAddress = userModel.EmailAddress,
					EncryptedPassword = password,
					PhoneNumber = userModel.PhoneNumber
				});

				return Ok<string>(apiKey);
			}
		}
	}
}