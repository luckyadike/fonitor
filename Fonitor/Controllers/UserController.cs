namespace Fonitor.Controllers
{
	using Fonitor.Filters;
	using FonitorData.Repositories;
	using FonitorData.Services;
	using FonitorData.ViewModels;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http;

	public class UserController : ApiController
	{
		private Repository<FonitorData.Models.User> userRepository { get; set; }

		public UserController()
		{
			userRepository = new Repository<FonitorData.Models.User>(new TableStorageService(), Constants.UserTableName);
		}

		public UserController(Repository<FonitorData.Models.User> repository)
		{
			userRepository = repository;
		}

		// POST api/user/register
		public IHttpActionResult Register(RegisterUser userModel)
		{
			return ModelState.IsValid ? RegisterUser(userModel) : BadRequest(ModelState);
		}

		private IHttpActionResult RegisterUser(RegisterUser userModel)
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
				userRepository.Add(new FonitorData.Models.User(userModel.EmailAddress, password, guid));

				var apiKey = guid.ToString("N").ToUpper();

				// Map the user to its key.					
				userRepository.Add(new FonitorData.Models.User(apiKey, userModel.EmailAddress, Guid.Empty));

				return Ok<string>(apiKey);
			}
		}
	}
}