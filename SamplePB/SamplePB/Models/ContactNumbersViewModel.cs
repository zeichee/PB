using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Web.Mvc;

namespace SamplePB.Models
{
    public class ContactNumbersViewModel
    {
        public int ContactId { get; set; }
        public int PersonId { get; set; }

        public IEnumerable<SelectListItem> ContactType { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 3)]
        [Display(Name = "Contact Type")]
        public string SelectedContactType { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        [StringLength(11, MinimumLength = 7)]
        [RegularExpression(@"^[0-9]{0,15}$", ErrorMessage = "Contact number should contain only numbers")]
        public string ContactNumber { get; set; }

    }
}