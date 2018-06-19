using NetBank.Testability;
using System;
using System.Collections.Generic;

namespace NetBank
{
    /// <summary>
    /// Session state.
    /// </summary>
    public class SessionState
    {
        public Nullable<int> CurrentAccountId { get; set; }
        public string CurrentUserName { get; set; }

        public SessionState()
        {
            // Generate new session id
            SessionId = Guid.NewGuid();
            Data = new Dictionary<string, object>();
            //Messages = new List<SimpleMessageModel>();            
        }

        /// <summary>
        /// The current SessionState instance.
        /// </summary>
        public static SessionState Current
        {
            get
            {
                return HttpContextFacade.SessionState;
            }
            set
            {
                HttpContextFacade.SessionState = value;
            }
        }

        /// <summary>
        /// Miscellaneous data
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Session identifier.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// The messages to be displayed for the user.
        /// </summary>
        //public List<SimpleMessageModel> Messages { get; set; }        

        /// <summary>
        /// Initializes the session state
        /// </summary>
        public static void InitSessionState()
        {
            Current = new SessionState();
        }
    }
}