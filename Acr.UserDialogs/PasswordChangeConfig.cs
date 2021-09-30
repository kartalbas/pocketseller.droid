using System;


namespace Acr.UserDialogs
{

    public class PasswordChangeConfig : IStandardDialogConfig, IAndroidStyleDialogConfig
    {
        public static string DefaultTitle { get; set; } = "Login";
        public static string DefaultOkText { get; set; } = "Ok";
        public static string DefaultCancelText { get; set; } = "Cancel";
        public static string DefaultLoginPlaceholder { get; set; } = "User Name";
        public static string DefaultOldPasswordPlaceholder { get; set; } = "Password";
        public static string DefaultNewPasswordPlaceholder { get; set; } = "New Password";
        public static string DefaultNewConfirmPasswordPlaceholder { get; set; } = "New Password";
        public static int? DefaultAndroidStyleId { get; set; }

        public string Title { get; set; } = DefaultTitle;
        public string Message { get; set; }
        public string OkText { get; set; } = DefaultOkText;
        public string CancelText { get; set; } = DefaultCancelText;
        public string LoginValue { get; set; }
        public string LoginPlaceholder { get; set; } = DefaultLoginPlaceholder;
        public string OldPasswordPlaceholder { get; set; } = DefaultOldPasswordPlaceholder;
        public string NewPasswordPlaceholder { get; set; } = DefaultNewPasswordPlaceholder;
        public string NewPasswordConfirmPlaceholder { get; set; } = DefaultNewConfirmPasswordPlaceholder;
        public int? AndroidStyleId { get; set; } = DefaultAndroidStyleId;
        public Action<PasswordChangeResult> OnAction { get; set; }


        public PasswordChangeConfig SetTitle(string title)
        {
            this.Title = title;
            return this;
        }


        public PasswordChangeConfig SetMessage(string msg)
        {
            this.Message = msg;
            return this;
        }


        public PasswordChangeConfig SetOkText(string ok)
        {
            this.OkText = ok;
            return this;
        }


        public PasswordChangeConfig SetCancelText(string cancel)
        {
            this.CancelText = cancel;
            return this;
        }


        public PasswordChangeConfig SetLoginValue(string txt)
        {
            this.LoginValue = txt;
            return this;
        }


        public PasswordChangeConfig SetLoginPlaceholder(string txt)
        {
            this.LoginPlaceholder = txt;
            return this;
        }


        public PasswordChangeConfig SetOldPasswordPlaceholder(string txt)
        {
            this.OldPasswordPlaceholder = txt;
            return this;
        }

        public PasswordChangeConfig SetNewPasswordPlaceholder(string txt)
        {
            this.NewPasswordPlaceholder = txt;
            return this;
        }

        public PasswordChangeConfig SetNewPasswordConfirmPlaceholder(string txt)
        {
            this.NewPasswordConfirmPlaceholder = txt;
            return this;
        }


        public PasswordChangeConfig SetAction(Action<PasswordChangeResult> action)
        {
            this.OnAction = action;
            return this;
        }
    }
}
