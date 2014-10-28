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

    /// <summary>
    /// Controller for user related actions.
    /// </summary>
	public class UserController : ApiController
	{
		private Repository<FonitorData.Models.User> userRepository { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
		public UserController()
		{
			userRepository = new Repository<FonitorData.Models.User>(new TableStorageService(), Constants.UserTableName);
		}

        /// <summary>
        /// Constructor with repository parameter.
        /// </summary>
        /// <param name="repository">The data repository to use.</param>
		public UserController(Repository<FonitorData.Models.User> repository)
		{
			userRepository = repository;
		}

		// POST api/user/register
        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns>A HttpResponseMessage containing the operation status.</returns>
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