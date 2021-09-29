namespace Acr.UserDialogs
{
    public class Credentials
    {
        public Credentials(string userName, string oldPassword, string newPassword)
        {
            this.UserName = userName;
            this.OldPassword = oldPassword;
            this.NewPassword = oldPassword;
        }


        public string UserName { get; }
        public string OldPassword { get; }
        public string NewPassword { get; }
    }
}
