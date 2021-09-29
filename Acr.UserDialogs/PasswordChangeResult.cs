namespace Acr.UserDialogs
{

    public class PasswordChangeResult : AbstractStandardDialogResult<Credentials>
    {
        public string LoginText => this.Value.UserName;
        public string OldPassword => this.Value.OldPassword;
        public string NewPassword => this.Value.NewPassword;


        public PasswordChangeResult(bool ok, string login, string oldPassword, string newPassword) : base(ok, new Credentials(login, oldPassword, newPassword))
        {
        }
    }
}
