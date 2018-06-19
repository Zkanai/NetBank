using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NetBank.Models
{
    /// <summary>
    /// This class is for the User login and sending email if the User wants to Signup.
    /// </summary>
    public class HomeViewModel
    {
        public Nullable<int> Id { get; set; }
        // These are the necessary parameteres for sending an email.


        
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        [Display(Name = "E-mail címe")]
        public string Email { get; set; }
        public string Password { get; set; }

        // Name for message sent.
        [Display(Name = "Neve")]
        public string Name { get; set; }
        // Result Message to be displayed on the view if necessary.
        public string ResultMessage { get; set; }
    }
}