using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetBank.Models;

namespace NetBank.Controllers
{
    public class UserController : Controller
    {
        NetBankDatabaseEntities db = new NetBankDatabaseEntities();

        /// <summary>
        /// Decides based on the CurrentAccountId if it is a Company or Person.
        /// Creates the partial views for models based on this information. 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            int? id = SessionState.Current.CurrentAccountId;
            UserViewModel model = new UserViewModel();
            try
            {
                CurrentAccount current = db.CurrentAccount.FirstOrDefault(user => user.Id == id);

                //If this is a company
                if (current.PersonId == null)
                {
                    // Mapping GeneralData Necessary for Company Bank Account View.
                    model.GeneralData.Id = current.Id;
                    model.GeneralData.IsCompany = true;
                    model.GeneralData.AccountNumber = current.AccountNumber;
                    model.GeneralData.AccountOpenedDate = current.AccountOpenedDate;
                    model.GeneralData.Balance = current.Balance;
                    model.GeneralData.EmailAddress = current.EmailAddress;

                    // Mapping the UserSpecific Data for view
                    model.CompanyView.Address = current.Company.Address;
                    model.CompanyView.CompanyName = current.Company.Name;
                    model.CompanyView.ContactName = current.Company.ContactName;
                    model.CompanyView.RegistryNumber = current.Company.RegistryNumber;
                    model.CompanyView.TaxNumber = current.Company.TaxNumber;
                    model.CompanyView.TelephoneNumber = current.Company.TelephoneNumber;

                    return View(model);
                }
                //If this is a Person
                else if (current.CompanyId == null)
                {
                    // Mapping Data Necessary for Personal Bank Account View.
                    model.GeneralData.Id = current.Id;
                    model.GeneralData.IsCompany = false;
                    model.GeneralData.AccountNumber = current.AccountNumber;
                    model.GeneralData.AccountOpenedDate = current.AccountOpenedDate;
                    model.GeneralData.Balance = current.Balance;
                    model.GeneralData.EmailAddress = current.EmailAddress;

                    // Mapping the UserSpecific Data for view
                    model.PersonView.FullName = current.Person.FullName;
                    model.PersonView.MotherName = current.Person.MotherName;
                    model.PersonView.Address = current.Person.Address;
                    model.PersonView.TelephoneNumber = current.Person.TelephoneNumber;

                    return View(model);
                }
            }
            //If there is an error it is redirected to the main Index page
            // of the Home controller.
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(model);
        }

        /// <summary>
        /// Get action for "_TransactionHistory" partial view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PartialViewResult CurrentUser_TransactionHistory(UserViewModel model)
        {
            try
            {
                int? Id = SessionState.Current.CurrentAccountId;

                UserViewModel myTransactions = new UserViewModel();
                CurrentAccount current = db.CurrentAccount.FirstOrDefault(user => user.Id == Id);

                // Mapping Transactions data to a list.
                // Where the transaction source OR transaction target is the Current User.
                myTransactions.Transactions.DefaultRow = db.Transaction.Where(usr => usr.To == current.Id | current.Id == usr.From)
                         .Select(row => new UserTransactionResultRowData
                         {
                             TransactionAmount = row.Amount,
                             TransactionDueDate = row.DueDate,
                             TransactionToAccount = row.ToAccount.AccountNumber,
                             TransactionFromAccount = row.FromAccount.AccountNumber,
                             TransactionStartDate = row.Date,
                             TransactionDescription = row.Description

                         }).ToList();

                return PartialView("_TransactionHistory", myTransactions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This partial view serves for the filtering of the current users Transaction history
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PartialViewResult User_Transaction_Search(UserViewModel model)
        {
            var transactionSearch = model.Transactions;
            string currentAccountNumber = model.GeneralData.AccountNumber;
            var queryForSearch = db.Transaction.AsQueryable().Where(usr => usr.FromAccount.AccountNumber.Contains(currentAccountNumber) ||  usr.ToAccount.AccountNumber.Contains(currentAccountNumber));

            //Search Fields on the form
            // Get "To" account number, from filter
            if (!String.IsNullOrEmpty(transactionSearch.FilterRow.ToAccount))
                queryForSearch = queryForSearch.Where(tran => tran.ToAccount.AccountNumber.Contains(transactionSearch.FilterRow.ToAccount));

            // Get "From" account number, from filter
            if (!String.IsNullOrEmpty(transactionSearch.FilterRow.FromAccount))
                queryForSearch = queryForSearch.Where(tran => tran.FromAccount.AccountNumber.Contains(transactionSearch.FilterRow.FromAccount));

            // Get "Amount", from filter
            if (transactionSearch.FilterRow.Amount > 0)
                queryForSearch = queryForSearch.Where(tran => tran.Amount == transactionSearch.FilterRow.Amount);

            // Get "ExpiryDate" , from filter
            if (transactionSearch.FilterRow.ExpiryDate != null)
                queryForSearch = queryForSearch.Where(tran => tran.DueDate.Year == transactionSearch.FilterRow.ExpiryDate.Value.Year &&
                    tran.DueDate.Month == transactionSearch.FilterRow.ExpiryDate.Value.Month &&
                    tran.DueDate.Day == transactionSearch.FilterRow.ExpiryDate.Value.Day
                    );

            // Get "StartDate", from filter
            if (transactionSearch.FilterRow.StartDate != null)
                queryForSearch = queryForSearch
                    .Where(tran => tran.Date.Year == transactionSearch.FilterRow.StartDate.Value.Year &&
                    tran.Date.Month == transactionSearch.FilterRow.StartDate.Value.Month &&
                    tran.Date.Day == transactionSearch.FilterRow.StartDate.Value.Day
                    );

            // Get "Description", from filter
            if (!String.IsNullOrEmpty(transactionSearch.FilterRow.Description))
                queryForSearch = queryForSearch.Where(tran => tran.Description.Contains(transactionSearch.FilterRow.FromAccount));

            // Mapping filtered list data.
            transactionSearch.DefaultRow = queryForSearch.AsEnumerable().Select(filteredList => new UserTransactionResultRowData
            {
                TransactionAmount = filteredList.Amount,
                TransactionDescription = filteredList.Description,
                TransactionDueDate = filteredList.DueDate,
                TransactionFromAccount = filteredList.FromAccount.AccountNumber,
                TransactionStartDate = filteredList.Date,
                TransactionToAccount = filteredList.ToAccount.AccountNumber
            }).ToList();

            UserViewModel result = new UserViewModel();

            result.Transactions.DefaultRow = transactionSearch.DefaultRow;

            return PartialView("_TransactionHistory", result);
        }

        /// <summary>
        /// Preset expenses results report for "_TransactionHistory" partial view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PartialViewResult User_Transaction_Expenses(UserViewModel model)
        {
            var showExpenses = model.Transactions;

            var queryForExpenses = db.Transaction.AsQueryable();

            queryForExpenses = queryForExpenses.Where(expense => expense.FromAccount.Id == SessionState.Current.CurrentAccountId);

            // Mapping list data.
            showExpenses.DefaultRow = queryForExpenses.AsEnumerable().Select(filteredList => new UserTransactionResultRowData
            {
                TransactionAmount = filteredList.Amount,
                TransactionDescription = filteredList.Description,
                TransactionDueDate = filteredList.DueDate,
                TransactionFromAccount = filteredList.FromAccount.AccountNumber,
                TransactionStartDate = filteredList.Date,
                TransactionToAccount = filteredList.ToAccount.AccountNumber
            }).ToList();

            UserViewModel result = new UserViewModel();
            result.Transactions.DefaultRow = showExpenses.DefaultRow;


            return PartialView("_TransactionHistory", result);
        }

        /// <summary>
        /// Preset income results report for "_TransactionHistory" partial view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PartialViewResult User_Transaction_Income(UserViewModel model)
        {
            var showExpenses = model.Transactions;

            var queryForExpenses = db.Transaction.AsQueryable();

            queryForExpenses = queryForExpenses.Where(expense => expense.ToAccount.Id == SessionState.Current.CurrentAccountId);

            // Mapping list data.
            showExpenses.DefaultRow = queryForExpenses.AsEnumerable().Select(filteredList => new UserTransactionResultRowData
            {
                TransactionAmount = filteredList.Amount,
                TransactionDescription = filteredList.Description,
                TransactionDueDate = filteredList.DueDate,
                TransactionFromAccount = filteredList.FromAccount.AccountNumber,
                TransactionStartDate = filteredList.Date,
                TransactionToAccount = filteredList.ToAccount.AccountNumber
            }).ToList();

            UserViewModel result = new UserViewModel();
            result.Transactions.DefaultRow = showExpenses.DefaultRow;

            return PartialView("_TransactionHistory", result);
        }
    }
}