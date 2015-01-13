using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using Antlr.Runtime.Misc;

namespace SamplePB.Models
{
    public class PersonViewModel
    {
        public PersonViewModel()
        {
            ContactNumbersViewModels = new List<ContactNumbersViewModel>();
            EmailsViewModels = new ListStack<EmailsViewModel>();
        }

        public string SearchString { get; set; }

        public int PersonId { get; set; }
        [StringLength(20)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [StringLength(20)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [StringLength(20)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",
               ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public string BirthDate { get; set; }

        [StringLength(50)]
        [Display(Name = "Home Address")]
        public string HomeAddress { get; set; }

        [StringLength(30)]
        [Display(Name = "Company")]
        public string Company { get; set; }
        public DataSet StoreAllData { get; set; }

        public List<ContactNumbersViewModel> ContactNumbersViewModels { get; set; }
        public List<EmailsViewModel> EmailsViewModels { get; set; }
    }
}