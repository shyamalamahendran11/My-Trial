using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading;
using HPAssistEngine.HPDAL;


namespace HPAssistEngine.HPDAL
{
    /// <summary>
    /// Summary description for Logging.
    /// </summary>
    public class Logging
    {
        private static LogLevel currentLogLevel = LogLevel.Info;
        private const int LOG_MESSAGE_MAX_LENGTH = 7000;

        /// <summary>
        /// Log an info-level message
        /// </summary>
        /// <param name="source">Source class (normally use this.GetType)</param>
        /// <param name="title">Short title for log message</param>
        /// <param name="message">Longer message with details</param>
        public static void Info(string source, string title, string message)
        {
            Log(LogLevel.Info, source, title, message);
        }

        /// <summary>
        /// Log a debug-level message
        /// </summary>
        /// <param name="source">Source class (normally use this.GetType)</param>
        /// <param name="title">Short title for log message</param>
        /// <param name="message">Longer message with details</param>
        public static void Debug(string source, string title, string message)
        {
            Log(LogLevel.Debug, source, title, message);
        }

        /// <summary>
        /// Log a warning-level message
        /// </summary>
        /// <param name="source">Source class (normally use this.GetType)</param>
        /// <param name="title">Short title for log message</param>
        /// <param name="message">Longer message with details</param>
        public static void Warn(string source, string title, string message)
        {
            Log(LogLevel.Warn, source, title, message);
        }


        /// <summary>
        /// Log an error-level message
        /// </summary>
        /// <param name="source">Source class (normally use this.GetType)</param>
        /// <param name="title">Short title for log message</param>
        /// <param name="message">Longer message with details</param>
        public static void Error(string source, string title, string message)
        {
            Log(LogLevel.Error, source, title, message);
        }

        /// <summary>
        /// Log a critical error-level message
        /// </summary>
        /// <param name="source">Source class (normally use this.GetType)</param>
        /// <param name="title">Short title for log message</param>
        /// <param name="message">Longer message with details</param>
        public static void Critical(string source, string title, string message)
        {
            //Since many of these are critical system health booleans, wrap in try
            try
            {
                Log(LogLevel.Critical, source, title, message);
            }
            catch { }
        }

        /// <summary>
        /// Log without the aid of server and user names... get the info and then call the other log method
        /// </summary>
        /// <param name="level">Log levels: Info = 1, Debug = 2, Warn = 3, Error = 4</param>
        /// <param name="source">Source class (normally use this.GetType)</param>
        /// <param name="title">Short title for log message</param>
        /// <param name="message">Longer message with details</param>
        private static void Log(LogLevel level, string source, string title, string message)
        {
            string serverName = HttpContext.Current.Server.MachineName;
            string ipAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            string userName = null;
            string sessionID = "";

            userName = Thread.CurrentPrincipal.Identity.Name;
            Log(level, source, title, message, serverName, ipAddress, userName, sessionID);
        }

        private static void Log(LogLevel level, string source, string title, string message, string serverName, string ipAddress, string userName, string sessionId)
        {

            if (currentLogLevel > level)
            {
                return; // Do nothing.
            }

            DateTime LogDate = DateTime.Now;

            HP_Logging objLog = new HP_Logging();

            objLog.LogDateTime = LogDate;
            objLog.LogType = Convert.ToString(level);
            objLog.LogSource = Convert.ToString(source);
            objLog.LogTitle = title;
            objLog.LogMessage = message.Length > LOG_MESSAGE_MAX_LENGTH ? message.Substring(0, LOG_MESSAGE_MAX_LENGTH) : message;
            objLog.ServerName = serverName;
            objLog.UserName = userName;
            objLog.IPAddress = ipAddress;
            HPDAL.CommonDAL.InsertLogIntoDB(objLog);

        }

        public static string ErrLine(Exception ex, string prePend)
        {
            // split the stack on newlines
            string[] spliter = { "\r\n" };
            string stackTrace = ex.StackTrace;
            string[] splitStackTrace = stackTrace.Split(spliter, StringSplitOptions.None);

            // loop the array of strings looking for the one with ":line" (debug) info and return it
            foreach (string stackPointer in splitStackTrace)
            {
                // if we have a debugable line in the stack, return it
                if (stackPointer.Contains(":line"))
                {
                    // prepend & return "line of interest"
                    return prePend + stackPointer;
                }
            }

            // return zero-length string - no debug info
            return ""; //"--no error line data--";
        }

        /// <summary>
        /// Replace newlines with HTML breaks (<br>) for webpage layout
        /// </summary>
        /// <param name="ex">Exception to pull stack from</param>
        /// <param name="prePend">Prepend ex: "<p><b>Exception stack line of interest:</b><br>*&nbsp;"</param>
        /// <returns>StackTrace with HTML formatting</returns>
        /// <example>
        ///     // break up the stack with BR
        ///     Sb.Append(StackBreaker(exBase, "<p><b>Stack:</b><br>&nbsp;"));
        /// </example>
        public static string StackBreaker(Exception ex, string prePend)
        {
            string newLineStr = "\r\n"; // replace:newlines
            string breakStr = "<br />&nbsp;"; // with: HTML breaks, with leading whitespace
            string stackTrace = ex.StackTrace.Replace(newLineStr, breakStr); // reformat for HTML
            return prePend + stackTrace; // prepend & return HTML formatted StackTrace
        }

        /// <summary>
        /// Parse the exception to pull information most useful to debuggery
        /// </summary>
        /// <param name="ex">The exception to pull details from</param>
        /// <returns>HTML formatted exception details</returns>
        public static string GetErrorMessage(Exception ex)
        {
            StringBuilder Sb = new StringBuilder();
            try
            {
                // get the base exception if any
                Exception exBase = ex.GetBaseException();
                string htmlErrorMessage = "";
                //bool isDebug = false;
                string reqData = null;
                HttpRequest curRequest = System.Web.HttpContext.Current.Request;

                // start with the Exception:
                Sb.Append("<b>Message:</b><br>" + exBase.Message);

                // immediately add the 1-line from the stack where we think the stack occured:
                string errLine = ErrLine(exBase, "<p><b>Exception stack line of interest:</b><br>*&nbsp;");

                // if we have an error-line then we have some of the stack in debug
                if (errLine != null && errLine.Length > 0)
                {
                    //isDebug = true;

                    // only add this line if it has meaning
                    Sb.Append(errLine);
                }

                // KEEP: even in release-mode this usually has good information!
                // NOTE: reflection is used so both are slightly mangled but easily determined
                try
                {
                    // get the target of the crash site (class/method) from the base-exception
                    MethodBase crashMethod = exBase.TargetSite;
                    if (crashMethod != null)
                    {
                        // get class name with minor mangling: exBase.TargetSite.DeclaringType.Name
                        string className = crashMethod.DeclaringType.Name;
                        // get CRASH POINT: method name, return type, and parameter type(s) (function-signature): exBase.TargetSite.ToString()
                        string methodPrototype = Convert.ToString(crashMethod);

                        if ((className != null) && (methodPrototype != null))
                            Sb.Append("<p><b>TargetSite:</b> [<i>Class:</i>" + className + ", <i>Method:</i>" + methodPrototype + "]");
                    }
                }
                catch { /* do nothing */ }

                // KEEP: even in release-mode this usually has good information!
                // CRASH POINT: full file path with filename where the actual class' files reside
                try
                {
                    Sb.Append("<p><b>PhysicalPath:</b> " + curRequest.PhysicalPath);
                }
                catch { /* do nothing */ }

                // break up the stack with BR
                Sb.Append(StackBreaker(exBase, "<p><b>Stack:</b><br>&nbsp;"));

                Sb.Append("<p><b>Additional information:</b>");
                // get the UrlReferrer if any
                reqData = null;
                try
                {
                    if (curRequest.UrlReferrer != null)
                        reqData = Convert.ToString(curRequest.UrlReferrer);
                    else
                        reqData = Convert.ToString(curRequest.ServerVariables["HTTP_REFERER"]);

                    if ((reqData != null) && (reqData.Length > 0))
                        Sb.Append("<br>Referer: " + Convert.ToString(curRequest.UrlReferrer));
                }
                catch { /* do nothing */ }

                // get the QueryString if any
                reqData = null;
                try
                {
                    reqData = Convert.ToString(curRequest.QueryString);
                    if ((reqData != null) && (reqData.Length > 0))
                        Sb.Append("<br>QueryString: " + reqData);
                }
                catch { /* do nothing */ }

                // get the UserHostName (user's PC name) if any
                reqData = null;
                try
                {
                    reqData = curRequest.UserHostName;
                    if (reqData == null)
                        reqData = curRequest.ServerVariables["REMOTE_HOST"];
                    if (reqData != null)
                        Sb.Append("<br>Remote Host: " + reqData);
                }
                catch { /* do nothing */ }


                try
                {
                    Sb.Append("<br>Platform:" + curRequest.Browser.Platform);
                }
                catch { /* do nothing */ }

                // Browser ex: ",Browser:IE"
                try
                {
                    Sb.Append(",Browser:" + curRequest.Browser.Browser);
                }
                catch { /* do nothing */ }

                // Type ex: ",Type:IE7"
                try
                {
                    Sb.Append(",Type:" + curRequest.Browser.Type);
                }
                catch { /* do nothing */ }

                // Version ex: ",Version:7.0"
                try
                {
                    Sb.Append(",Version:" + curRequest.Browser.Version);
                }
                catch { /* do nothing */ }

                // (has)Cookies ex: ",Cookies:True"
                try
                {
                    Sb.Append(",Cookies:" + Convert.ToString(curRequest.Browser.Cookies));
                }
                catch { /* do nothing */ }

                // compress by removing duplicate whitespace:
                // take out multiple tabs(\t), html-blanks(&nbsp), and spaces( )
                htmlErrorMessage = Convert.ToString(Sb);
                if (htmlErrorMessage.Contains("\t"))
                {
                    // take out multiple tabs(\t)
                    htmlErrorMessage = Regex.Replace(htmlErrorMessage, "\t+", " ");
                }

                if (htmlErrorMessage.Contains("&nbsp;&nbsp;"))
                {
                    // take out multiple html-blanks(&nbsp)
                    htmlErrorMessage = Regex.Replace(htmlErrorMessage, "&nbsp;+", "&nbsp;");
                }

                if (htmlErrorMessage.Contains("  "))
                {
                    // take out multiple spaces
                    htmlErrorMessage = Regex.Replace(htmlErrorMessage, " +", " ");
                }

                // trim and return
                htmlErrorMessage = htmlErrorMessage.Trim();
                return htmlErrorMessage;
            }
            catch (Exception exEx)
            {
                // bad situation:
                Sb.Append("<br>Got exception processing exception. Message:" + Convert.ToString(exEx));
            }

            // return the string
            return Convert.ToString(Sb);
        }

        /// <summary>
        /// Parse the exception to pull information most useful to debuggery and log to the database
        /// </summary>
        /// <param name="outerException">The exception to pull details from</param>
        public static void LogException(Exception outerException)
        {
            Exception exceptionBase = null;

            // get the inner exception if any
            if (outerException.GetBaseException() != null)
                exceptionBase = outerException.GetBaseException();
            else
                exceptionBase = outerException;

            //LogException(outerException, exceptionBase.GetType());   ------ME


        }

        /// <summary>
        /// Parse the exception to pull information most useful to debuggery and log to the database
        /// </summary>
        /// <param name="outerException">The exception to pull details from</param>
        /// <param name="source">Type typically from this.GetType() - defaults to the exception type if not specified</param>
        public static void LogException(Exception outerException, string source)
        {
            LogException(outerException, source, "Application_Error");
        }

        /// <summary>
        /// Parse the exception to pull information most useful to debuggery and log to the database
        /// </summary>
        /// <param name="ex">The exception to pull details from</param>
        /// <param name="title">Short title for log message</param>
        //public static void LogException(Exception ex, string title)
        //{
        //    LogException(ex, ex.GetType(), title);
        //}

        /// <summary>
        /// Parse the exception to pull information most useful to debuggery and log to the database
        /// </summary>
        /// <param name="ex">The exception to pull details from</param>
        /// <param name="source">Type typically from this.GetType() - defaults to the exception type if not specified</param>
        /// <param name="title">Short title for log message</param>
        /// 

        public static void LogException(Exception ex, string source, string title)
        {
            Exception exceptionBase = null;

            // get the inner exception if any
            if (ex.GetBaseException() != null)
                exceptionBase = ex.GetBaseException();
            else
                exceptionBase = ex;

            // Parse the exception to pull information most useful to debuggery
            string errorMessage = Logging.GetErrorMessage(ex);

            try
            {

                // if we have something to log
                if (errorMessage != null)
                    // instead of this.GetType use lastErrorBase.GetType
                    Logging.Error(source, title, errorMessage);
                else
                    // just use the outerException
                    Logging.Error(source, title, Convert.ToString(ex));
            }
            catch //( Exception ex )
            {
                // TODO: attach to a trace listener?
            }
        }
    }


    public class HP_Logging
    {
        public DateTime LogDateTime { get; set; }
        public string LogType { get; set; }
        public string LogSource { get; set; }
        public string LogTitle { get; set; }
        public string LogMessage { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string IPAddress { get; set; }
    }
    public enum LogLevel
    {
        Info = 1,
        Debug = 2,
        Warn = 3,
        Error = 4,
        Critical = 5
    }
}
