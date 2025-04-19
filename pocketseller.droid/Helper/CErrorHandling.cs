using System;
using pocketseller.core.Resources.Languages;
using Exception = Java.Lang.Exception;

namespace pocketseller.droid.Helper
{
    public class CErrorHandling
    {
        public static void Log(Exception objException)
        {
            //MvxTrace.Trace(MvxTraceLevel.Error, GetErrorMessage(objException), objException);
        }

        public static void Log(System.Exception objException)
        {
            //MvxTrace.Trace(MvxTraceLevel.Error, GetErrorMessage(objException), objException);
        }

        public static void Log(string strTitle, string strMessage)
        {
            //MvxTrace.Trace(MvxTraceLevel.Error, strMessage);
        }

        public static void Log(Exception objException, bool bShowMessage)
        {
//            if (bShowMessage) ShowError(GetErrorMessage(objException));
//                MvxTrace.Trace(MvxTraceLevel.Error, objException.Message, objException);
        }

        public static void Log(System.Exception objException, bool bShowMessage)
        {
//                if (bShowMessage) ShowError(GetErrorMessage(objException));
//                    MvxTrace.Trace(MvxTraceLevel.Error, objException.Message, objException);
        }

        public static void Log(string strTitle, string strMessage, bool bShowMessage)
        {
//                if (bShowMessage) ShowError(strTitle, strMessage );
//                MvxTrace.Trace(MvxTraceLevel.Error, strMessage);
        }

        public static void ShowError(string strMessage)
        {
            CTools.ShowMessage(Language.Attention, strMessage);
        }

        public static void ShowError(string strTitle, string strMessage)
        {
            CTools.ShowMessage(strTitle, strMessage);
        }

        public static string GetErrorMessage(Exception objException)
        {
            var strMessage = string.Empty;

            if (objException == null)
                return strMessage;

            if (!string.IsNullOrEmpty(objException.Message))
            {
                strMessage += Environment.NewLine + objException?.Message;
                if (objException?.InnerException == null)
                    return strMessage;
            }

            if (objException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException == null)
                    return strMessage;
            }

            if (objException?.InnerException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException?.InnerException == null)
                    return strMessage;
            }

            if (objException?.InnerException?.InnerException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.InnerException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException?.InnerException?.InnerException == null)
                    return strMessage;
            }

            if (objException?.InnerException?.InnerException?.InnerException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.InnerException?.InnerException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException == null)
                    return strMessage;
            }

            if (objException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException == null)
                    return strMessage;
            }

            return strMessage;
        }

        public static string GetErrorMessage(System.Exception objException)
        {
            var strMessage = string.Empty;

            if (objException == null)
                return strMessage;

            if (!string.IsNullOrEmpty(objException?.Message))
            {
                strMessage += Environment.NewLine + objException?.Message;
                if (objException?.InnerException == null)
                    return strMessage;
            }

            if (objException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException == null)
                    return strMessage;                    
            }

            if (objException?.InnerException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException?.InnerException == null)
                    return strMessage;
            }

            if (objException?.InnerException?.InnerException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.InnerException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException?.InnerException?.InnerException == null)
                    return strMessage;
            }

            if (objException?.InnerException?.InnerException?.InnerException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.InnerException?.InnerException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException == null)
                    return strMessage;
            }

            if (objException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException != null)
            {
                strMessage += Environment.NewLine + objException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException?.Message;
                if (objException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException?.InnerException == null)
                    return strMessage;
            }

            return strMessage;
        }
    }
}