using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SamplePB.Models
{
    public class SearchPerson
    {

        
        public string SearchString { get; set; }

        public int PersonId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }
        
        public string LastNameOrFirstName { get; set; }

        public DataSet StoreAllData { get; set; }


    }
}