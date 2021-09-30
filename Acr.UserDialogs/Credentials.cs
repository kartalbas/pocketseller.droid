namespace Acr.UserDialogs
{
    public class Credentials
    {
        public Credentials(string userName, string oldPassword, string newPassword, string newPasswordConfirm)
        {
            this.UserName = userName;
            this.OldPassword = oldPassword;
            this.NewPassword = newPassword;
            this.NewPasswordConfirm = newPasswordConfirm;
        }


        public string UserName { get; }
        public string OldPassword { get; }
        public string NewPassword { get; }
        public string NewPasswordConfirm { get; }
    }
}
