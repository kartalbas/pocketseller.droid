using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using pocketseller.droid.Views.Fragments;
using Quotationdetail = pocketseller.core.Models.Quotationdetail;

namespace pocketseller.droid.Views
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/Theme.Pocketsellertheme1", WindowSoftInputMode = SoftInput.AdjustResize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class QuotationView : MvxFragmentActivity
    {
        #region Private properties

        private QuotationViewModel QuotationViewModel => (ViewModel as QuotationViewModel);
        private MvxDialogFragment _objDialogFragment;
        private MvxSubscriptionToken _objTokenDocument;
        private MvxListView _objListView;
        private TextView _objTextViewStart;
        private TextView _objTextViewStop;
        private const int DATE_DIALOG_START_ID = 0;
        private const int DATE_DIALOG_STOP_ID = 1;

        #endregion

        #region Overridden methods

        protected override void OnCreate(Bundle objInState)
        {
            base.OnCreate(objInState);
            SetContentView(orderline.droid.Resource.Layout.QuotationView);

            QuotationViewModel.SetDefaultDatesCommand.Execute(null);

            //Init Autocomplete
            var objAutoCompleteTextView = FindViewById<MvxAutoCompleteTextView>(orderline.droid.Resource.Id.selectionarticlesearchbox);
            objAutoCompleteTextView.InputType = CTools.GetInputType((int)QuotationViewModel.KeyboardSetting);
            objAutoCompleteTextView.RequestFocus();

            //Init ListView
            _objListView = FindViewById<MvxListView>(orderline.droid.Resource.Id.quotation_listview);
            _objListView.SmoothScrollbarEnabled = true;
            _objListView.SmoothScrollBy(1, 1);
            RegisterForContextMenu(_objListView);

            //Init DateTime pickers
            _objTextViewStart = FindViewById<TextView>(orderline.droid.Resource.Id.quotation_startdatetime);
            _objTextViewStart.Click += (sender, args) => ShowDialog(DATE_DIALOG_START_ID);
            _objTextViewStop = FindViewById<TextView>(orderline.droid.Resource.Id.quotation_stopdatetime);
            _objTextViewStop.Click += (sender, args) => ShowDialog(DATE_DIALOG_STOP_ID);

            CTools.InitActionBar(ActionBar, QuotationViewModel.LabelTitle);

            PauseCalled += (sender, e) => UnSubscribeMessenger();

            SubscribeMessenger();
        }

        #region Lifecycle events


        protected override void OnRestart()
        {
            SubscribeMessenger();
            base.OnRestart();
        }

        #endregion

        protected override Dialog OnCreateDialog(int id)
        {
            switch(id)
            {
                case DATE_DIALOG_START_ID:
                    return new DatePickerDialog(this, OnDateSetStart, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                case DATE_DIALOG_STOP_ID:
                    return new DatePickerDialog(this, OnDateSetStop, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1);
            }
            return base.OnCreateDialog(id);
        }

        private void OnDateSetStart(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var objDate = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, 0, 0, 0);
            if (objDate <= QuotationViewModel.Quotation.StopDateTime)
            {
                QuotationViewModel.Quotation.StartDateTime = objDate;
                QuotationViewModel.DocumentState = EOrderState.CHANGED;
            }
            else
            {
                CTools.ShowToast(Language.SetDateLessEqualStop);
            }
        }

        private void OnDateSetStop(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var objDate = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, 0, 0, 0);
            if (objDate >= QuotationViewModel.Quotation.StartDateTime)
            {
                QuotationViewModel.Quotation.StopDateTime = objDate;
                QuotationViewModel.DocumentState = EOrderState.CHANGED;
            }
            else
            {
                CTools.ShowToast(Language.SetDateGreaterEqualStart);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu objMenu)
        {
            objMenu.Clear();

            objMenu.Add("")
                .SetIcon(orderline.droid.Resource.Drawable.ic_action_cancel_dark)
                .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnNewActionClicked))
                .SetShowAsAction(ShowAsAction.Always);

            return true;
        }

        private bool OnNewActionClicked(IMenuItem objMenuItem)
        {
            switch (objMenuItem.ItemId)
            {
                case (int)EOptionMenu.Exit:
                    OnBackPressed();
                    break;
            }

            return true;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(QuotationViewModel.LabelEdit);
            objMenu.Add(QuotationViewModel.LabelDelete);
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objDocumentdetail = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Quotationdetail;

            if (strSelectedAction == QuotationViewModel.LabelEdit)
            {
                QuotationViewModel.PositionEditCommand.Execute(objDocumentdetail);
            }
            else if (strSelectedAction == QuotationViewModel.LabelDelete)
            {
                QuotationViewModel.PositionDeleteCommand.Execute(objDocumentdetail);
            }

            return base.OnContextItemSelected(objItem);
        }

        public override void OnBackPressed()
        {
            if (QuotationViewModel.DocumentState == EOrderState.CHANGED)
            {
                CTools.ShowDialog(this
                    , Language.Exit
                    , Language.ExitMessageHow
                    , Language.SaveAndExit
                    , (sender, args) =>
                    {
                        SaveDocument();
                        DiscardDocument();
                        Finish();
                        base.OnBackPressed();
                    }
                    , Language.ExitWithoutSave
                    , (sender, args) =>
                    {
                        DiscardDocument();
                        Finish();
                        base.OnBackPressed();
                    }
                    , string.Empty
                    , null);
            }
            else
            {
                CTools.ShowDialog(this
                    , Language.Exit
                    , Language.ExitMessage
                    , Language.Exit
                    , (sender, args) =>
                    {
                        DiscardDocument();
                        base.OnBackPressed();
                    }
                    , Language.Stay
                    , (sender, args) =>
                    {
                    }
                    , string.Empty
                    , null);
            }
        }

        #endregion

        #region Private methods

        private void SubscribeMessenger()
        {
            _objTokenDocument = Mvx.IoCProvider.Resolve<IMvxMessenger>().SubscribeOnMainThread<DocumentMessage>(OnDocumentAction);
        }

        private void UnSubscribeMessenger()
        {
            Mvx.IoCProvider.Resolve<IMvxMessenger>().Unsubscribe<DocumentMessage>(_objTokenDocument);
            _objTokenDocument.Dispose();
            _objTokenDocument = null;
        }

        private void OnDocumentAction(DocumentMessage objDocumentMessage)
        {
            switch (objDocumentMessage.EDocumentAction)
            {
                case EDocumentAction.ShowDocumentDetail:
                    ShowFragment(EMenu.Documentdetail);
                    break;
            }
        }

        private void SaveDocument()
        {
            if (!QuotationViewModel.IsSavable)
            {
                CTools.ShowToast(Language.QuotationNotSaved);
            }
            else
            {
                QuotationViewModel.SaveDocumentCommand.Execute(null);
                CTools.ShowToast(Language.QuotationSaved);
            }
        }

        private void DiscardDocument()
        {
            QuotationViewModel.DiscarDocumentCommand.Execute(null);
        }

        private void ShowFragment(EMenu enmMenu)
        {
            switch (enmMenu)
            {
                case EMenu.Documentdetail:
                    QuotationViewModel.DocumentdetailViewModel.Mode = CDocumentService.EMode.Quotation;
                    _objDialogFragment = new DocumentdetailFragment { ViewModel = QuotationViewModel.DocumentdetailViewModel };
                    _objDialogFragment.Show(SupportFragmentManager, typeof(DocumentdetailFragment).Name);
                    break;
            }
        }

        public enum EMenu
        {
            Documentdetail = 0
        }

        public enum EOptionMenu
        {
            Exit = 0
        }

        #endregion
    }

}

