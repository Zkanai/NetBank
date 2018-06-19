using System;
using System.Web;

namespace NetBank.Testability
{
    /// <summary>
    /// This class wraps the HttpContext object and lets unit tests to change that by using SetContext
    /// </summary>
    public static class HttpContextFacade
    {
        /// <summary>
        /// Default getter sets HttpContext.Current as managed HttpContext
        /// </summary>
        private static Func<HttpContextBase> _getter = () => new HttpContextWrapper(HttpContext.Current);

        /// <summary>
        /// wrapped HttpContext object
        /// </summary>
        public static HttpContextBase CurrentHttpContext => _getter();

        /// <summary>
        /// Use this method if you want to mock HttpContext
        /// </summary>
        /// <param name="getter"></param>
        public static void SetContext(Func<HttpContextBase> getter)
        {
            _getter = getter;
        }

        /// <summary>
        /// Property for accessing SessionState object
        /// </summary>
        public static SessionState SessionState
        {
            get
            {
                return (SessionState)CurrentHttpContext?.Session?["SessionState"];
            }
            set
            {
                if (CurrentHttpContext?.Session != null)
                    CurrentHttpContext.Session["SessionState"] = value;
            }
        }

        /// <summary>
        /// Property for accessing ApplicationState object
        /// </summary>
        public static ApplicationState ApplicationState
        {
            get
            {
                return (ApplicationState)CurrentHttpContext?.Application?["ApplicationState"];
            }
            set
            {
                if (CurrentHttpContext?.Application != null)
                    CurrentHttpContext.Application["ApplicationState"] = value;
            }
        }

        /// <summary>
        /// Property for Last exception stored in the session
        /// </summary>
        public static Exception LastException
        {
            get
            {
                return (Exception)CurrentHttpContext?.Session?["LastException"];
            }
            set
            {
                if (CurrentHttpContext?.Session != null)
                    CurrentHttpContext.Session["LastException"] = value;
            }
        }
    }
}