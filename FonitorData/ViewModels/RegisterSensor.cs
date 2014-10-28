namespace FonitorData.ViewModels
{
	using FonitorData.Models;
	using System.ComponentModel.DataAnnotations;

	public class RegisterSensor
	{
        /// <summary>
        /// Simple name e.g. Front door Camera 1
        /// </summary>
		[Required]
		public string Name { get; set; }

        /// <summary>
        /// Description e.g. This is the camera facing the backyard.
        /// </summary>
		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		public string Description { get; set; }

        /// <summary>
        /// The type of sensor.
        /// </summary>
		[Required]
		public SensorType Sensor { get; set; }

        /// <summary>
        /// A unique Id. All sensors belonging to a user must have different Ids.
        /// </summary>
		[Required]
		public string Id { get; set; }

        /// <summary>
        /// The operating system of the host device.
        /// </summary>
		public DeviceOS HostOS { get; set; }
	}
}
