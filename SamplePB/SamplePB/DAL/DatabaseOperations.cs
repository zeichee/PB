using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.WebPages;
using SamplePB.Models;

namespace SamplePB.DAL
{
    public class DatabaseOperations
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ContactDbContext"].ToString());
        public string InsertContactPerson(PersonViewModel model)
        {
            string result = "";
            DataSet ds = null;
            try
            {
                var cmd = new SqlCommand("uspContactPersonAddition", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@ProfilePic", model.ActualImage);
                cmd.Parameters.AddWithValue("@ContentType", model.ContentType);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@MiddleName", model.MiddleName);
                cmd.Parameters.AddWithValue("@BirthDate", model.BirthDate);
                cmd.Parameters.AddWithValue("@HomeAddress", model.HomeAddress);
                cmd.Parameters.AddWithValue("@Company", model.Company);
                con.Open();

                result = cmd.ExecuteScalar().ToString();
                
                return result;
            }
            catch
            {
                
                return result; 
            }
            finally 
            {
                con.Close();
            }
        }


        public string UpdateContactPerson(PersonViewModel model)
        {
            string result = "";
            try
            {
                var cmd = new SqlCommand("uspContactPersonInfoUpdate", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@PersonID", model.PersonId);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@MiddleName", model.MiddleName);
                cmd.Parameters.AddWithValue("@BirthDate", model.BirthDate);
                cmd.Parameters.AddWithValue("@HomeAddress", model.HomeAddress);
                cmd.Parameters.AddWithValue("@Company", model.Company);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }

        public string DeleteContact(int personId)
        {
            string result = "";
            try
            { 
                var cmd = new SqlCommand("uspContactDeletion", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@PersonID", personId);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                 con.Close();
            }
        }
        public DataSet SelectAllContacts(PersonViewModel model)
        {
            if (model.SearchString.IsEmpty())
            {
                DataSet ds = null;
                try
                {
                    var cmd = new SqlCommand("uspContactList", con) {CommandType = CommandType.StoredProcedure};
                    con.Open();
                    var da = new SqlDataAdapter {SelectCommand = cmd};
                    ds = new DataSet();
                    da.Fill(ds);
                    model.Result = "";
                    return ds;
                }
                catch
                {
                    return ds;
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                string result = "";
                DataSet ds = null;
                try
                {
                    var cmd = new SqlCommand("uspContactSearching", con) {CommandType = CommandType.StoredProcedure};
                    cmd.Parameters.AddWithValue("@Searcher", model.SearchString);
                    con.Open();
                    var da = new SqlDataAdapter {SelectCommand = cmd};
                    ds = new DataSet();
                    da.Fill(ds);
                    model.Result = "No Results";
                    return ds;
                }
                catch
                {
                    return ds;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public DataSet SelectById(int id)
        {
            DataSet ds = null;
            try
            {
                var cmd = new SqlCommand("uspContactShowDetail", con);
                ds = new DataSet();
                var da = new SqlDataAdapter();
            
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PersonID", id);
                con.Open();
                
                da.SelectCommand = cmd;
                
                da.TableMappings.Add("Table", "tblPerson");
                da.TableMappings.Add("Table1", "tblContactNumbers");
                da.TableMappings.Add("Table2", "tblEmails");
                da.Fill(ds);
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public string AddEmails(EmailsViewModel eModel)
        {
            string result = "";
            try
            {
                var cmd = new SqlCommand("uspEmailAddition", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@PersonID", eModel.PersonId);
                cmd.Parameters.AddWithValue("@EmailAddress", eModel.Emails);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }
        public string AddContactNumbers(ContactNumbersViewModel cModel)
        {
            string result = "";
            try
            {
                var cmd = new SqlCommand("uspContactAddition", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@PersonID", cModel.PersonId);
                cmd.Parameters.AddWithValue("@SelectedContactType", cModel.SelectedContactType);
                cmd.Parameters.AddWithValue("@ContactNumber", cModel.ContactNumber);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet SearchContact(string searcher)
        {
            DataSet ds = null;
            try
            {
                var cmd = new SqlCommand("uspContactPersonSearch", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@searcher", searcher);
                con.Open();
                var da = new SqlDataAdapter {SelectCommand = cmd};
                ds = new DataSet();
                da.Fill(ds);
                return ds;
             }
            catch
            {
                return ds;
            }
            finally
            {
                con.Close();
            }
        }

       public string ViewContactDetails(int personId)
        {
            string result = "";
            try
            {
                var cmd = new SqlCommand("uspContactSearching", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@PersonID", personId);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                con.Close();
            }

        }
        public DataSet SelectByContactId(int id)
        {
            string result = "";
            DataSet ds = null;

            try
            {
               var cmd = new SqlCommand("uspContactNumberSearchId", con);
                ds = new DataSet();
               
                var da = new SqlDataAdapter();
                
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ContactID", id);
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public string UpdateContactNumber(ContactNumbersViewModel model)
        {
            var result = "";
            try
            {
                var cmd = new SqlCommand("uspContactNumberUpdate", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@ContactID", model.ContactId);
                cmd.Parameters.AddWithValue("@PersonID", model.PersonId);
                cmd.Parameters.AddWithValue("@SelectedContactType", model.SelectedContactType);
                cmd.Parameters.AddWithValue("@ContactNumber", model.ContactNumber);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }
        public string DeleteContactNumber(int contactId)
        {
            string result = "";
            try
            {
                var cmd = new SqlCommand("uspContactNumberDeletion", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@ContactID", contactId);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet SelectByEmailId(int id)
        {
            DataSet ds = null;

            try
            {
                var cmd = new SqlCommand("uspEmailSearchById", con);
                ds = new DataSet();

                var da = new SqlDataAdapter();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmailID", id);
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public string UpdateEmail(EmailsViewModel model)
        {
            string result = "";
            try
            {
                
                var cmd = new SqlCommand("uspEmailUpdate", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@EmailID", model.EmailId);
                cmd.Parameters.AddWithValue("@EmailAddress", model.Emails);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }
        public string DeleteEmail(EmailsViewModel model)
        {
            string result = "";
            try
            {
                var cmd = new SqlCommand("uspEmailDeletion", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@EmailID", model.EmailId);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }
        public string ChangeProfilePicture(PersonViewModel model)
        {
            string result = "";
            try
            {
                var cmd = new SqlCommand("uspChangeProfilePic", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@PersonID", model.PersonId);
                cmd.Parameters.AddWithValue("@ProfilePic", model.ActualImage);
                cmd.Parameters.AddWithValue("@ContentType", model.ContentType);

                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;
            }
            catch
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet SelectLastInsertPerson(PersonViewModel model)
        {
            DataSet ds = null;
            try
            {
                var cmd = new SqlCommand("uspSelectLastInsertedPerson", con) { CommandType = CommandType.StoredProcedure };
                con.Open();
                var da = new SqlDataAdapter { SelectCommand = cmd };
                ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet GetDataTablesForReportContactList()
        {
            DataSet ds = null;
            try
            {
                var cmd = new SqlCommand("uspMasterList", con) { CommandType = CommandType.StoredProcedure };
                con.Open();
                var da = new SqlDataAdapter { SelectCommand = cmd };
                ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet GetDataTablesForReportPersonalDetail(int id)
        {
            //storedproc
            var cmd = new SqlCommand("uspContactShowDetail", con);

            var ds = new DataSet();
            var da = new SqlDataAdapter();

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PersonID", id);
                con.Open();

                da.SelectCommand = cmd;

                da.TableMappings.Add("Table", "tblPerson");
                da.TableMappings.Add("Table1", "tblContactNumbers");
                da.TableMappings.Add("Table2", "tblEmails");
                da.Fill(ds);
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet GetPicture(int id)
        {
            var ds = new DataSet();
            try
            {
                var model = new PersonViewModel();
                var cmd = new SqlCommand("uspGetImage", con) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@PersonID", id);

                con.Open();
                var da = new SqlDataAdapter {SelectCommand = cmd};

                da.Fill(ds);
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet GetDataTablesForReportEmail()
        {
            SqlConnection con = null;
            DataSet ds = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["ContactDbContext"].ToString());
                var cmd = new SqlCommand("uspSelectAllPersonEmail", con) {CommandType = CommandType.StoredProcedure};


                con.Open();
                var da = new SqlDataAdapter {SelectCommand = cmd};
                ds = new DataSet();

                da.Fill(ds);
                return ds;
            }
            catch
            {

                return ds;
            }
            finally
            {
                if(con!=null) con.Close();
            }

        }
        public DataSet GetDataTablesForReportNumber()
        {
            SqlConnection con = null;
            DataSet ds = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["ContactDbContext"].ToString());
                var cmd = new SqlCommand("uspSelectAllPersonNumber", con) { CommandType = CommandType.StoredProcedure };


                con.Open();
                var da = new SqlDataAdapter { SelectCommand = cmd };
                ds = new DataSet();

                da.Fill(ds);
                return ds;
            }
            catch
            {

                return ds;
            }
            finally
            {
                if (con != null) con.Close();
            }

        }
    }

}