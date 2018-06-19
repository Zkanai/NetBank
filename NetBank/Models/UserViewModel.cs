using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NetBank.Models
{
    /// <summary>
    /// Defines the UserViewModel with transactions History.
    /// </summary>
    public class UserViewModel
    {
        public PersonalUserViewModel PersonView { get; set; }
        public CompanyUserViewModel CompanyView { get; set; }
        public UserTransactionViewModel Transactions { get; set; }
        public UserGeneralData GeneralData { get; set; }

        public UserViewModel()
        {
            PersonView = new PersonalUserViewModel();
            CompanyView = new CompanyUserViewModel();
            Transactions = new UserTransactionViewModel();
            GeneralData = new UserGeneralData();
        }
    }
   
    /// <summary>
    /// Partial view in UserViewModel/Index.
    /// </summary>
    public class PersonalUserViewModel
    {
        [Display(Name = "Teljese Neve")]
        public string FullName { get; set; }

        [Display(Name = "Személyi Igazolvány száma")]
        public string IdNumber { get; set; }

        [Display(Name = "Anyja Neve")]
        public string MotherName { get; set; }

        [Display(Name = "Telefonszáma")]
        public string TelephoneNumber { get; set; }
        [Display(Name = "Lakcíme")]
        public string Address { get; set; }
    }

    /// <summary>
    /// Defines Data that is present in both personal and company account.
    /// </summary>
    public class UserGeneralData
    {
        public int Id { get; set; }

        [Display(Name = "Számlaszáma")]
        public string AccountNumber { get; set; }

        [Display(Name = "Egyenleg")]
        public int Balance { get; set; }

        [Display(Name = "Számlanyitás Dátuma")]
        public DateTime? AccountOpenedDate { get; set; }

        [Display(Name = "Email címe")]
        public string EmailAddress { get; set; }

        public bool IsCompany { get; set; }
    }

    /// <summary>
    ///Partial view in UserViewModel/Index 
    /// </summary>
    public class CompanyUserViewModel
    {
        [Display(Name = "Cég neve")]
        public string CompanyName { get; set; }

        [Display(Name = "Adószám")]
        public string TaxNumber { get; set; }

        [Display(Name = "Cég kapcsolattartója")]
        public string ContactName { get; set; }

        [Display(Name = "Cégjegyzékszám")]
        public string RegistryNumber { get; set; }

        [Display(Name = "Kapcsolattartó telefonszáma")]
        public string TelephoneNumber { get; set; }

        [Display(Name = "Cég telephelyének címe")]
        public string Address { get; set; }
    }
    
    /// <summary>
    /// User Transaction View Model, defining search paramaters for history and starting transactions.
    /// </summary>
    public class UserTransactionViewModel
    {
        public UserTransactionViewModel()
        {
            DefaultRow = new List<UserTransactionResultRowData>();
            FilterRow = new UserTransactionFilterRow();
        }

        public IEnumerable<UserTransactionResultRowData> DefaultRow { get; set; }
        public UserTransactionFilterRow FilterRow { get; set; }
    }
 
    /// <summary>
    /// Describes the values that will show in the table on the UserView.
    /// </summary>
    public class UserTransactionResultRowData
    {
        public string TransactionToAccount { get; set; }
        public string TransactionFromAccount { get; set; }
        public string TransactionDescription { get; set; }
        public int TransactionAmount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime TransactionDueDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime TransactionStartDate { get; set; }
    }

    /// <summary>
    /// Describes the Search Parameters for the view.
    /// </summary>
    public class UserTransactionFilterRow
    {
        [Display(Name = "Számlának küldött")]
        public string ToAccount { get; set; }

        [Display(Name = "Számlától kapott")]
        public string FromAccount { get; set; }

        [Display(Name = "Összeg")]
        public int? Amount { get; set; }

        [Display(Name = "Lejárati Idő")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Utalás indításának időpontja")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Közlemény")]
        public string Description { get; set; }
    }

}
