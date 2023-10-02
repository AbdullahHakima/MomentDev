namespace Bloggi.Contracts
{
	public interface IEmailService
	{
		public string GetEmailContent(string emailTempalte);
		public Task SendEmail(string toEmail ,string subject, string body);
	}
}
