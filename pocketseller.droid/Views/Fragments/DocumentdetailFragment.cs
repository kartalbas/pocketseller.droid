using System;
using System.Globalization;
using System.Threading;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using ThreadPriority = System.Threading.ThreadPriority;

namespace pocketseller.droid.Views.Fragments
{
    [MvxDialogFragmentPresentation]
    [Register(nameof(DocumentdetailFragment))]
    public sealed class DocumentdetailFragment : MvxDialogFragment<DocumentdetailViewModel>
    {
        #region Member variables

        public DocumentdetailViewModel DocumentdetailViewModel => ViewModel;

        private enum EEditPosition { Count, Amount, Price }
        private EEditPosition _enmEditPosition;

        private EditText _objCount;
        private EditText _objAmount;
        private EditText _objPrice;

        private TextView _objArticleLable;

        private ImageButton _objNextButton;
        private ImageButton _objPreviousButton;

        private LinearLayout _objLinearLayoutInput;
        private LinearLayout _objLinearLayoutInformation1;
        private LinearLayout _objLinearLayoutInformation2;

        private decimal _djCountBackup;
        private decimal _djAmountBackup;
        private decimal _djPriceBackup;

        private MvxSubscriptionToken _objToken;

        private string LogTag;

        #endregion

        #region Public methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var objDialogView = this.BindingInflate(pocketseller.droid.Resource.Layout.DocumentdetailFragment, null);

            LogTag = GetType().ToString();

            var objSingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.DocumentdetailViewModel == null)
                objSingletonService.DocumentdetailViewModel = CMvvmCrossTools.LoadViewModel<DocumentdetailViewModel>();

            ViewModel = objSingletonService.DocumentdetailViewModel;

            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            RegisterActions(objDialogView);

            _objToken = Mvx.IoCProvider.Resolve<IMvxMessenger>().SubscribeOnMainThread<DocumentdetailMessage>(OnDocumentdetailAction);

            var showKeyboardThread = new Thread(ShowKeyboard) { IsBackground = true, Priority = ThreadPriority.Normal };
            showKeyboardThread.Start();
            
            return objDialogView;
        }

        private void ShowKeyboard()
        {
            Thread.Sleep(300);
            Mvx.IoCProvider.Resolve<IBasicPlatformService>().ShowKeyboard();
        }

        #endregion

        #region Lifecycle events

        public override void OnStart()
        {
            base.OnStart();

            if (Dialog != null)
            {
                var objAttributs = Dialog.Window.Attributes;
                objAttributs.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
                objAttributs.Y = 200;
                Dialog.Window.SetWindowAnimations(pocketseller.droid.Resource.Style.DialogAnimation);
            }

            BackupValues();
            SetFocus(EEditPosition.Price);
            SetFocus(EEditPosition.Count);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Mvx.IoCProvider.Resolve<IMvxMessenger>().Unsubscribe<DocumentdetailMessage>(_objToken);
            _objToken.Dispose();
            _objToken = null;

            UnregisterActions();
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);
            RestoreAndExit();
        }

        #endregion

        #region Private methods

        private void OnDocumentdetailAction(DocumentdetailMessage objDocumentdetailMessage)
        {
            switch (objDocumentdetailMessage.EDocumentdetailAction)
            {
                case EDocumentdetailAction.Exit:
                    Dismiss();
                    break;
                case EDocumentdetailAction.Stay:
                    break;
            }
        }

        private void RestoreAndExit()
        {
            RestoreValues();
            Dismiss();
        }

        private void BackupValues()
        {
            _djCountBackup = DocumentdetailViewModel.TextCount;
            _djAmountBackup = DocumentdetailViewModel.TextAmount;
            _djPriceBackup = DocumentdetailViewModel.TextPrice;
        }

        private void RestoreValues()
        {
            DocumentdetailViewModel.TextCount = _djCountBackup;
            DocumentdetailViewModel.TextAmount = _djAmountBackup;
            DocumentdetailViewModel.TextPrice = _djPriceBackup;
        }

        private void RegisterActions(View objView)
        {
            _objArticleLable = objView.FindViewById<TextView>(pocketseller.droid.Resource.Id.documentdetail_articlelable);
            _objArticleLable.LongClick += ArticleLableOnLongClick;
            _objArticleLable.Click += ArticleLableOnClick;

            _objLinearLayoutInput = objView.FindViewById<LinearLayout>(pocketseller.droid.Resource.Id.documentdetail_linearlayout_input);
            _objLinearLayoutInformation1 = objView.FindViewById<LinearLayout>(pocketseller.droid.Resource.Id.documentdetail_linearlayout_lastinformations1);
            _objLinearLayoutInformation2 = objView.FindViewById<LinearLayout>(pocketseller.droid.Resource.Id.documentdetail_linearlayout_lastinformations2);

            _objCount = objView.FindViewById<EditText>(pocketseller.droid.Resource.Id.documentdetail_edittext_count);
            _objCount.FocusChange += CountOnFocusChange;
            _objCount.EditorAction += OnEditorAction;
            _objCount.InputType = CTools.GetInputType((int)DocumentdetailViewModel.KeyboardSetting);

            _objAmount = objView.FindViewById<EditText>(pocketseller.droid.Resource.Id.documentdetail_edittext_amount);
            _objAmount.FocusChange += AmountOnFocusChange;
            _objAmount.EditorAction += OnEditorAction;
            _objAmount.InputType = CTools.GetInputType((int)DocumentdetailViewModel.KeyboardSetting);

            _objPrice = objView.FindViewById<EditText>(pocketseller.droid.Resource.Id.documentdetail_edittext_price);
            _objPrice.FocusChange += PriceOnFocusChange;
            _objPrice.EditorAction += OnEditorAction;
            _objPrice.InputType = CTools.GetInputType((int)DocumentdetailViewModel.KeyboardSetting);

            _objNextButton = objView.FindViewById<ImageButton>(pocketseller.droid.Resource.Id.documentdetail_imagebutton_next);
            _objNextButton.Click += NextOnClick;

            _objPreviousButton = objView.FindViewById<ImageButton>(pocketseller.droid.Resource.Id.documentdetail_imagebutton_previous);
            _objPreviousButton.Click += PreviousOnClick;
        }

        private void OnEditorAction(object objSender, TextView.EditorActionEventArgs objArgs)
        {
            if (objArgs.ActionId == ImeAction.Done || objArgs.ActionId == ImeAction.Go | objArgs.ActionId == ImeAction.Next)
            {
                NavigateForward();
                objArgs.Handled = false;
            }
        }

        private void ArticleLableOnClick(object sender, EventArgs eventArgs)
        {
            _objLinearLayoutInput.Visibility = ViewStates.Visible;
            _objLinearLayoutInformation1.Visibility = ViewStates.Visible;
            _objLinearLayoutInformation2.Visibility = ViewStates.Visible;
        }

        private void ArticleLableOnLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            //TODO: Show Stock as dialog in a listview
            _objLinearLayoutInput.Visibility = ViewStates.Gone;
            _objLinearLayoutInformation1.Visibility = ViewStates.Gone;
            _objLinearLayoutInformation2.Visibility = ViewStates.Gone;
        }

        private void UnregisterActions()
        {
            _objCount.EditorAction -= OnEditorAction;
            _objAmount.EditorAction -= OnEditorAction;
            _objPrice.EditorAction -= OnEditorAction;

            _objCount.FocusChange -= CountOnFocusChange;
            _objAmount.FocusChange -= AmountOnFocusChange;
            _objPrice.FocusChange -= PriceOnFocusChange;

            _objNextButton.Click -= NextOnClick;
            _objPreviousButton.Click -= PreviousOnClick;

            _objCount = null;
            _objAmount = null;
            _objPrice = null;
            _objNextButton = null;
            _objPreviousButton = null;
        }

        private void NextOnClick(object sender, EventArgs eventArgs)
        {
            NavigateForward();
        }

        private void NavigateForward()
        {
            switch (_enmEditPosition)
            {
                case EEditPosition.Count:
                    if (GetEditValue(_objCount) == 0)
                        SetFocus(EEditPosition.Amount);
                    else
                        SetFocus(EEditPosition.Price);
                    break;
                case EEditPosition.Amount:
                    SetFocus(EEditPosition.Price);
                    break;
                case EEditPosition.Price:
                    SetPriceValueToModel();
                    DocumentdetailViewModel.AddDocumentdetailCommand.Execute(null);
                    break;
            }            
        }

        private void PreviousOnClick(object sender, EventArgs eventArgs)
        {
            NavigateBackward();
        }

        private void NavigateBackward()
        {
            switch (_enmEditPosition)
            {
                case EEditPosition.Count:
                    break;
                case EEditPosition.Amount:
                    SetFocus(EEditPosition.Count);
                    break;
                case EEditPosition.Price:
                    if (GetEditValue(_objCount) == 0)
                        SetFocus(EEditPosition.Amount);
                    else
                        SetFocus(EEditPosition.Count);
                    break;
            }
        }

        private void SetFocus(EEditPosition enmPosition)
        {
            switch (enmPosition)
            {
                case EEditPosition.Count:
                    _objCount.RequestFocus();
                    Mvx.IoCProvider.Resolve<IBasicPlatformService>().ShowKeyboard();
                    break;
                case EEditPosition.Amount:
                    _objAmount.RequestFocus();
                    Mvx.IoCProvider.Resolve<IBasicPlatformService>().ShowKeyboard();
                    break;
                case EEditPosition.Price:
                    _objPrice.RequestFocus();
                    Mvx.IoCProvider.Resolve<IBasicPlatformService>().ShowKeyboard();
                    break;
            }
        }

        private void CountOnFocusChange(object sender, View.FocusChangeEventArgs focusChangeEventArgs)
        {
            if(_enmEditPosition ==  EEditPosition.Count)
                return;

            var objEdit = sender as EditText;
            if (objEdit == null) return;
            _enmEditPosition = EEditPosition.Count;
           
            objEdit.SelectAll();
        }

        private void AmountOnFocusChange(object sender, View.FocusChangeEventArgs focusChangeEventArgs)
        {
            if (_enmEditPosition == EEditPosition.Amount)
                return;

            var objEdit = sender as EditText;
            if (objEdit == null) return;
            _enmEditPosition = EEditPosition.Amount;

            objEdit.SelectAll();
        }

        private void PriceOnFocusChange(object sender, View.FocusChangeEventArgs focusChangeEventArgs)
        {
            if (_enmEditPosition == EEditPosition.Price)
                return;

            var objEdit = sender as EditText;
            if (objEdit == null) return;
            _enmEditPosition = EEditPosition.Price;

            GetAmountValueFromModel();
            
            objEdit.SelectAll();
        }

        private void GetAmountValueFromModel()
        {
            //Need to update manually because it might not got a focus, in this case it was not binded
            _objAmount.Text = DocumentdetailViewModel.TextAmount.ToString(CultureInfo.InvariantCulture);
        }

        private void SetPriceValueToModel()
        {
            //Need to update manually - not binded when foucs lost
            DocumentdetailViewModel.TextPrice = GetEditValue(_objPrice);
        }

        private decimal GetEditValue(EditText objEditText)
        {
            return objEditText == null 
                ? 0 
                : CParser.SafeParseDecimal(objEditText.Text);
        }

        #endregion

    }
}