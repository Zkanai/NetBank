using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetBank.Models;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Security.Cryptography;
using System.Web.Helpers;
using System.Data.Entity;


namespace NetBank.Controllers
{
    public class HomeController : Controller
    {

        /// <summary>
        /// Access for SMTP setting from web.config.
        /// </summary>
        public class EmailBase
        {
            public static string SmtpUser = "smtpUser";
            public static string SmtpPort = "smtpPort";
            public static string SmtpSSL = "EnableSsl";
            public static string SmtpServer = "smtpServer";
            public static string SmtpPassword = "smtpPass";
            public static string SmtpUserDisplayName = "smtpUserDisplayName";
        }


        /// <summary>
        /// Email sending class parameters.
        /// </summary>
        public class RegistrationEmail : EmailBase
        {
            public static string Subject = "Új számlanyitási igény";

            public static string GetBodyContent(string name, string url, string accountNumber)
            {
                return "Tisztelt " + name + "!" + "\nKöszönjük ,hogy a mi bankunkat választotta, új számla nyitáshoz kattintson a linkre:  " + url + " ." +
                        "\nSzámla száma: " + accountNumber;
            }

            public static Uri GetActivationUri(string registrationToken)
            {
                return new Uri("http://localhost:64415/Registration/TokenValidation" + "?token=" + registrationToken);
            }
        }

        NetBankDatabaseEntities db = new NetBankDatabaseEntities();

        /// <summary>
        /// Index page get action.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            int? Id = SessionState.Current.CurrentAccountId;
            return View();
        }

        /// <summary>
        /// Index page POST action, also new Bank Account Number can be requested here.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(HomeViewModel model)
        {
            try
            {
                //Generating account number.
                string newAccountNumber = GetAccountNumber();

                if (!ModelState.IsValid)
                {
                    model.ResultMessage = "Hiba Történt!";
                    return View(model);
                }
                //Generating token for registry with GUID.
                var token = Guid.NewGuid();

                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    // Token data with 1 hour expiry.
                    var newToken = new TokenManager
                    {
                        TokenKey = token.ToString(),
                        TokenIssued = DateTime.Now,
                        TokenExpiry = DateTime.Now.AddHours(1),
                        IsActive = true
                    };

                    //Creating new Account.
                    var newAccountRequest = new CurrentAccount
                    {
                        AccountNumber = newAccountNumber,
                        Balance = 0,
                        EmailAddress = model.To,
                        TokenManagerTokenKey = token.ToString()
                    };

                    // adding new user to database
                    db.CurrentAccount.Add(newAccountRequest);

                    // adding token to database
                    db.TokenManager.Add(newToken);

                    db.SaveChanges();
                    transaction.Commit();
                }

                // URI address for the user to click on, in the sent email.                    
                Uri uriAddress = RegistrationEmail.GetActivationUri(token.ToString());

                // Using the data configured in the webconfig for sending Mail.
                var sendMail = new MailAddress(WebConfigurationManager.AppSettings[RegistrationEmail.SmtpUser], WebConfigurationManager.AppSettings[RegistrationEmail.SmtpUserDisplayName]);
                var receiverMail = new MailAddress(model.To);
                var password = WebConfigurationManager.AppSettings[RegistrationEmail.SmtpPassword];
                var subject = RegistrationEmail.Subject;
                var body = RegistrationEmail.GetBodyContent(model.Name, uriAddress.ToString(), newAccountNumber);

                // Creating new SMTP client, if necessary change data accordingly in webconfig.
                using (var smtp = new SmtpClient
                {
                    Host = WebConfigurationManager.AppSettings[RegistrationEmail.SmtpServer],
                    Port = Convert.ToInt32(WebConfigurationManager.AppSettings[RegistrationEmail.SmtpPort]),
                    EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings[RegistrationEmail.SmtpSSL]),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sendMail.Address, password)
                })
                {
                    var mailMessage = new MailMessage(sendMail.ToString(), model.To)
                    {
                        Subject = subject,
                        Body = body
                    };
                    
                    smtp.Send(mailMessage);
                    model.ResultMessage = "Email kiküldve!";
                }
                
                return View(model);
            }
            catch
            {
                //Catching Exception and displaying error Message.

                model.ResultMessage = "Hiba Történt!";
                return View(model);
            }
        }

        /// <summary>
        /// Error page, displaying in case of error in th process.
        /// </summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            return View();
        }


        /// <summary>
        /// Index login page with session start.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index_Login(HomeViewModel model)
        {
            // Error Message content during Login.
            string errorResult = "Hibás bejelentkezési paraméterek!";
            try
            {
                // Get user from database, according to entered email address.
                var user = db.CurrentAccount.FirstOrDefault(userAttempt => userAttempt.EmailAddress == model.Email);
                //Password field, required for validation.
                var password = model.Password;
                //Validate user and Password field
                if (user == null || password == null)
                {
                    model.ResultMessage = errorResult;
                    return View("Index", model);
                }

                // If login model is OK.
                if (ModelState.IsValid)
                {
                    // Get the salt belonging to the user.
                    var salt = user.Salt;
                    // Hash the password again with the salt from the database.
                    var loginPassword = Crypto.SHA256(password + salt);
                    // See if the two hashes match, if yes the user is logged in.
                    if (user.Password == loginPassword)
                    {
                        ViewBag.Message = "Sikeres Bejelentkezés!";
                        SessionState.Current.CurrentAccountId = user.Id;
                        //If a person logs in display their name.
                        if (user.CompanyId == null)
                        {
                            SessionState.Current.CurrentUserName = user.Person.FullName;
                        }
                        // If a company logs in display its name.
                        else if (user.PersonId == null)
                        {
                            SessionState.Current.CurrentUserName = user.Company.Name;
                        }
                        return RedirectToAction("Index", "User");
                    }
                }
                model.ResultMessage = errorResult;
                return View("Index", model);
            }
            catch
            {
                model.ResultMessage = errorResult;
                return RedirectToAction("Index", "Home", model);
            }
        }

        /// <summary>
        /// This function generates the unique 3x8 Bank Account Number, and puts it in a string List.
        /// </summary>
        /// <returns></returns>
        public string GetAccountNumber()
        {
            List<string> newAccountNumber = new List<string>();
            bool result = false;
            string accountNumber = String.Empty;
            try
            {
                do
                {
                    // The three parts of the Bank account number, as strings.
                    string accountNumberParts = String.Empty;

                    for (int i = 0; i < 3; i++)
                    {
                        //Generating the parts of the account number.
                        accountNumberParts = GenerateAccountNumberParts();
                        // Adding the generated number to the string list.
                        newAccountNumber.Add(accountNumberParts);
                    }

                    // Creating a string out of the obtained list.
                    for (int i = 0; i < newAccountNumber.Count; i++)
                    {
                        accountNumber += newAccountNumber[i] + "-";
                    }
                    //Formatting string to adequate format.
                    accountNumber = accountNumber.TrimEnd('-');

                    //Checking if there is a match with custom function.
                    result = IsNotMatch(accountNumber);
                }
                while (result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // Returns a list with 3 elements with 8 digit numbers.
            return accountNumber;
        }

        /// <summary>
        /// Checks Database for matching Current Account Number, if there is a match it returns false.
        /// </summary>
        /// <param name="accnum"></param>
        /// <returns></returns>
        public bool IsNotMatch(string accnum)
        {
            bool isNotMatch = false;
            try
            {
                //Looking up matches in the database.
                if (db.CurrentAccount.Any(curraccnum => curraccnum.AccountNumber == accnum))
                {
                    isNotMatch = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isNotMatch;
        }

        /// <summary>
        /// Logout button, sets the CurrentAccountId in the session to null.
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            SessionState.Current.CurrentAccountId = null;
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// This function generates the account number parts.
        /// </summary>
        /// <returns></returns>
        public string GenerateAccountNumberParts()
        {
            string getNumber = String.Empty;

            // Secure random numbers are important if you are creating a very high volume of numbers or
            // need to ensure proper uniqueness over a long period of time.
            // RNGCryptoServiceProvider should be disposed.
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                // Necessary Parameters for creating the unique BankAccountNumber
                byte[] byteArray = new byte[4];

                uint value = 0;

                for (int i = 0; i < 8; i++)
                {
                    do
                    {
                        provider.GetBytes(byteArray);
                        value = BitConverter.ToUInt32(byteArray, 0);
                    } while (value == 0);
                    getNumber += ((int)(value % 9)).ToString();
                }
            }
            // Adding the generated number to the string list.
            return getNumber;
        }
    }
}
