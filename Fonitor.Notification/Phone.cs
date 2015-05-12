namespace Fonitor.Notification
{
    using System;
    using System.Configuration;
    using System.IO;
    using Twilio;

    public class Phone
    {
        static string SaveStreamToTempImage(MemoryStream imgStream)
        {
            try
            {
                var tempFile = Path.GetTempFileName();
                using (var fs = File.OpenWrite(tempFile))
                {
                    imgStream.CopyTo(fs);
                }

                return tempFile;
            }
            catch (Exception)
            {
                // Log the exception message?
                return string.Empty;
            }
        }

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

            var imgUrl = SaveStreamToTempImage(changedImageStream);

            var result = client.SendMessage(
                senderPhone,
                recipientPhoneNumber,
                message /*, new string[] { imgUrl }*/ );

            if (result.RestException != null)
            {
                // an exception occurred making the REST call
                // Log this message.
                string error = result.RestException.Message;
            }
        }
    }
}
