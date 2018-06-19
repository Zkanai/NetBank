using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetBank.Models;
using System.Data.Entity;

namespace NetBank.Controllers
{
    public class ManageMoneyController : Controller
    {
        NetBankDatabaseEntities db = new NetBankDatabaseEntities();

        /// <summary>
        /// Gets the Manage Money Index page, which has a partial view on it named "_Transaction".
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                ManageMoneyViewModel manageMoneyView = new ManageMoneyViewModel();

                if (SessionState.Current.CurrentAccountId != null)
                {
                    var currentUser = db.CurrentAccount.FirstOrDefault(current => current.Id == SessionState.Current.CurrentAccountId);

                    manageMoneyView.NewTransaction.FromAccount = currentUser.AccountNumber;
                }

                return View(manageMoneyView);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }


        /// <summary>
        /// Partial view post.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index_Transaction(ManageMoneyViewModel model)
        {
            try
            {
                //Getting "To" and "From" current accounts.
                var getToData = db.CurrentAccount.FirstOrDefault(toUser => toUser.AccountNumber == model.NewTransaction.ToAccount);
                var accountBalance = db.CurrentAccount.FirstOrDefault(thisacc => thisacc.AccountNumber == model.NewTransaction.FromAccount);

                // Server Side validations
                if (getToData == null || model.NewTransaction.ToAccount == model.NewTransaction.FromAccount)
                    ModelState.AddModelError("Hibás számlaszám", "Kérem ellenőrizze a bevitt számlaszámot!");

                if (accountBalance.Balance < model.NewTransaction.Amount)
                    ModelState.AddModelError("Egyenleg hiba", "Az utalni kívánt összeg nagyobb, mint az egyenlege!");

                if (model.NewTransaction.TransactionDueDate == null)
                    ModelState.AddModelError("Hiányos adat", "A lejárati idő kitöltése kötelező!");

                if (model.NewTransaction.Amount < 0)
                    ModelState.AddModelError("hibás érték", "Hibás értéket adott meg kérem ellenőrizze");

                if (model.NewTransaction.TransactionDueDate < DateTime.Now)
                    ModelState.AddModelError("Lejárati idő", "Hibás lejárati időt adott meg kérem ellenőrizze!");

                if (!ModelState.IsValid)
                    return View("Index", model);

                //If everything was OK -> Starting Db Transaction
                using (DbContextTransaction currentTransaction = db.Database.BeginTransaction())
                {
                    //Creating new record for Transaction Table in Database
                    Transaction newTransaction = new Transaction
                    {
                        To = getToData.Id,
                        From = SessionState.Current.CurrentAccountId.GetValueOrDefault(),
                        Amount = model.NewTransaction.Amount,
                        Date = DateTime.Now,
                        DueDate = model.NewTransaction.TransactionDueDate,
                        Description = model.NewTransaction.Description
                    };

                    // Adding new Entity
                    db.Transaction.Add(newTransaction);

                    // Subtract amount from "From" account
                    var subtractAmount = db.CurrentAccount.FirstOrDefault(u => u.Id == SessionState.Current.CurrentAccountId);
                    subtractAmount.Balance -= model.NewTransaction.Amount;
                    db.Entry(subtractAmount).State = EntityState.Modified;

                    // Adding amount to "To" account
                    var addAmount = db.CurrentAccount.FirstOrDefault(acc => acc.AccountNumber == model.NewTransaction.ToAccount);
                    addAmount.Balance += model.NewTransaction.Amount;
                    db.Entry(addAmount).State = EntityState.Modified;

                    // Saving changes
                    db.SaveChanges();
                    // Committing DataBase Transaction.
                    currentTransaction.Commit();
                    // If everything is OK redirect user to their main-page
                    return RedirectToAction("Index","User");
                }
            }
            catch 
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}