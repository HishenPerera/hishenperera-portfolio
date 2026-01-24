using Microsoft.AspNetCore.Mvc;
using hishenperera_portfolio.Services;
using hishenperera_portfolio.Models;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace hishenperera_portfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProjectService _projectService;

        public HomeController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        public IActionResult Index()
        {
            var model = new HomePageViewModel
            {
                Projects = _projectService.GetAll()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Contact(HomePageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Projects = _projectService.GetAll();
                return View("Index", model);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                "Portfolio Contact",
                Environment.GetEnvironmentVariable("SMTP_EMAIL")
            ));

            message.To.Add(new MailboxAddress("Hishen", "hishenportofolio@gmail.com"));
            message.Subject = "New Contact Form Message";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>New Contact Message</h2>
                    <p><strong>Name:</strong> {model.Contact.Name}</p>
                    <p><strong>Email:</strong> {model.Contact.Email}</p>
                    <p><strong>Message:</strong><br/>{model.Contact.Message}</p>
                "
            };

            try
            {
                using var client = new SmtpClient();

                client.Connect(
                    Environment.GetEnvironmentVariable("SMTP_HOST"),
                    int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")),
                    SecureSocketOptions.StartTls
                );

                client.Authenticate(
                    Environment.GetEnvironmentVariable("SMTP_EMAIL"),
                    Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                );

                client.Send(message);
                client.Disconnect(true);

                TempData["Success"] = "Message sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Email failed: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
