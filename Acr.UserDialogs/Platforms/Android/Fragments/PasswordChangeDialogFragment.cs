using Acr.UserDialogs.Builders;
using Android.App;


namespace Acr.UserDialogs.Fragments
{
    public class PasswordChangeDialogFragment : AbstractAppCompatDialogFragment<PasswordChangeConfig>
    {
        //protected override void OnKeyPress(object sender, DialogKeyEventArgs args)
        //{
        //    base.OnKeyPress(sender, args);
        //    if (args.KeyCode != Keycode.Back)
        //    {
        //        args.Handled = false;
        //        return;
        //    }
        //    args.Handled = true;
        //    this.Config?.OnAction(new LoginResult(false, null, null));
        //    this.Dismiss();
        //}


        protected override Dialog CreateDialog(PasswordChangeConfig config)
        {
            return new PasswordChangeBuilder().Build(this.AppCompatActivity, config);
        }
    }
}