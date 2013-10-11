using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CastleMaster.Errors
{
    public enum NamePreferences
    {
        CUSTOM, DATETIME
    }

    /// @author Denis Zhidkikh
    /// @version 28.7.2013
    /// 
    /// <summary>
    /// Error logging class
    /// </summary>
    public class MrsMarple
    {
        private const string ERROR_START = "BEGIN ERROR:";
        private const string ERROR_END = "END ERROR";
        private const string EXCEPTION_NAME = "Type of the exception: ";
        private const string ERROR_METHOD = "Came from method: ";
        private const string ERROR_MESSAGE = "Message for the developer: ";
        private const string TRACE = "Stack trace: ";
        private const string LOG_EXTENSION = ".log";

        /// <summary>
        /// Logs the exception's error into a file as a .log file.
        /// </summary>
        /// <param name="e">Exception to handle.</param>
        /// <param name="prefix">Starting message into the log file.</param>
        /// <param name="namePreference">Preference for log's name.</param>
        /// <param name="customName">If namePreference is set to CUSTOM, the name will be changed to this.</param>
        public static void LogError(Exception e, string prefix, NamePreferences namePreference, string customName = "")
        {
            string path = @"logs\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            StringBuilder errorMessage = new StringBuilder();
            string fileName = customName;
            if (namePreference == NamePreferences.DATETIME)
            {
                fileName = DateTime.Now.ToString("yyyy'-'MM'-'dd''HH'-'mm'-'ss");
            }

            string finalPath = new StringBuilder(path).Append(fileName).Append(LOG_EXTENSION).ToString();
            int id = 1;
            while (File.Exists(finalPath))
            {
                id++;
                finalPath = new StringBuilder(path).Append(fileName).Append("-").Append(id).Append(LOG_EXTENSION).ToString();
            }

            errorMessage.AppendLine(prefix);
            errorMessage.AppendLine(ERROR_START);
            errorMessage.Append(EXCEPTION_NAME).AppendLine(e.GetType().Name);
            errorMessage.Append(ERROR_METHOD).AppendLine(e.TargetSite.Name);
            errorMessage.Append(ERROR_MESSAGE).AppendLine(e.Message);
            errorMessage.AppendLine(TRACE).AppendLine(e.StackTrace);
            errorMessage.Append(ERROR_END);

            using (StreamWriter sw = File.CreateText(finalPath))
            {
                sw.Write(errorMessage.ToString());
            }
        }

        /// <summary>
        /// Creates a simple error message.
        /// </summary>
        /// <param name="message">Text of the error message.</param>
        public static void ThrowMessage(string message)
        {
            MessageBox.Show(message, Game.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Creates a error message and a .log file.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="e">Exception that caused the crash.</param>
        /// <param name="logFileNamePreference">Preference for naming .log file.</param>
        /// <param name="customFileName">Will be used to name the .log file if logFileNamePreference is CUSTOM.</param>
        public static void Error(string message, Exception e, NamePreferences logFileNamePreference, string customFileName = "")
        {
            ThrowMessage(message);
            LogError(e, "An error occurred on " + DateTime.Now.ToString("dd'-'MM'-'yyyy") + " at " + DateTime.Now.ToString("H:mm"), logFileNamePreference, customFileName);
        }
    }
}
