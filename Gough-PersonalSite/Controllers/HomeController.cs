using Gough_PersonalSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


using MimeKit; //Added for access to the MimeMessage class
using MailKit.Net.Smtp; //Added for access to the SmtpClient class

namespace Gough_PersonalSite.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Classmates()
        {
            return View();
        }

        public IActionResult Resume()
        {
            return View();
        }

        public IActionResult Portfolio()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            if (!ModelState.IsValid)
            {
                return View(cvm);
            }

            string contactMessage = $"You have a new email from your personal site!<br/>" +
                $"Sender: {cvm.Name}<br/>Email: {cvm.Email}<br/>Subject: {cvm.Subject}<br/>" +
                $"Message: {cvm.Message}";

            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));
            mimeMessage.To.Add(new MailboxAddress("PersonalEmail", _config.GetValue<string>("Credentials:Email:Recipient")));
            mimeMessage.Subject = cvm.Subject;
            mimeMessage.Body = new TextPart("HTML")
            {
                Text = contactMessage
            };
            mimeMessage.Priority = MessagePriority.Urgent;
            mimeMessage.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            using (var client = new SmtpClient())
            {
                client.Connect(_config.GetValue<string>("Credentials:Email:Client"));
                client.Authenticate(
                    _config.GetValue<string>("Credentials:Email:User"),
                    _config.GetValue<string>("Credentials:Email:Password"));

                try
                {
                    client.Send(mimeMessage);
                }
                catch (Exception ex)
                {

                    ViewBag.ErrorMessage = $"There was an error processing your request. Please try again later<br/> Error Message: {ex.StackTrace}";

                    return View(cvm);
                }
            }
                return View("EmailConfirmation", cvm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}