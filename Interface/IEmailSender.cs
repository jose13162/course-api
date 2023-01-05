using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace course_api.Interface {
	public interface IEmailSender {
		void Send(MailMessage message);
	}
}