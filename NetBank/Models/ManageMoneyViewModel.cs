using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NetBank.Models
{
    /// <summary>
    /// This will be the model for the Transaction starting and deposit and add money
    /// </summary>
    public class ManageMoneyViewModel
    {
        public CreateNewTransactionViewModel NewTransaction { get; set; }

        public ManageMoneyViewModel()
        {
            NewTransaction = new CreateNewTransactionViewModel();
        }
    }
    /// <summary>
    /// Creating a new transaction will be on a partial view model.
    /// </summary>
    public class CreateNewTransactionViewModel
    {
        [Display(Name = "Az ön számlaszáma")]
        public string FromAccount { get; set; }

        [Display(Name = "Kedvezményezett számlaszáma: ")]
        [MaxLength(26, ErrorMessage = "Túllépte a megengedett karakterszámot!")]
        public string ToAccount { get; set; }

        [Display(Name = "Összeg")]
        public int Amount { get; set; }

        public DateTime TransactionStartTime { get; set; }

        [Display(Name = "Teljesítés időpontja")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime TransactionDueDate { get; set; }

        [Display(Name = "Közlemény")]
        public string Description { get; set; }
    }
}