using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using NetBank.Models;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Web.Helpers;

namespace NetBank.Controllers
{
    public class RegistrationController : Controller
    {
        NetBankDatabaseEntities db = new NetBankDatabaseEntities();

        /// <summary>
        /// Gets personal bank account opening page if the user has a valid ID.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult PersonalBankAccountOpening(int? Id)
        {
            try
            {
                var inDB = db.CurrentAccount.Where(hasValue => hasValue.PersonId == null && hasValue.CompanyId == null).FirstOrDefault(usr => usr.Id == Id).Equals(1);

                if (inDB == false)
                {
                    PersonRegistrationViewModel personRegistration = new PersonRegistrationViewModel();
                    try
                    {
                        CurrentAccount dbAcc = db.CurrentAccount.Where(model => model.Id == Id).FirstOrDefault();
                        personRegistration.PersonalCommonData.AccountNumber = dbAcc.AccountNumber;
                        personRegistration.PersonalCommonData.EmailAddress = dbAcc.EmailAddress;

                        return View(personRegistration);
                    }
                    catch (Exception)
                    {
                        ViewBag.Message = "Sikertelen feldolgozás";
                        return View(personRegistration);
                    }

                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Gets page for company bank account opening page if the user has a valid ID.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CompanyBankAccountOpening(int? Id)
        {
            try
            {
                var inDB = db.CurrentAccount.Where(hasValue => hasValue.PersonId == null && hasValue.CompanyId == null).FirstOrDefault(usr => usr.Id == Id).Equals(1);

                if (inDB == false)
                {

                    CompanyRegistrationViewModel companyRegistration = new CompanyRegistrationViewModel();
                    try
                    {
                        CurrentAccount dbAcc = db.CurrentAccount.Where(model => model.Id == Id).FirstOrDefault();
                        companyRegistration.CompanyCommonData.AccountNumber = dbAcc.AccountNumber;
                        companyRegistration.CompanyCommonData.EmailAddress = dbAcc.EmailAddress;

                        return View(companyRegistration);
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Validates token for user, if it valid the registration can continue.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult TokenValidation(string token)
        {
            RegistrationViewModel newUser = new RegistrationViewModel();
            if (db.TokenManager.Any(model => model.TokenKey == token && model.TokenExpiry > DateTime.Now && model.IsActive == true))
            {
                newUser = db.CurrentAccount.Where(model => model.TokenManagerTokenKey == token).Select(dbdata => new RegistrationViewModel
                {
                    AccountNumber = dbdata.AccountNumber,
                    Id = dbdata.Id,
                    TokenIsValid = true
                }).FirstOrDefault();
                return View(newUser);
            }
            else
            {
                newUser.TokenIsValid = false;
                return View(newUser);
            }
        }


        /// <summary>
        /// Post action for Company Account opening.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CompanyBankAccountOpening(CompanyRegistrationViewModel model)
        {

            // Server side validations 
            // Telephone number validation
            bool validTelephoneNumber = IsValidTelephoneNumber(model.CompanyCommonData.TelephoneNumber);
            if (!validTelephoneNumber) { ModelState.AddModelError("Formátum hiba", "A telefonaszám nem megfelelő formátumú!"); }

            //Validation of Company Tax Number
            bool validTaxNumber = IsValidTaxNumber(model.TaxNumber);
            if (!validTaxNumber) { ModelState.AddModelError("Formátum hiba", "Hibás formátumú az adószám!"); }

            //Validation of Company Registry Number
            bool validRegistryNumber = IsValidRegistryNumber(model.CompanyRegistryNumber);
            if (!validRegistryNumber) { ModelState.AddModelError("Formátum hiba", "Hibás formátumú a cégjegyzék szám!"); }
            try
            {
                if (ModelState.IsValid)
                {
                    // Hashing the password.
                    var salt = Crypto.GenerateSalt();
                    var savedPasswordHash = Crypto.SHA256(model.CompanyCommonData.Password + salt);

                    //Adding obtained data to database 
                    Company newCompany = new Company();

                    newCompany.Name = model.CompanyCommonData.Name;
                    newCompany.TaxNumber = model.TaxNumber;
                    newCompany.ContactName = model.ContactName;
                    newCompany.RegistryNumber = model.CompanyRegistryNumber;
                    newCompany.TelephoneNumber = model.CompanyCommonData.TelephoneNumber;
                    newCompany.Address = model.CompanyCommonData.Address;

                    // Adding new Company to database
                    db.Company.Add(newCompany);

                    var currentAccount = db.CurrentAccount.FirstOrDefault(currAcc => currAcc.AccountNumber == model.CompanyCommonData.AccountNumber);
                    currentAccount.AccountOpenedDate = DateTime.Now;
                    currentAccount.Password = savedPasswordHash;
                    currentAccount.Salt = salt;

                    currentAccount.Company = newCompany;
                    var tokenManagerEntry = db.TokenManager.FirstOrDefault(token => token.TokenKey == currentAccount.TokenManagerTokenKey);

                    tokenManagerEntry.IsActive = false;
                    tokenManagerEntry.AccountCreated = currentAccount.AccountOpenedDate;
                    tokenManagerEntry.TokenExpiry = null;

                    db.Entry(currentAccount).State = EntityState.Modified;
                    db.Entry(tokenManagerEntry).State = EntityState.Modified;
                    db.SaveChanges();

                    SessionState.Current.CurrentAccountId = currentAccount.Id;
                    SessionState.Current.CurrentUserName = currentAccount.Company.Name;

                    return RedirectToAction("Index", "User");
                }
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Post action for Personal Bank Account Opening.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PersonalBankAccountOpening(PersonRegistrationViewModel model)
        {
            // Server side validations 
            // Telephone number validation
            bool validTelephoneNumber = IsValidTelephoneNumber(model.PersonalCommonData.TelephoneNumber);
            if (!validTelephoneNumber) { ModelState.AddModelError("Formátum hiba", "A telefonaszám nem megfelelő formátumú!"); }

            //Validation of Company Tax Number
            bool isValidIdentification = isValidIdentificationNumber(model.IdNumber);
            if (!isValidIdentification) { ModelState.AddModelError("Formátum hiba", "Hibás formátum!"); }

            try
            {
                if (ModelState.IsValid)
                {
                    // Hashing the password.
                    var salt = Crypto.GenerateSalt();
                    var savedPasswordHash = Crypto.SHA256(model.PersonalCommonData.Password + salt);

                    Person newPerson = new Person();
                    newPerson.FullName = model.PersonalCommonData.Name;
                    newPerson.IdNumber = model.IdNumber;
                    newPerson.MotherName = model.MotherName;
                    newPerson.TelephoneNumber = model.PersonalCommonData.TelephoneNumber;
                    newPerson.Address = model.PersonalCommonData.Address;
                    // Adding new Company to database
                    db.Person.Add(newPerson);

                    var currentAccount = db.CurrentAccount.FirstOrDefault(currAcc => currAcc.AccountNumber == model.PersonalCommonData.AccountNumber);
                    currentAccount.AccountOpenedDate = DateTime.Now;
                    currentAccount.Password = savedPasswordHash;
                    currentAccount.Salt = salt;

                    currentAccount.Person = newPerson;
                    var tokenManagerEntry = db.TokenManager.FirstOrDefault(token => token.TokenKey == currentAccount.TokenManagerTokenKey);

                    tokenManagerEntry.IsActive = false;
                    tokenManagerEntry.AccountCreated = currentAccount.AccountOpenedDate;
                    tokenManagerEntry.TokenExpiry = null;

                    db.Entry(currentAccount).State = EntityState.Modified;
                    db.Entry(tokenManagerEntry).State = EntityState.Modified;
                    db.SaveChanges();

                    SessionState.Current.CurrentAccountId = currentAccount.Id;
                    SessionState.Current.CurrentUserName = currentAccount.Person.FullName;

                    return RedirectToAction("Index", "User");
                }
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Validating telephone number with regex, returning bool value. Expected format is +0020/00-00-000
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsValidTelephoneNumber(string data)
        {
            try
            {
                bool validationResult = false;

                // Creating custom telephone number format +3620/12-34-567
                string pattern = @"\+(\d){4}/(\d){2}-(\d){2}-(\d){3}";

                Regex regexp = new Regex(pattern);
                //looking for match
                Match myMatch = regexp.Match(data);

                if (myMatch != null && myMatch.Success)
                {
                    validationResult = true;
                }
                return validationResult;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Validating company tax number according to this format: "12345678-1-23".
        /// </summary>
        /// <param name="TaxNumber"></param>
        /// <returns></returns>
        public bool IsValidTaxNumber(string TaxNumber)
        {
            try
            {
                bool isValidNumber = false;
                // Creating pattern for regex
                string pattern = @"(\d){8}-(\d)-(\d){2}";
                // Creating regex
                Regex regexp = new Regex(pattern);
                // Looking for matches
                Match match = regexp.Match(TaxNumber);
                // Evaluating result
                if (match != null && match.Success) { isValidNumber = true; }

                return isValidNumber;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Validating company registry number to this format "00-00-000000"
        /// </summary>
        /// <param name="RegistryNumber"></param>
        /// <returns></returns>
        public bool IsValidRegistryNumber(string RegistryNumber)
        {
            try
            {
                bool isValidNumber = false;
                // Creating pattern for regex
                string pattern = @"(\d){2}-(\d){2}-(\d){6}";
                // Creating regex
                Regex regexp = new Regex(pattern);
                // Looking for matches
                Match match = regexp.Match(RegistryNumber);
                // Evaluating result
                if (match != null && match.Success) { isValidNumber = true; }

                return isValidNumber;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Validating personal Identification Number according to this format "123456AB"
        /// </summary>
        /// <param name="IDNumber"></param>
        /// <returns></returns>
        public bool isValidIdentificationNumber(string IDNumber)
        {
            try
            {
                bool isValidIDNUmber = false;
                IDNumber = IDNumber.ToLower();
                // Creating pattern for regex
                string pattern = @"(\d){6}(\w){2}";
                // Creating regex
                Regex regexp = new Regex(pattern);
                // Looking for matches
                Match match = regexp.Match(IDNumber);
                // Evaluating result
                if (match != null && match.Success) { isValidIDNUmber = true; }
                return isValidIDNUmber;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}