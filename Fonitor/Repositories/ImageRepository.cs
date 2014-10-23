namespace Fonitor.Repositories
{
	using Fonitor.Models;
	using Fonitor.Services;

	public class ImageRepository : Repository<Image>
	{
		public ImageRepository(TableStorageService service)
			: base(service, TableName)
		{
		}

		public static string TableName
		{
			get
			{
				return "Image";
			}
		}
	}
}