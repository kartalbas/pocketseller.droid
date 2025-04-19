namespace Acr.UserDialogs
{

    public class PasswordChangeResult : AbstractStandardDialogResult<Credentials>
    {
        public string LoginText => this.Value.UserName;
        public string OldPassword => this.Value.OldPassword;
        public string NewPassword => this.Value.NewPassword;
        public string NewPasswordConfirm => this.Value.NewPasswordConfirm;


        public PasswordChangeResult(bool ok, string login, string oldPassword, string newPassword, string newPasswordConfirm) : base(ok, new Credentials(login, oldPassword, newPassword, newPasswordConfirm))
        {
        }
    }
}
