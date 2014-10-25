namespace Fonitor
{
	using System.Security.Cryptography;
	using System.Text;

	public class Security
	{
		public static string SHA1Hash(string value)
		{
			var sha1 = SHA1.Create();
			var input = Encoding.ASCII.GetBytes(value);
			var hash = sha1.ComputeHash(input);

			var sb = new StringBuilder();
			for (var i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}

			return sb.ToString();
		}
	}
}