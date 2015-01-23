using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using SamplePB.Models;

namespace SamplePB.Controllers
{
    public class SendMailerController : Controller
    {
        // GET: /SendMailer/ 
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void SendEmail(MailModel model)
        {
            MailMessage mail = new MailMessage("johnralphdaz@gmail.com", model.To, model.Subject, model.Body);
            //mail.From = new MailAddress("xxx@gmail.com", "nameEmail");
            mail.IsBodyHtml = true; // necessary if you're using html email

            NetworkCredential credential = new NetworkCredential("johnralphdaz@gmail.com", "johnralph");
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = credential;
            smtp.Send(mail);
        }
    }
}