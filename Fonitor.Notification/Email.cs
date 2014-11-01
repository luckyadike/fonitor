namespace Fonitor.Notification
{
    using SendGrid;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Net.Mail;

	public class Email
    {
        public static void SendImageChangeNotification(string recipientAddress,
                                                       string sensorName,
                                                       MemoryStream changedImageStream)
        {
            // Retrieve settings.
            // Ensure that they are set?
            var senderAddress = ConfigurationManager.AppSettings["EmailAddress"];

            var sendGridUserName = ConfigurationManager.AppSettings["SendGridUsername"];

            var sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"];

            // SendGrid setup.
            var message = new SendGridMessage();

            message.AddTo(recipientAddress);

            message.From = new MailAddress(senderAddress, "Fonitor");

            message.Subject = "Fonitor Notification";

            message.Html = string.Format("<p>Hello,<br /><br /><b>{0}</b> has detected a problem! Please review the attached image and take appropriate action.</p>", sensorName);

            if (changedImageStream != null)
            {
				changedImageStream.Seek(0, SeekOrigin.Begin);

                message.AddAttachment(changedImageStream, "Picture.jpg");
            }

            var credentials = new NetworkCredential(sendGridUserName, sendGridPassword);

            var transportWeb = new Web(credentials);

            transportWeb.Deliver(message);
        }
    }
}
