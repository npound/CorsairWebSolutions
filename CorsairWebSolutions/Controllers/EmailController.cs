using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace CorsairWebSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SendEmail(ContectEmail email)
        {

            var apiKey = "";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(email.email, email.name);
            var subject = "Contact Request";
            var to = new EmailAddress("corsairwebsolutions@gmail.com", "CWS");
            var plainTextContent = $"{email.name}\n{email.email}\n\n{email.message}";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, plainTextContent);
            var response = await client.SendEmailAsync(msg);
            return Accepted();

        }


    }

    public class ContectEmail
    {
        public string email { get; set; }
        public string name { get; set; }
        public string message { get; set; }
    }
}
