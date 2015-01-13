using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamplePB.Models
{
    public class ContactNumbersViewModel
    {
        public int ContactId { get; set; }
        public int PersonId { get; set; }
        [Display(Name = "Contact Type")]
        public string SelectedContactType { get; set; }

        public IEnumerable<SelectListItem> ContactTypes { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        [StringLength(11)]
        [RegularExpression(@"^[0-9]{0,15}$", ErrorMessage = "Contact number should contain only numbers")]
        public string ContactNumber { get; set; }


        public DataSet StoreContactNumbers { get; set; }
    }
}