using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace SamplePB.Models
{
    public class EmailsViewModel
    {
        public int EmailId { get; set; }

        public int PersonId { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [StringLength(40)]
        [DataType(DataType.EmailAddress)]

        public string Emails { get; set; }

        public DataSet StoreEmails { get; set; }
    }
}