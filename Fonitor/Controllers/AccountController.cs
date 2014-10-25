namespace Fonitor.Controllers
{
	using FonitorData.Repositories;
	using FonitorData.Services;
	using FonitorData.ViewModels;
	using System;
	using System.Linq;
	using System.Web.Http;

	public class AccountController : ApiController
	{
		private Repository<FonitorData.Models.User> userRepository { get; set; }

		public AccountController()
		{
			userRepository = new Repository<FonitorData.Models.User>(new TableStorageService(), Constants.UserTableName);
		}

		public AccountController(Repository<FonitorData.Models.User> repository)
		{
			userRepository = repository;
		}

		// POST api/account/register
		[HttpPost]
		public IHttpActionResult Register(RegisterUser userModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return RegisterUser(userModel);
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
				// Add the user.
				var user = new FonitorData.Models.User(userModel.EmailAddress, guid, password);

				userRepository.Add(user);

				// Return the guid (Api key) to the client.

				return new TextResult(string.Format("Key: {0}", guid.ToString()), Request);
			}
		}
	}
}