using Functions.Core.Interfaces;
using Functions.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;

namespace Functions.Infra.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _log;
        private readonly SendGridClient _client;

        public EmailService(ILogger<EmailService> log)
        {
            _log = log;
            _client = new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API_KEY"));
        }

        public async void SendAsync(Email email)
        {
            _log.LogInformation($"Send email {JsonConvert.SerializeObject(email)}");
            EmailAddress from = new EmailAddress(email.FromAddress, email.FromName);
            EmailAddress to = new EmailAddress(email.ToAddress, email.ToName);
            SendGridMessage msg;
            if (email.Tos == null)
                msg = MailHelper.CreateSingleEmail(from, to, email.Subject, email.PlainTextContent, email.HtmlContent);
            else
            {
                var tos = new List<EmailAddress>();
                foreach (var tto in email.Tos)
                {
                    tos.Add(new EmailAddress(tto));
                }
                msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, email.Subject, email.PlainTextContent, email.HtmlContent);
            }
            Response response = await _client.SendEmailAsync(msg);
            _log.LogInformation("Email sent " + JsonConvert.SerializeObject(response));
        }
    }
}
