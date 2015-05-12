namespace Fonitor.Notification
{
    using System.Configuration;
    using System.IO;
    using Twilio;

    public class Phone
    {
        public static void SendImageChangeNotification(string recipientPhoneNumber,
                                                       string sensorName,
                                                       MemoryStream changedImageStream)
        {
            // Retrieve settings.
            // Ensure that they are set?
            var sid = ConfigurationManager.AppSettings["TwilioAccountSID"];

            var token = ConfigurationManager.AppSettings["TwilioAuthToken"];

            var senderPhone = ConfigurationManager.AppSettings["PhoneNumber"];

            var client = new TwilioRestClient(sid, token);

            var message = string.Format("{0} has detected a problem! Please review the attached image and take appropriate action.", sensorName);

            var result = client.SendMessage(
                senderPhone,
                recipientPhoneNumber,
                message);

            if (result.RestException != null)
            {
                // an exception occurred making the REST call
                // Log this message.
                string error = result.RestException.Message;
            }
        }
    }
}
