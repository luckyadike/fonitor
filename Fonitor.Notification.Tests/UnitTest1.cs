namespace Fonitor.Notification.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           Email.SendImageChangeNotification("luckyadike@gmail.com", "Indoor Gym", null);
        }
    }
}
