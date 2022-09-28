using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StarboardNFT.Utilities
{
    public class EmailUtility
    {
		public static async void SendEmail(string ToAddress, string Subject, string Body, string EmailTemp, bool IsHTML = false, string AlertTitle = "", string AlertURL = "")
		{
			try
			{
				if (IsHTML == true)
				{
					if (EmailTemp == "action")
					{
						string path = Path.Combine(Environment.CurrentDirectory, @"EmailTemplates\" + EmailTemp + ".html");
						string bodyTemplate = File.ReadAllText(path);
						bodyTemplate = bodyTemplate.Replace("{ConfirmEmailLink}", Body);
						Body = bodyTemplate;
					}
					if (EmailTemp == "alert")
					{
						string path = Path.Combine(Environment.CurrentDirectory, @"EmailTemplates\" + EmailTemp + ".html");
						string bodyTemplate = File.ReadAllText(path);
						bodyTemplate = bodyTemplate.Replace("{AlertTitle}", AlertTitle);
						bodyTemplate = bodyTemplate.Replace("{AlertBody}", Body);
						bodyTemplate = bodyTemplate.Replace("{AlertURL}", AlertURL);
						Body = bodyTemplate;
					}
					if (EmailTemp == "login")
					{
						//do nothing. was done in coode
					}
				}


				var fromAddress = new MailAddress("no-reply@starboard.org", "Starboard");
				var toAddress = new MailAddress(ToAddress);

				//Email is using amazons ses. We pay per email. Cost is relatively low, but eventually once we build email queue we will want our own maybe.
				var smtp = new SmtpClient
				{
					Host = "email-smtp.us-west-2.amazonaws.com",
					Port = 587,
					EnableSsl = true,
					Credentials = new NetworkCredential("AKIAX4ZMGKMUG3EI5YRB", "BIUXlnkW1oHiD7CVZionf+VPfXWENXJMtOlEhKejQL4W"),
					DeliveryMethod = SmtpDeliveryMethod.Network
				};
				using (var message = new MailMessage(fromAddress, toAddress)
				{
					IsBodyHtml = true,
					Subject = Subject,
					Body = Body,
				})
				{
					await smtp.SendMailAsync(message);
					//return Task.CompletedTask;
				}
			}
			catch (Exception ex)
			{
				using (EventLog eventLog = new EventLog("Application"))
				{
					eventLog.Source = "Application";
					eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Information, 101, 1);
				}
				ex.ToString();
				//return Task.CompletedTask;
			}
		}

		public static string AddStandardEmailTemplate(string body)
		{
			string emailTemplate = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "EmailTemplates/StandardEmailTemplate.html"));
			emailTemplate = emailTemplate.Replace("[@body]", body);
			return emailTemplate;
		}
	}
}
