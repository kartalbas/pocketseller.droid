using System;
using Android.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using AlertDialog = Android.App.AlertDialog;
#if ANDROIDX
using AndroidX.AppCompat.App;
using AppCompatAlertDialog = AndroidX.AppCompat.App.AlertDialog;
#else
using Android.Support.V7.App;
using AppCompatAlertDialog = Android.Support.V7.App.AlertDialog;
#endif


namespace Acr.UserDialogs.Builders
{
    public class PasswordChangeBuilder : IAlertDialogBuilder<PasswordChangeConfig>
    {
        public Dialog Build(Activity activity, PasswordChangeConfig config)
        {
            var txtUser = new EditText(activity)
            {
                Hint = config.LoginPlaceholder,
                InputType = InputTypes.ClassText,
                Text = config.LoginValue ?? String.Empty
            };
            txtUser.SetSingleLine(true);

            var txtOldPass = new EditText(activity)
            {
                Hint = config.OldPasswordPlaceholder ?? "*"
            };
            txtOldPass.SetSingleLine(true);

            var txtNewPass = new EditText(activity)
            {
                Hint = config.NewPasswordPlaceholder ?? "*"
            };
            txtNewPass.SetSingleLine(true);

            var txtNewPassConfirm = new EditText(activity)
            {
                Hint = config.NewPasswordConfirmPlaceholder ?? "*"
            };
            txtNewPassConfirm.SetSingleLine(true);

            PromptBuilder.SetInputType(txtOldPass, InputType.Password);
            PromptBuilder.SetInputType(txtNewPass, InputType.Password);
            PromptBuilder.SetInputType(txtNewPassConfirm, InputType.Password);

            var layout = new LinearLayout(activity)
            {
                Orientation = Orientation.Vertical
            };

            txtUser.SetMaxLines(1);
            txtOldPass.SetMaxLines(1);

            layout.AddView(txtUser, ViewGroup.LayoutParams.MatchParent);
            layout.AddView(txtOldPass, ViewGroup.LayoutParams.MatchParent);
            layout.AddView(txtNewPass, ViewGroup.LayoutParams.MatchParent);
            layout.AddView(txtNewPassConfirm, ViewGroup.LayoutParams.MatchParent);

            return new AlertDialog.Builder(activity, config.AndroidStyleId ?? 0)
                .SetCancelable(false)
                .SetTitle(config.Title)
                .SetMessage(config.Message)
                .SetView(layout)
                .SetPositiveButton(config.OkText, (s, a) =>
                    config.OnAction(new PasswordChangeResult(true, txtUser.Text, txtOldPass.Text, txtNewPass.Text, txtNewPassConfirm.Text))
                )
                .SetNegativeButton(config.CancelText, (s, a) =>
                    config.OnAction(new PasswordChangeResult(false, txtUser.Text, txtOldPass.Text, txtNewPass.Text, txtNewPassConfirm.Text))
                )
                .Create();
        }


        public Dialog Build(AppCompatActivity activity, PasswordChangeConfig config)
        {
            var txtUser = new EditText(activity)
            {
                Hint = config.LoginPlaceholder,
                InputType = InputTypes.ClassText,
                Text = config.LoginValue ?? String.Empty
            };
            txtUser.SetSingleLine(true);

            var txtOldPass = new EditText(activity)
            {
                Hint = config.OldPasswordPlaceholder ?? "*"
            };
            txtOldPass.SetSingleLine(true);

            var txtNewPass = new EditText(activity)
            {
                Hint = config.NewPasswordPlaceholder ?? "*"
            };
            txtNewPass.SetSingleLine(true);

            var txtNewPassConfirm = new EditText(activity)
            {
                Hint = config.NewPasswordConfirmPlaceholder ?? "*"
            };
            txtNewPassConfirm.SetSingleLine(true);

            PromptBuilder.SetInputType(txtOldPass, InputType.Password);
            PromptBuilder.SetInputType(txtNewPass, InputType.Password);
            PromptBuilder.SetInputType(txtNewPassConfirm, InputType.Password);

            var layout = new LinearLayout(activity)
            {
                Orientation = Orientation.Vertical
            };

            txtUser.SetMaxLines(1);
            txtOldPass.SetMaxLines(1);

            layout.AddView(txtUser, ViewGroup.LayoutParams.MatchParent);
            layout.AddView(txtOldPass, ViewGroup.LayoutParams.MatchParent);
            layout.AddView(txtNewPass, ViewGroup.LayoutParams.MatchParent);
            layout.AddView(txtNewPassConfirm, ViewGroup.LayoutParams.MatchParent);

            return new AppCompatAlertDialog.Builder(activity, config.AndroidStyleId ?? 0)
                .SetCancelable(false)
                .SetTitle(config.Title)
                .SetMessage(config.Message)
                .SetView(layout)
                .SetPositiveButton(config.OkText, (s, a) =>
                    config.OnAction(new PasswordChangeResult(true, txtUser.Text, txtOldPass.Text, txtNewPass.Text, txtNewPassConfirm.Text))
                )
                .SetNegativeButton(config.CancelText, (s, a) =>
                    config.OnAction(new PasswordChangeResult(false, txtUser.Text, txtOldPass.Text, txtNewPass.Text, txtNewPassConfirm.Text))
                )
                .Create();
        }
    }
}
