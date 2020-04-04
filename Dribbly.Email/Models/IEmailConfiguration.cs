namespace Dribbly.Email.Models
{
    public interface IEmailConfiguration
    {
        string From { get; set; }
        string SmtpServer { get; set; }
        int Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
    }
}
