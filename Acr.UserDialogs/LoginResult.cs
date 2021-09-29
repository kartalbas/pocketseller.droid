namespace Acr.UserDialogs
{

    public class LoginResult : AbstractStandardDialogResult<Credentials>
    {
        public string LoginText => this.Value.UserName;
        public string Password => this.Value.OldPassword;


        public LoginResult(bool ok, string login, string password) : base(ok, new Credentials(login, password, string.Empty))
        {
        }
    }
}
