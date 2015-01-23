using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.IO;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Reporting.WebForms;
using SamplePB.DAL;
using SamplePB.Models;

namespace SamplePB.Controllers
{
    public class ContactsController : Controller
    {
        //
        // GET: /Contacts/
        //Show contact details
        private IEnumerable<SelectListItem> GetContactType()
        {

            var contactTypes = new List<SelectListItem>();

            var mobile = new SelectListItem
            {
                Text = "Mobile",
                Value = "Mobile",

            };

            contactTypes.Add(mobile);
            var tel = new SelectListItem
            {
                Text = "Telephone",
                Value = "Telephone"
            };
            contactTypes.Add(tel);

            var home = new SelectListItem
            {
                Text = "Home",
                Value = "Home"
            };
            contactTypes.Add(home);

            var work = new SelectListItem
            {
                Text = "Work",
                Value = "Work"
            };
            contactTypes.Add(work);

            return (contactTypes);
        }

        public ActionResult ShowContactDetails(int id)
        {
            var objDb = new DatabaseOperations();

            var ds = objDb.SelectById(id);
            var pModel = new PersonViewModel();

            pModel.PersonId = id;
            pModel.LastName = ds.Tables[0].Rows[0]["LastName"].ToString();
            pModel.FirstName = ds.Tables[0].Rows[0]["FirstName"].ToString();
            pModel.MiddleName = ds.Tables[0].Rows[0]["MiddleName"].ToString();
            pModel.BirthDate = ds.Tables[0].Rows[0]["BirthDate"].ToString();
            pModel.HomeAddress = ds.Tables[0].Rows[0]["HomeAddress"].ToString();
            pModel.Company = ds.Tables[0].Rows[0]["Company"].ToString();
            pModel.ActualImage = (byte[]) ds.Tables[0].Rows[0]["ProfilePic"];
            pModel.ContentType = ds.Tables[0].Rows[0]["ContentType"].ToString();
            

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                pModel.ContactNumbersViewModels.Add(
                    new ContactNumbersViewModel
                    {
                        ContactId = Convert.ToInt32(row["ID"]),
                        PersonId = Convert.ToInt32(row["PersonID"]),
                        SelectedContactType = row["SelectedContactType"].ToString(),
                        ContactNumber = row["ContactNumber"].ToString()
                    });
            }
            foreach (DataRow row in ds.Tables[2].Rows)
            {
                pModel.EmailsViewModels.Add(
                    new EmailsViewModel
                    {
                        EmailId = Convert.ToInt32(row["ID"]),
                        PersonId = Convert.ToInt32(row["PersonID"]),
                        Emails = row["EmailAddress"].ToString()
                    });
            }

            return View(pModel);
        }

        public ActionResult ShowAllContacts(PersonViewModel model)
        {
            
            var obj = new DatabaseOperations();
            model.StoreAllData = obj.SelectAllContacts(model);
            return View(model);
        }
        #region Add,Edit,Delete Person
        public ActionResult InsertContactPerson()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertContactPerson(PersonViewModel model)
        {
            if (ModelState.IsValid){


                
                HttpPostedFileBase file = Request.Files["OriginalLocation"];
                
                model.ContentType = file.ContentType;

                Int32 length = file.ContentLength;

                byte[] tempImage = new byte[length];
                file.InputStream.Read(tempImage, 0, length);
                model.ActualImage = tempImage;

                var obj = new DatabaseOperations();
                obj.InsertContactPerson(model);
                



                var mail = new MailMessage();
                mail.To.Add("ichaosblade@gmail.com");
                mail.From = new MailAddress("johnralphdaz@gmail.com");
                mail.Subject = "Contacts";
                string Body = model.LastName + ", " + model.FirstName + " " + model.MiddleName + " has been added as contact.";
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
             
                ModelState.Clear();

                var ds = obj.SelectLastInsertPerson(model);
                model.PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"].ToString());
                return RedirectToAction("ShowContactDetails", "Contacts", new { id = model.PersonId });
            }

            return View();
        }


        
        public ActionResult EditContact(int id)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.SelectById(id);
            var model = new PersonViewModel
            {
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"].ToString()),
                LastName = ds.Tables[0].Rows[0]["LastName"].ToString(),
                FirstName = ds.Tables[0].Rows[0]["FirstName"].ToString(),
                MiddleName = ds.Tables[0].Rows[0]["MiddleName"].ToString(),
                BirthDate = ds.Tables[0].Rows[0]["BirthDate"].ToString(),
                HomeAddress = ds.Tables[0].Rows[0]["HomeAddress"].ToString(),
                Company = ds.Tables[0].Rows[0]["Company"].ToString()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditContact(PersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var obj = new DatabaseOperations();
                obj.UpdateContactPerson(model);
                return RedirectToAction("ShowContactDetails", "Contacts",new{id=model.PersonId});
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteContact(int id)
        {
            var objDB = new DatabaseOperations();
            var ds = objDB.SelectById(id);
            var model = new PersonViewModel
            {
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"].ToString()),
                LastName = ds.Tables[0].Rows[0]["LastName"].ToString(),
                FirstName = ds.Tables[0].Rows[0]["FirstName"].ToString(),
                MiddleName = ds.Tables[0].Rows[0]["MiddleName"].ToString(),
                BirthDate = ds.Tables[0].Rows[0]["BirthDate"].ToString(),
                HomeAddress = ds.Tables[0].Rows[0]["HomeAddress"].ToString(),
                Company = ds.Tables[0].Rows[0]["Company"].ToString()
            };

            return View(model);
        }

        public ActionResult DeleteContact(PersonViewModel model)
        {
            var obj = new DatabaseOperations();
            obj.DeleteContact(model.PersonId);
            return RedirectToAction("ShowAllContacts", "Contacts");
        }
        #endregion
          
        #region Contact Actions
        [HttpGet]
        public ActionResult InsertPersonEmail(int id)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.SelectById(id);
            var model = new EmailsViewModel();
            model.PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"].ToString());
            return View(model);
        }

        public ActionResult InsertPersonEmail(EmailsViewModel eModel)
        {
            if (ModelState.IsValid)
            {
                var obj = new DatabaseOperations();
                obj.AddEmails(eModel);
                ModelState.Clear();
                return RedirectToAction("ShowContactDetails", "Contacts", new { id = eModel.PersonId });
            }
            else
            {
                return View(eModel);
            }
        }

         [HttpGet]
        public ActionResult InsertPersonContactNumber(int id)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.SelectById(id);
            var model = new ContactNumbersViewModel
            {
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"].ToString())
            };
            model.ContactType = GetContactType();

            return View(model);
        }

        public ActionResult InsertPersonContactNumber(ContactNumbersViewModel cModel)
        {
            if (ModelState.IsValid)
            {
                var obj = new DatabaseOperations();
                obj.AddContactNumbers(cModel);
                ModelState.Clear();

                return RedirectToAction("ShowContactDetails", "Contacts", new { id = cModel.PersonId });

            }
            else
            {
                TempData["AlertMessage"] = "Please insert a valid contact number!";
                return RedirectToAction("InsertPersonContactNumber", "Contacts", new {id = cModel.PersonId});
            }
        }

        [HttpGet]
        public ActionResult EditContactNumber(int id)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.SelectByContactId(id);
            var model = new ContactNumbersViewModel
            {
                ContactId = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString()),
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"].ToString()),
                SelectedContactType = ds.Tables[0].Rows[0]["SelectedContactType"].ToString(),
                ContactNumber = ds.Tables[0].Rows[0]["ContactNumber"].ToString()
            };

            model.ContactType = GetContactType();
            return View(model);
        }

        [HttpPost]
        public ActionResult EditContactNumber(ContactNumbersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var obj = new DatabaseOperations();
                obj.UpdateContactNumber(model);
                return RedirectToAction("ShowContactDetails", "Contacts", new {id = model.PersonId});
            }
            else
            {
                
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult DeleteContactNumber(int id)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.SelectByContactId(id);
            var model = new ContactNumbersViewModel
            {
                ContactId = id,
                SelectedContactType = ds.Tables[0].Rows[0]["SelectedContactType"].ToString(),
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"].ToString()),
                ContactNumber = ds.Tables[0].Rows[0]["ContactNumber"].ToString()
            };

            return View(model);
        }

        public ActionResult DeleteContactNumber(ContactNumbersViewModel model)
        {
            var obj = new DatabaseOperations();

            obj.DeleteContactNumber(model.ContactId);
            return RedirectToAction("ShowContactDetails", "Contacts",new {id = model.PersonId});
        }
        #endregion

        #region Email Related Actions
        public ActionResult EditEmail(int id)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.SelectByEmailId(id);
            var model = new EmailsViewModel();

            model.EmailId = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString());
            model.PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"].ToString());
            model.Emails = ds.Tables[0].Rows[0]["EmailAddress"].ToString();
            return View(model);
        }

        [HttpPost]
        public ActionResult EditEmail(EmailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var obj = new DatabaseOperations();

                obj.UpdateEmail(model);
                return RedirectToAction("ShowContactDetails", "Contacts", new { id = model.PersonId });
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult DeleteEmail(int id)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.SelectByEmailId(id);
            var model = new EmailsViewModel
            {
                EmailId = id,
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"].ToString()),
                Emails = ds.Tables[0].Rows[0]["EmailAddress"].ToString()
            };

            return View(model);
        }

        public ActionResult DeleteEmail(EmailsViewModel model)
        {
            var obj = new DatabaseOperations();
            obj.DeleteEmail(model);
            return RedirectToAction("ShowContactDetails", "Contacts", new { id = model.PersonId });
        } 
        #endregion


        [HttpGet]
        public ActionResult ChangeProfilePicture(int id, PersonViewModel model)
        {
            model.PersonId = id;
            var objDb = new DatabaseOperations();

            var ds = objDb.GetPicture(id);

            model.ActualImage = (byte[])ds.Tables[0].Rows[0]["ProfilePic"];
            model.ContentType = ds.Tables[0].Rows[0]["ContentType"].ToString();
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeProfilePicture(PersonViewModel model)
        {


           
            
            HttpPostedFileBase file = Request.Files["OriginalLocation"];

            model.ContentType = file.ContentType;
            Int32 length = file.ContentLength;
            byte[] tempImage = new byte[length];
            file.InputStream.Read(tempImage, 0, length);
            model.ActualImage = tempImage;
            if (file.FileName != "")
            {
            var obj = new DatabaseOperations();
            obj.ChangeProfilePicture(model);
            return RedirectToAction("ShowContactDetails", "Contacts", new {id = model.PersonId});
            }
            else
            {
                return RedirectToAction("ShowContactDetails", "Contacts", new {id = model.PersonId});
            }
    }

        public ActionResult Report(string format)
        {
            LocalReport localReport = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "ReportContactsList.rdlc");
            if (System.IO.File.Exists(path))
            {
                localReport.ReportPath = path;
            }
            else
            {
                return View("ShowAllContacts");
            }
            //var model = new PersonViewModel();
            var objDb = new DatabaseOperations();
            var ds = objDb.GetDataTablesForReportContactList();

            /*var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ContactDbContext"].ToString());
            con.Open();
            var ds = new DataSet();
            var da = new SqlDataAdapter("SELECT LastName,FirstName,MiddleName FROM tblPerson ORDER BY Lastname ASC", con);
            da.Fill(ds);*/

            var reportDataSource = new ReportDataSource("DataSet1", ds.Tables[0]);
            //con.Close();




            //----for printing reports.
            localReport.DataSources.Add(reportDataSource);
            string reportType = format;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + format + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Microsoft.Reporting.WebForms.Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            return File(renderedBytes, mimeType);
        }

        public ActionResult ReportPersonDetail(int id, string format)
        {
            var model = new PersonViewModel();
            LocalReport localReport = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "ReportContactDetails.rdlc");
            if (System.IO.File.Exists(path))
            {
                localReport.ReportPath = path;
            }
            else
            {
                return View("ShowContactDetails");
            }


            var objDb = new DatabaseOperations();
            var ds = objDb.GetDataTablesForReportPersonalDetail(id);

            var reportDataSource = new ReportDataSource("DataSetPersonDetails", ds.Tables[0]);
            var reportDataSource2 = new ReportDataSource("DataSetPersonDetails2", ds.Tables[2]);
            var reportDataSource3 = new ReportDataSource("DataSetPersonDetails3", ds.Tables[1]);

            //----for printing reports.
            localReport.DataSources.Add(reportDataSource);
            localReport.DataSources.Add(reportDataSource2);
            localReport.DataSources.Add(reportDataSource3);

            string reportType = format;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + format + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Microsoft.Reporting.WebForms.Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            return File(renderedBytes, mimeType);
        }

        public ActionResult ShowPhoto(int id)
        {
            var objDb = new DatabaseOperations();

            var ds = objDb.GetPicture(id);
            var pModel = new PersonViewModel();

            pModel.ActualImage = (byte[])ds.Tables[0].Rows[0]["ProfilePic"];
            pModel.ContentType = ds.Tables[0].Rows[0]["ContentType"].ToString();
            return File(pModel.ActualImage, pModel.ContentType);

        }

    }
}