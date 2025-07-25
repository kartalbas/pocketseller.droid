﻿using System;
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
    public class LoginBuilder : IAlertDialogBuilder<LoginConfig>
    {
        public Dialog Build(Activity activity, LoginConfig config)
        {
            var txtUser = new EditText(activity)
            {
                Hint = config.LoginPlaceholder,
                InputType = InputTypes.ClassText,
                Text = config.LoginValue ?? String.Empty
            };
            txtUser.SetSingleLine(true);

            var txtPass = new EditText(activity)
            {
                Hint = config.OldPasswordPlaceholder ?? "*"
            };
            txtPass.SetSingleLine(true);

            PromptBuilder.SetInputType(txtPass, InputType.Password);

            var layout = new LinearLayout(activity)
            {
                Orientation = Orientation.Vertical
            };
            txtPass.SetSingleLine(true);

            txtUser.SetMaxLines(1);
            txtPass.SetMaxLines(1);

            layout.AddView(txtUser, ViewGroup.LayoutParams.MatchParent);
            layout.AddView(txtPass, ViewGroup.LayoutParams.MatchParent);

            return new AlertDialog.Builder(activity, config.AndroidStyleId ?? 0)
                .SetCancelable(false)
                .SetTitle(config.Title)
                .SetMessage(config.Message)
                .SetView(layout)
                .SetPositiveButton(config.OkText, (s, a) =>
                    config.OnAction(new LoginResult(true, txtUser.Text, txtPass.Text))
                )
                .SetNegativeButton(config.CancelText, (s, a) =>
                    config.OnAction(new LoginResult(false, txtUser.Text, txtPass.Text))
                )
                .Create();
        }


        public Dialog Build(AppCompatActivity activity, LoginConfig config)
        {
            var txtUser = new EditText(activity)
            {
                Hint = config.LoginPlaceholder,
                InputType = InputTypes.ClassText,
                Text = config.LoginValue ?? String.Empty
            };
            txtUser.SetSingleLine(true);

            var txtPass = new EditText(activity)
            {
                Hint = config.OldPasswordPlaceholder ?? "*"
            };
            PromptBuilder.SetInputType(txtPass, InputType.Password);

            var layout = new LinearLayout(activity)
            {
                Orientation = Orientation.Vertical
            };

            txtUser.SetMaxLines(1);
            txtPass.SetMaxLines(1);

            layout.AddView(txtUser, ViewGroup.LayoutParams.MatchParent);
            layout.AddView(txtPass, ViewGroup.LayoutParams.MatchParent);

            return new AppCompatAlertDialog.Builder(activity, config.AndroidStyleId ?? 0)
                .SetCancelable(false)
                .SetTitle(config.Title)
                .SetMessage(config.Message)
                .SetView(layout)
                .SetPositiveButton(config.OkText, (s, a) =>
                    config.OnAction(new LoginResult(true, txtUser.Text, txtPass.Text))
                )
                .SetNegativeButton(config.CancelText, (s, a) =>
                    config.OnAction(new LoginResult(false, txtUser.Text, txtPass.Text))
                )
                .Create();
        }
    }
}
