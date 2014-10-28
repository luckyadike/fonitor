namespace FonitorData.ViewModels
{
	using FonitorData.Models;
	using System.ComponentModel.DataAnnotations;

	public class RegisterSensor
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		public string Description { get; set; }

		[Required]
		public SensorType Sensor { get; set; }

		[Required]
		public string Id { get; set; }

		public DeviceOS HostOS { get; set; }
	}
}
