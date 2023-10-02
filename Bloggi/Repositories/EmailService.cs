using Bloggi.Contracts;
using Bloggi.Helpers;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http.HttpResults;
using MimeKit;

namespace Bloggi.Repositories
{
	public class EmailService : IEmailService
	{
		private EmailConfiguration _emailConfiguration;
		private IWebHostEnvironment _webHostEnvironment;
		public EmailService(EmailConfiguration emailConfiguration,IWebHostEnvironment webHostEnvironment)
		{
			_emailConfiguration = emailConfiguration;
			_webHostEnvironment = webHostEnvironment;
		}

		public string GetEmailContent(string emailTempalte)
		{
			var templatePath = Path.Combine(_webHostEnvironment.ContentRootPath,"Views","Emails", $"{emailTempalte}.cshtml");
			if (!File.Exists(templatePath))
				throw new FileNotFoundException($"{emailTempalte} not founded!");
			return File.ReadAllText(templatePath);
		}

		public async Task SendEmail(string toEmail, string subject, string body)
		{
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress("Abdullah hakim", _emailConfiguration.SmtpUsername));
			emailMessage.To.Add(new MailboxAddress("", toEmail));
			emailMessage.Subject=subject;
			var bodyBuilder = new BodyBuilder()
			{
				HtmlBody = body
			};
			emailMessage.Body = bodyBuilder.ToMessageBody();
			using var client = new SmtpClient();
			await client.ConnectAsync(_emailConfiguration.SmtpServer,_emailConfiguration.SmtpPort,true);
			 client.AuthenticationMechanisms.Remove("XOAUTH2");
			await client.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
			await client.SendAsync(emailMessage);
			await client.DisconnectAsync(true);

		}
	}
}
