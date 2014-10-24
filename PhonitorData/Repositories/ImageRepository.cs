namespace FonitorData.Repositories
{
	using FonitorData.Models;
	using FonitorData.Services;

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