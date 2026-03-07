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
        private readonly IConfiguration _config;

        public HomeController(ProjectService projectService, IConfiguration config)
        {
            _projectService = projectService;
            _config = config;
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
        public async Task<IActionResult> Contact(HomePageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Projects = _projectService.GetAll();
                return View("Index", model);
            }

            var smtpHost     = _config["Smtp:Host"];
            var smtpPort     = int.Parse(_config["Smtp:Port"] ?? "587");
            var smtpEmail    = _config["Smtp:Email"];
            var smtpPassword = _config["Smtp:Password"];

            if (string.IsNullOrWhiteSpace(smtpEmail) || string.IsNullOrWhiteSpace(smtpPassword))
            {
                TempData["Error"] = "Email service is not configured. Please contact me directly at hishenportofolio@gmail.com";
                return RedirectToAction("Index");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Portfolio Contact", smtpEmail));
            message.To.Add(new MailboxAddress("Hishen", "hishenperera@gmail.com"));
            message.Subject = $"📬 New Portfolio Enquiry from {model.Contact.Name}";

            message.Body = new TextPart("html")
            {
                Text = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8' />
  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
</head>
<body style='margin:0;padding:0;background:#f1f5f9;font-family:Montserrat,Segoe UI,Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f1f5f9;padding:40px 0;'>
    <tr>
      <td align='center'>
        <table width='580' cellpadding='0' cellspacing='0' style='max-width:580px;width:100%;'>

          <!-- HEADER -->
          <tr>
            <td style='background:linear-gradient(135deg,#ff8800,#ffb347);border-radius:16px 16px 0 0;padding:36px 40px;text-align:center;'>
              <h1 style='margin:0;color:#ffffff;font-size:26px;font-weight:800;letter-spacing:2px;text-transform:uppercase;'>
                HISHEN PERERA
              </h1>
              <p style='margin:6px 0 0;color:rgba(255,255,255,0.85);font-size:13px;letter-spacing:1px;text-transform:uppercase;'>
                Portfolio · New Enquiry
              </p>
            </td>
          </tr>

          <!-- BODY CARD -->
          <tr>
            <td style='background:#ffffff;padding:36px 40px;border-left:1px solid #e5e7eb;border-right:1px solid #e5e7eb;'>

              <p style='margin:0 0 24px;font-size:15px;color:#374151;line-height:1.6;'>
                You have received a new message through your portfolio contact form. Details are below:
              </p>

              <!-- INFO TABLE -->
              <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse:collapse;margin-bottom:28px;'>
                <tr>
                  <td style='padding:14px 16px;background:#f8fafc;border:1px solid #e5e7eb;border-radius:8px 8px 0 0;width:30%;'>
                    <span style='font-size:11px;font-weight:700;letter-spacing:1px;text-transform:uppercase;color:#ff8800;'>Full Name</span>
                  </td>
                  <td style='padding:14px 16px;background:#f8fafc;border:1px solid #e5e7eb;border-top:1px solid #e5e7eb;border-left:none;border-radius:0 8px 0 0;'>
                    <span style='font-size:15px;font-weight:600;color:#0f172a;'>{model.Contact.Name}</span>
                  </td>
                </tr>
                <tr>
                  <td style='padding:14px 16px;background:#ffffff;border:1px solid #e5e7eb;border-top:none;'>
                    <span style='font-size:11px;font-weight:700;letter-spacing:1px;text-transform:uppercase;color:#ff8800;'>Contact No.</span>
                  </td>
                  <td style='padding:14px 16px;background:#ffffff;border:1px solid #e5e7eb;border-top:none;border-left:none;'>
                    <span style='font-size:15px;font-weight:600;color:#0f172a;'>{model.Contact.Phone}</span>
                  </td>
                </tr>
              </table>

              <!-- MESSAGE BOX -->
              <div style='margin-bottom:8px;'>
                <span style='font-size:11px;font-weight:700;letter-spacing:1px;text-transform:uppercase;color:#ff8800;'>Message</span>
              </div>
              <div style='background:#f8fafc;border:1px solid #e5e7eb;border-radius:10px;padding:20px 24px;font-size:15px;color:#374151;line-height:1.75;white-space:pre-wrap;'>
{model.Contact.Message}
              </div>

            </td>
          </tr>

          <!-- FOOTER -->
          <tr>
            <td style='background:#0b1220;border-radius:0 0 16px 16px;padding:24px 40px;text-align:center;'>
              <p style='margin:0;font-size:12px;color:#64748b;letter-spacing:0.5px;'>
                This message was sent via the contact form on
                <a href='https://hishenperera.site' style='color:#ff8800;text-decoration:none;font-weight:600;'>hishenperera.com</a>
              </p>
              <p style='margin:8px 0 0;font-size:11px;color:#475569;'>
                © {DateTime.Now.Year} Hishen Perera · All rights reserved
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>"
            };


            try
            {
                // 10-second timeout — prevents browser hanging if Railway blocks SMTP
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                using var client = new SmtpClient();
                client.Timeout = 10000; // ms

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cts.Token);
                await client.AuthenticateAsync(smtpEmail, smtpPassword, cts.Token);
                await client.SendAsync(message, cts.Token);
                await client.DisconnectAsync(true, cts.Token);

                TempData["Success"] = "Message sent! I'll get back to you soon 🎉";
            }
            catch (OperationCanceledException)
            {
                TempData["Error"] = "Connection timed out. Please try WhatsApp or email me directly.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Couldn't send the message: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
