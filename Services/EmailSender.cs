using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using course_api.Interface;

namespace course_api.Services {
	public class EmailSender : IEmailSender {
		public void Send(MailMessage message) {
			var client = new SmtpClient();
			client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
			client.PickupDirectoryLocation = @"C:\Users\ansel\Desktop";

			message.From = new MailAddress("Admin@course_api.com");

			client.Send(message);
		}
	}
}