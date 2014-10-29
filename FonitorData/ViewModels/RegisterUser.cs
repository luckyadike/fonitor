namespace Fonitor.Data.ViewModels
{
	using System.ComponentModel.DataAnnotations;

	public class RegisterUser
	{
        /// <summary>
        /// A valid email address.
        /// </summary>
		[Required]
		[EmailAddress]
		public string EmailAddress { get; set; }

        /// <summary>
        /// Account password.
        /// </summary>
		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

        /// <summary>
        /// Account password confirmation.
        /// </summary>
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
