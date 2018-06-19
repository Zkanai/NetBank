using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using NetBank.Testability;

namespace NetBank
{
    public class ApplicationState
    {
        /// <summary>
        /// The current instance of the application state.
        /// </summary>
        public static ApplicationState Current
        {
            get
            {
                return HttpContextFacade.ApplicationState;
            }
            private set
            {
                HttpContextFacade.ApplicationState = value;
            }
        }

        /// <summary>
        /// Application szintû cache
        /// </summary>
        public Dictionary<string, object> Cache { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Folder path of Exercise attachments
        /// </summary>
        public string ExerciseAttachmentUploadFolder { get; set; }

        /// <summary>
        /// Folder path of Student attachments
        /// </summary>
        public string StudentAttachmentUploadFolder { get; set; }

        /// <summary>
        /// Folder path of Student attachments
        /// </summary>
        public string ExcelFileExportFolder { get; set; }
        


        /// <summary>
        /// Initialization of Application state
        /// </summary>
        public static void Initialize()
        {
            Current = new ApplicationState();
        }
       
    }
}