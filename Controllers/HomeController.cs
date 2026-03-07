using Microsoft.AspNetCore.Mvc;
using hishenperera_portfolio.Services;
using hishenperera_portfolio.Models;
using System.Text;
using System.Text.Json;

namespace hishenperera_portfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProjectService _projectService;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ProjectService projectService, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _projectService = projectService;
            _config = config;
            _httpClientFactory = httpClientFactory;
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

            var apiKey = _config["Resend:ApiKey"]
                      ?? Environment.GetEnvironmentVariable("RESEND_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                TempData["Error"] = "Email service is not configured. Please reach me on WhatsApp.";
                return RedirectToAction("Index");
            }

            // Build the professional HTML email body
            var htmlBody = $@"
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

          <!-- BODY -->
          <tr>
            <td style='background:#ffffff;padding:36px 40px;border-left:1px solid #e5e7eb;border-right:1px solid #e5e7eb;'>
              <p style='margin:0 0 24px;font-size:15px;color:#374151;line-height:1.6;'>
                You have received a new message through your portfolio contact form.
              </p>

              <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse:collapse;margin-bottom:28px;'>
                <tr>
                  <td style='padding:14px 16px;background:#f8fafc;border:1px solid #e5e7eb;width:30%;'>
                    <span style='font-size:11px;font-weight:700;letter-spacing:1px;text-transform:uppercase;color:#ff8800;'>Full Name</span>
                  </td>
                  <td style='padding:14px 16px;background:#f8fafc;border:1px solid #e5e7eb;border-left:none;'>
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
              <p style='margin:0;font-size:12px;color:#64748b;'>
                Sent via the contact form on
                <a href='https://hishenperera.site' style='color:#ff8800;text-decoration:none;font-weight:600;'>hishenperera.site</a>
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
</html>";

            // Call Resend API via plain HTTP POST (works on Railway — no SMTP port needed)
            var payload = new
            {
                from = "Portfolio <onboarding@resend.dev>",
                to   = new[] { "hishenperera@gmail.com" },
                subject = $"📬 New Portfolio Enquiry from {model.Contact.Name}",
                html = htmlBody
            };

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                var client   = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var json     = JsonSerializer.Serialize(payload);
                var content  = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.resend.com/emails", content, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Message sent! I'll get back to you soon 🎉";
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Email failed ({(int)response.StatusCode}). Please try WhatsApp.";
                }
            }
            catch (OperationCanceledException)
            {
                TempData["Error"] = "Request timed out. Please try WhatsApp instead.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Couldn't send the message: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
