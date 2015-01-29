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

            pModel.LastName = Convert.ToString(ds.Tables[0].Rows[0]["LastName"]);

            pModel.FirstName = Convert.ToString(ds.Tables[0].Rows[0]["FirstName"]);

            pModel.MiddleName = Convert.ToString(ds.Tables[0].Rows[0]["MiddleName"]);

            pModel.BirthDate = Convert.ToString(ds.Tables[0].Rows[0]["BirthDate"]);

            pModel.HomeAddress = Convert.ToString(ds.Tables[0].Rows[0]["HomeAddress"]);

            pModel.Company = Convert.ToString(ds.Tables[0].Rows[0]["Company"]);

            pModel.ActualImage = (byte[]) ds.Tables[0].Rows[0]["ProfilePic"];

            pModel.ContentType = Convert.ToString(ds.Tables[0].Rows[0]["ContentType"]);
            

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                pModel.ContactNumbersViewModels.Add(
                    new ContactNumbersViewModel
                    {
                        ContactId = Convert.ToInt32(row["ID"]),

                        PersonId = Convert.ToInt32(row["PersonID"]),

                        SelectedContactType = Convert.ToString(row["SelectedContactType"]),

                        ContactNumber = Convert.ToString(row["ContactNumber"])
                    });
            }


            foreach (DataRow row in ds.Tables[2].Rows)
            {
                pModel.EmailsViewModels.Add(
                    new EmailsViewModel
                    {
                        EmailId = Convert.ToInt32(row["ID"]),

                        PersonId = Convert.ToInt32(row["PersonID"]),

                        Emails = Convert.ToString(row["EmailAddress"])
                    });
            }

            return View(pModel);
        }

        public ActionResult ShowAllContacts(PersonViewModel model)
        {
            
            var obj = new DatabaseOperations();
            model.StoreAllData = obj.SelectAllContacts(model);
            TempData["AlertMessage"] = model.Result;
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
                try
                {
                    smtp.Send(mail);
                }
                catch
                {
                    TempData["AlertMessage"] = "Error while saving contacts. An internet connection is required.";
                    return View();
                }
                
                var obj = new DatabaseOperations();
                obj.InsertContactPerson(model);

                    ModelState.Clear();
                    var objs = new DatabaseOperations();
                    var ds = objs.SelectLastInsertPerson(model);
                    model.PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"]);
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
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"]),

                LastName =Convert.ToString( ds.Tables[0].Rows[0]["LastName"]),
                
                FirstName = Convert.ToString(ds.Tables[0].Rows[0]["FirstName"]),
                
                MiddleName = Convert.ToString(ds.Tables[0].Rows[0]["MiddleName"]),
                
                BirthDate = Convert.ToString(ds.Tables[0].Rows[0]["BirthDate"]),
                
                HomeAddress = Convert.ToString(ds.Tables[0].Rows[0]["HomeAddress"]),
                
                Company = Convert.ToString(ds.Tables[0].Rows[0]["Company"])
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
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"]),
                
                LastName = Convert.ToString(ds.Tables[0].Rows[0]["LastName"]),
                
                FirstName = Convert.ToString(ds.Tables[0].Rows[0]["FirstName"]),
                
                MiddleName = Convert.ToString(ds.Tables[0].Rows[0]["MiddleName"]),
                
                BirthDate = Convert.ToString( ds.Tables[0].Rows[0]["BirthDate"]),
                
                HomeAddress = Convert.ToString( ds.Tables[0].Rows[0]["HomeAddress"]),
                
                Company = Convert.ToString(ds.Tables[0].Rows[0]["Company"])
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

            model.PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"]);
            
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
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"])
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
                ContactId = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"]),
                
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"]),
                
                SelectedContactType = Convert.ToString(ds.Tables[0].Rows[0]["SelectedContactType"]),
                
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
                TempData["AlertMessage"] = "Please insert a valid contact number!";
                
                return RedirectToAction("EditContactNumber", "Contacts", new { id = model.ContactId });
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
                
                SelectedContactType = Convert.ToString(ds.Tables[0].Rows[0]["SelectedContactType"]),
                
                PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"]),
                
                ContactNumber = Convert.ToString(ds.Tables[0].Rows[0]["ContactNumber"])
            };

            return View(model);
        }

        public ActionResult DeleteContactNumber(ContactNumbersViewModel model)
        {
                var obj = new DatabaseOperations();
                obj.DeleteContactNumber(model.ContactId);
                
                return RedirectToAction("ShowContactDetails", "Contacts", new { id = model.PersonId });
        }
        #endregion

        #region Email Related Actions
        public ActionResult EditEmail(int id)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.SelectByEmailId(id);
            var model = new EmailsViewModel();

            model.EmailId = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"]);
            
            model.PersonId = Convert.ToInt32(ds.Tables[0].Rows[0]["PersonID"]);

            model.Emails = Convert.ToString(ds.Tables[0].Rows[0]["EmailAddress"]);
            
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
            var objDb = new DatabaseOperations();
            var ds = objDb.GetPicture(id);

            model.PersonId = id;

            model.ActualImage = (byte[])ds.Tables[0].Rows[0]["ProfilePic"];
            
            model.ContentType = Convert.ToString(ds.Tables[0].Rows[0]["ContentType"]);

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeProfilePicture(PersonViewModel model)
        {
            HttpPostedFileBase file = Request.Files["OriginalLocation"];

            
            Int32 length = file.ContentLength;
            byte[] tempImage = new byte[length];
            file.InputStream.Read(tempImage, 0, length);
            
            model.ActualImage = tempImage;

            model.ContentType = file.ContentType;

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
            var localReport = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "ReportContactsList.rdlc");
            if (System.IO.File.Exists(path))
            {
                localReport.ReportPath = path;
            }
            else
            {
                return View("ShowAllContacts");
            }
          
            var objDb = new DatabaseOperations();
            var ds = objDb.GetDataTablesForReportContactList();


            localReport.SubreportProcessing += new SubreportProcessingEventHandler(ReportSubReportProcessingEventHandler);
            localReport.SubreportProcessing += new SubreportProcessingEventHandler(ReportSubReportProcessingEventHandler2);

            

            var reportDataSource = new ReportDataSource("DataSet1", ds.Tables[0]);
         
            //----for printing reports.
            localReport.DataSources.Add(reportDataSource);


            //start
            return PrintingReports(localReport, format);
           
            //end
        }

        private ActionResult PrintingReports(LocalReport localReport, string format)
        {
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

        private void ReportSubReportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.GetDataTablesForReportEmail();

            var reportDataSource = new ReportDataSource("DataSetEmail", ds.Tables[0]);
            e.DataSources.Add(reportDataSource);
        }

        private void ReportSubReportProcessingEventHandler2(object sender, SubreportProcessingEventArgs e)
        {
            var objDb = new DatabaseOperations();
            var ds = objDb.GetDataTablesForReportNumber();
            var reportDataSource = new ReportDataSource("DataSetNumber", ds.Tables[0]);
            e.DataSources.Add(reportDataSource);
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

            //start
            return PrintingReports(localReport, format);
           
            //end
        }


        public ActionResult ShowPhoto(int id)
        {
            var objDb = new DatabaseOperations();

            var ds = objDb.GetPicture(id);
            var pModel = new PersonViewModel();

            pModel.ActualImage = (byte[])ds.Tables[0].Rows[0]["ProfilePic"];
            
            pModel.ContentType = Convert.ToString(ds.Tables[0].Rows[0]["ContentType"]);
            
            return File(pModel.ActualImage, pModel.ContentType);

        }

        

    }

}