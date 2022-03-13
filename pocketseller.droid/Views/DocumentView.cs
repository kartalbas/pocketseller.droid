using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using pocketseller.droid.Views.Fragments;
using System.Threading.Tasks;
using System;
using System.Net;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Models;

namespace pocketseller.droid.Views
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/Theme.Pocketsellertheme1", WindowSoftInputMode = SoftInput.AdjustResize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class DocumentView : MvxFragmentActivity
    {
        #region Private properties

        private DocumentViewModel DocumentViewModel => (ViewModel as DocumentViewModel);
        private MvxDialogFragment _objDialogFragment;
        private MvxSubscriptionToken _objTokenDocument;
        private MvxSubscriptionToken _objTokenWorking;
        private FragmentTabHost _objFragmentTabHost;

        #endregion

        #region Overridden methods

        protected override void OnCreate(Bundle objInState)
        {
            base.OnCreate(objInState);
            RequestWindowFeature(WindowFeatures.IndeterminateProgress);
            SetContentView(pocketseller.droid.Resource.Layout.DocumentView);

            CTools.InitActionBar(ActionBar, DocumentViewModel.LabelTitle);

            _objFragmentTabHost = FindViewById<FragmentTabHost>(Android.Resource.Id.TabHost);
            _objFragmentTabHost.Setup(this, SupportFragmentManager, Android.Resource.Id.TabContent);

            CTools.CreateTab(_objFragmentTabHost, typeof(DocumentAddressFragment), DocumentViewModel.DocumentAddressViewModel.LabelTitle, pocketseller.droid.Resource.Drawable.documentsfragment_tab_newdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(DocumentOrderFragment), DocumentViewModel.DocumentOrderViewModel.LabelTitle, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(DocumentInfoFragment), DocumentViewModel.DocumentInfoViewModel.LabelTitle, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.CreateTab(_objFragmentTabHost, typeof(DocumentOutstandingFragment), DocumentViewModel.DocumentOutstandingViewModel.LabelTitle, pocketseller.droid.Resource.Drawable.documentsfragment_tab_sentdocument);
            CTools.SetTabHostTextSizes(_objFragmentTabHost, 8);

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

        public override bool OnCreateOptionsMenu(IMenu objMenu)
        {
            objMenu.Clear();

            objMenu.Add("")
                .SetIcon(pocketseller.droid.Resource.Drawable.ic_action_download_dark)
                .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnShowQuotationsActionClicked))
                .SetShowAsAction(ShowAsAction.Always);

            objMenu.Add("")
                .SetIcon(pocketseller.droid.Resource.Drawable.ic_action_cancel_dark)
                .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnNewActionClicked))
                .SetShowAsAction(ShowAsAction.Always);

            return true;
        }

        private bool OnShowQuotationsActionClicked(IMenuItem arg)
        {
            ShowFragment(EMenu.Quotations);
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

        public override void OnBackPressed()
        {
            if(!DocumentViewModel.IsInPaymentState)
            {
                CTools.ShowDialog(this
                    , Language.Attention
                    , Language.OrderNotSavable
                    , Language.Exit
                    , (sender, args) =>
                    {
                        CTools.ShowToast(Language.OrderNotSaved);
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
            else if (DocumentViewModel.DocumentState == EOrderState.CHANGED)
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
                    , Language.SaveAndSend
                    , (sender, args) =>
                    {
                        var id = SaveDocument();
                        SendOrder(id);
                        Finish();
                        base.OnBackPressed();
                    });
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

        private void OnWorking(WorkingMessage objWorkingMessage)
        {
            RunOnUiThread(() =>
            {
                switch (objWorkingMessage.EWorkingAction)
                {
                    case EWorkingAction.ShowWorking:
                        SetProgressBarIndeterminateVisibility(true);
                        _objFragmentTabHost.TabWidget.Enabled = false;
                        break;
                    case EWorkingAction.HideWorking:
                        SetProgressBarIndeterminateVisibility(false);
                        _objFragmentTabHost.TabWidget.Enabled = true;
                        break;
                }
            });
        }

        private void SubscribeMessenger()
        {
            _objTokenDocument = Mvx.IoCProvider.Resolve<IMvxMessenger>().SubscribeOnMainThread<DocumentMessage>(OnDocumentAction);
            _objTokenWorking = Mvx.IoCProvider.Resolve<IMvxMessenger>().SubscribeOnMainThread<WorkingMessage>(OnWorking);
        }

        private void UnSubscribeMessenger()
        {
            Mvx.IoCProvider.Resolve<IMvxMessenger>().Unsubscribe<DocumentMessage>(_objTokenDocument);
            _objTokenDocument.Dispose();
            _objTokenDocument = null;

            Mvx.IoCProvider.Resolve<IMvxMessenger>().Unsubscribe<WorkingMessage>(_objTokenWorking);
            _objTokenWorking.Dispose();
            _objTokenWorking = null;
        }

        private void OnDocumentAction(DocumentMessage objDocumentMessage)
        {
            RunOnUiThread(() =>
            {
                switch (objDocumentMessage.EDocumentAction)
                {
                    case EDocumentAction.ShowAddress:
                        ShowFragment(EMenu.DocumentAddress);
                        break;
                    case EDocumentAction.ShowDocumentOrder:
                        ShowFragment(EMenu.DocumentOrder);
                        break;
                    case EDocumentAction.ShowDocumentDetail:
                        ShowFragment(EMenu.Documentdetail, CDocumentService.EMode.Order);
                        break;
                }
            });
        }

        private void CheckSaveable()
        {
            if (!DocumentViewModel.IsInPaymentState)
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(
                    Language.OrderNotSavable,
                    Language.Attention,
                    Language.Ok);
            }
        }

        private Guid SaveDocument()
        {
            if (!DocumentViewModel.IsSavable)
            {
                CTools.ShowToast(Language.OrderNotSaved);
                return Guid.Empty;
            }
            else
            {
                var result = DocumentViewModel.DocumentOrderViewModel.Document.Id;
                DocumentViewModel.DocumentOrderViewModel.SaveDocumentCommand.Execute(null);
                CTools.ShowToast(Language.OrderSaved);
                return result;
            }
        }

        private void SendOrder(Guid id)
        {
            if(id == Guid.Empty)
            {
                CTools.ShowToast(Language.OrderNotSaved);
            }
            else
            {
                var dataService = ((CDataService)Mvx.IoCProvider.Resolve<IDataService>());
                var document = dataService.FindWithQuery<Document>($"SELECT * FROM Document WHERE Id ='{id}'");

                Task.Run(() => CGmWebServices.Instance.SendDocument(document, ESettingType.RestDocumentAddOrUpdate)
                    .ContinueWith(objTask =>
                    {
                        try
                        {
                            if (objTask.Result.StatusCode != HttpStatusCode.Accepted && objTask.Result.StatusCode != HttpStatusCode.OK)
                            {
                                var message = Mvx.IoCProvider.Resolve<IBaseService>().GetErrorMessage(objTask.Result.Content);
                                CTools.ShowMessage("Sending", message);
                                throw new Exception(message);
                            }
                            else
                            {
                                CTools.ShowToast(Language.OrderSentSuccessfully);
                                if (document != null)
                                {
                                    Document.DeleteDocument(document);
                                    Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new DocumentsViewServiceMessage(this, EDocumentsViewAction.Added));
                                }
                            }

                            DiscardDocument();
                        }
                        catch (Exception objException)
                        {
                            CErrorHandling.Log(objException, true);
                        }
                    }));
            }
        }

        private void DiscardDocument()
        {
            DocumentViewModel.DocumentOrderViewModel.DiscarDocumentCommand.Execute(null);
        }

        private void ShowFragment(EMenu enmMenu, CDocumentService.EMode mode = CDocumentService.EMode.Order)
        {
            switch (enmMenu)
            {
                case EMenu.Quotations:
                    DocumentViewModel.QuotationSelectorViewModel.ShowValidQuotationsCommand.Execute(null);
                    _objDialogFragment = new QuotationSelectorFragment { ViewModel = DocumentViewModel.QuotationSelectorViewModel };
                    _objDialogFragment.Show(SupportFragmentManager, typeof(QuotationSelectorFragment).Name);
                    break;
                case EMenu.Documentdetail:
                    DocumentViewModel.DocumentdetailViewModel.Mode = mode;
                    _objDialogFragment = new DocumentdetailFragment { ViewModel = DocumentViewModel.DocumentdetailViewModel };
                    _objDialogFragment.Show(SupportFragmentManager, typeof(DocumentdetailFragment).Name);
                    break;
            }
        }

        public enum EMenu
        {
            DocumentAddress = 0,
            DocumentOrder = 1,
            DocumentInfo = 2,
            DocumentOutstanding = 3,
            DocumentAccount = 4,
            Documentdetail = 5,
            Quotations = 6
        }

        public enum EOptionMenu
        {
            Exit = 0
        }

        #endregion
    }

}

