using System;
using System.Configuration;

namespace Dribbly.Email.Models
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public EmailConfiguration()
        {
            From = ConfigurationManager.AppSettings["EMAIL_FROM"];
            SmtpServer = ConfigurationManager.AppSettings["EMAIL_SMTPSERVER"];
            Port = Convert.ToInt16(ConfigurationManager.AppSettings["EMAIL_PORT"]);
            UserName = ConfigurationManager.AppSettings["EMAIL_USERNAME"];
            Password = ConfigurationManager.AppSettings["EMAIL_PASSWORD"];
        }
    }
}
