using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace NetBank.Models
{
    public class RegistrationViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Email címe")]
        public string EmailAddress { get; set; }
        [Display(Name = "Számlaszám")]
        public string AccountNumber { get; set; }
        
        public bool TokenIsValid { get; set; }
    }
    /// <summary>
    /// These are the data for the Signup form of Personal Current Account creation.
    /// </summary>
    public class PersonRegistrationViewModel
    {
        /// <summary>
        /// This class includes Telephone number, email address, the password with the matching.
        /// </summary>
        public RegistrationGeneralData PersonalCommonData { get; set; }

        // These are the specific data that are required from a Personal Account type user.
        [Display(Name = "Személyi Igazolvány száma")]
        [Required]
        public string IdNumber { get; set; }

        [Display(Name = "Anyja Neve")]
        [Required]
        public string MotherName { get; set; }

        public PersonRegistrationViewModel()
        {
            PersonalCommonData = new RegistrationGeneralData(); 
        }
    }

    /// <summary>
    /// These are data for the Company signup form for Current Account.
    /// </summary>
    public class CompanyRegistrationViewModel
    {
        /// <summary>
        /// This class includes Telephone number, email address, the password with confirmation.
        /// </summary>
        public RegistrationGeneralData CompanyCommonData { get; set; }

        // These are the specific data that are required from a Company Account type user.
        [Display(Name = "Kontakt neve")]
        [Required]
        public string ContactName { get; set; }

        [Display(Name = "Adószám")]
        [Required]
        [MaxLength(13, ErrorMessage = "Túllépte a karaktermennyiséget!")]
        public string TaxNumber { get; set; }

        [Display(Name = "Cégjegyzék szám")]
        [MaxLength(13, ErrorMessage = "A cégjegyzékszám max 10 karakter hosszú lehet")]
        [Required]
        public string CompanyRegistryNumber { get; set; }

        public CompanyRegistrationViewModel()
        {
            CompanyCommonData = new RegistrationGeneralData();
        }
    }

    /// <summary>
    /// General data that both of the account types have.
    /// </summary>
    public class RegistrationGeneralData
    {
        [Display(Name = "Számlaszám")]
        public string AccountNumber { get; set; }

        [Display(Name = "Cím")]
        [Required]
        public string Address { get; set; }

        [Display(Name = "Név")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Email címe")]
        public string EmailAddress { get; set; }

        [Display(Name = "Telefonszám")]
        [Required]
        [MaxLength(16,ErrorMessage ="Túl hosszú a megadott telefonszám!")]
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// The Password is required to be given twice for comparison, its minimum length is 6.
        /// </summary>
        [Display(Name = "Jelszó")]
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "A {0}nak minimum {1} karakter hosszúnak kell lennie", MinimumLength = 6)]
        public string Password { get; set; }
        /// <summary>
        /// Password matching with the previously given one.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó megerősítése")]
        [Compare("Password", ErrorMessage = "A jelszavak nem egyeznek")]
        public string ConfirmPassword { get; set; }
    }
}