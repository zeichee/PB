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

        public String ContentType { get; set; }

        public Byte[] ActualImage { get; set; }

        [Required]
        [StringLength(20,MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",
               ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [StringLength(10, MinimumLength = 10,ErrorMessage = "Format: MM/DD/YYYY")]
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
        [Required]
        public List<EmailsViewModel> EmailsViewModels { get; set; }


    }
}