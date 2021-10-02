using System;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding;
using MvvmCross.Platforms.Android.Binding.Target;

namespace pocketseller.droid.CustomBindings
{

    public class MvxEditViewFocusChange : MvxAndroidTargetBinding
    {
        protected EditText EditText => (EditText)Target;

        private bool _subscribed;

        public MvxEditViewFocusChange(EditText objView) : base(objView)
        {
        }

        protected override void SetValueImpl(object target, object value)
        {
            var objeditText = EditText;
            if (objeditText == null)
                return;

            value = value ?? string.Empty;
            objeditText.Text = value.ToString();
        }

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public override void SubscribeToEvents()
        {
            var objeditText = EditText;
            if (objeditText == null)
                return;

            objeditText.FocusChange += HandleFocusChange;
            _subscribed = true;
        }

        private void HandleFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            var objeditText = EditText;
            if (objeditText == null)
                return;

            if (!e.HasFocus)
                FireValueChanged(objeditText.Text);
        }

        public override Type TargetType => typeof(string);

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                var objeditText = EditText;
                if (objeditText != null && _subscribed)
                {
                    objeditText.FocusChange -= HandleFocusChange;
                    _subscribed = false;
                }
            }
            base.Dispose(isDisposing);
        }
    }
}
