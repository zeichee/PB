using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace SamplePB.Controllers
{
    public class SendMailerController : Controller
    {
        //
        // GET: /SendMailer/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Index(SamplePB.Models.MailModel _objModelMail)
        {
            if (ModelState.IsValid)
            {
                //hack for auto accepting certificate
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

                var mail = new MailMessage();
                mail.To.Add(_objModelMail.To);
                mail.From = new MailAddress(_objModelMail.From);
                mail.Subject = _objModelMail.Subject;
                string Body = _objModelMail.Body;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential
                        ("johnralphdaz@gmail.com", "johnralph"),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Timeout = (3 * 60) * 1000
                };
                smtp.Send(mail);

                return View("Index", _objModelMail);
            }
            else
            {
                return View();
            }
        }
    }
}