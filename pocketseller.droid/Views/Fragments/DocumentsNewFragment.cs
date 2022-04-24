using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.Models;
using pocketseller.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using pocketseller.core.ModelsAPI;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class DocumentsNewFragment : MvxFragment
    {
        public DocumentsNewViewModel DocumentsNewViewModel => (ViewModel as DocumentsNewViewModel);
        private MvxListView _objListView;
        private Bundle _bundle;

        private string LogTag;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.DocumentsNewFragment, null);
            _bundle = savedInstanceState;

            LogTag = GetType().ToString();

            var objSingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.DocumentsNewViewModel == null)
                objSingletonService.DocumentsNewViewModel = CMvvmCrossTools.LoadViewModel<DocumentsNewViewModel>();

            ViewModel = objSingletonService.DocumentsNewViewModel;

            _objListView = objView.FindViewById<MvxListView>(pocketseller.droid.Resource.Id.documentsnewview_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(DocumentsNewViewModel.LabelEdit);
            objMenu.Add(DocumentsNewViewModel.LabelSendAsOrder);

            if(DocumentsNewViewModel.SettingService.Get<int>(ESettingType.CashAndCarry) == 1)
            {
                objMenu.Add(DocumentsNewViewModel.LabelSendAsDelivery);
                objMenu.Add(DocumentsNewViewModel.LabelSendAsFactura);
                objMenu.Add(DocumentsNewViewModel.LabelSendAsCreditNote);
            }

            objMenu.Add(DocumentsNewViewModel.LabelDelete);
            objMenu.Add($"BESTELLSCHEIN {DocumentsNewViewModel.LabelMail}");
            objMenu.Add($"BESTELLSCHEIN {DocumentsNewViewModel.LabelPrint}");
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            try
            {
                var strSelectedAction = objItem.ToString();

                var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
                var objDocument = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Document;

                if (strSelectedAction == DocumentsNewViewModel.LabelEdit)
                {
                    DocumentsNewViewModel.EditDocumentCommand.Execute(objDocument);
                    DocumentsNewViewModel.HideWorkingCommand.Execute(null);
                }
                else if (strSelectedAction == DocumentsNewViewModel.LabelSendAsOrder)
                {
                    if (objDocument.TotalBrutto < 0 && objDocument.Doctype == (int)EDocumentType.CREDITNOTE)
                    {
                        CTools.ShowMessage("Fehler", $"Beleg ist eine Gutschrift mit Betrag {objDocument.TotalBrutto}");
                    }
                    else
                    {
                        DocumentsNewViewModel.ShowWorkingCommand.Execute(null);
                        CTools.EnableOrDisableView(_objListView, false);
                        SendOrder(objDocument, ESettingType.RestDocumentAddOrUpdate);
                    }
                }
                else if (strSelectedAction == DocumentsNewViewModel.LabelSendAsDelivery)
                {
                    if (objDocument.TotalBrutto < 0 && objDocument.Doctype == (int)EDocumentType.CREDITNOTE)
                    {
                        CTools.ShowMessage("Fehler", $"Beleg ist eine Gutschrift mit Betrag {objDocument.TotalBrutto}");
                    }
                    else
                    {
                        DocumentsNewViewModel.ShowWorkingCommand.Execute(null);
                        CTools.EnableOrDisableView(_objListView, false);
                        objDocument.Doctype = (int)EDocumentType.DELIVERY;
                        SendOrder(objDocument, ESettingType.RestDocumentInsertOrderAsDelivery);
                    }
                }
                else if (strSelectedAction == DocumentsNewViewModel.LabelSendAsFactura)
                {
                    if (objDocument.TotalBrutto < 0 && objDocument.Doctype == (int)EDocumentType.CREDITNOTE)
                    {
                        CTools.ShowMessage("Fehler", $"Beleg ist eine Gutschrift mit Betrag {objDocument.TotalBrutto}");
                    }
                    else
                    {
                        DocumentsNewViewModel.ShowWorkingCommand.Execute(null);
                        CTools.EnableOrDisableView(_objListView, false);
                        objDocument.Doctype = (int)EDocumentType.FACTURA;
                        SendOrder(objDocument, ESettingType.RestDocumentInsertOrderAsFactura);
                    }
                }
                else if (strSelectedAction == DocumentsNewViewModel.LabelSendAsCreditNote)
                {
                    if(objDocument.TotalBrutto > 0 && objDocument.Doctype != (int)EDocumentType.CREDITNOTE)
                    {
                        CTools.ShowMessage("Fehler", $"Beleg ist eine Bestellung mit Betrag {objDocument.TotalBrutto}");
                    }
                    else
                    {
                        DocumentsNewViewModel.ShowWorkingCommand.Execute(null);
                        CTools.EnableOrDisableView(_objListView, false);
                        objDocument.Doctype = (int)EDocumentType.CREDITNOTE;
                        SendOrder(objDocument, ESettingType.RestDocumentInsertOrderAsCreditNote);
                    }
                }
                else if (strSelectedAction == DocumentsNewViewModel.LabelDelete)
                {
                    CTools.ShowDialog(Activity
                        , Language.Attention
                        , Language.DoYouWantDelete
                        , Language.Yes
                        , (sender, args) => DeleteOrder(objDocument)
                        , Language.No
                        , (sender, args) => { }
                        , string.Empty
                        , null);
                }
                else if (strSelectedAction == $"BESTELLSCHEIN {DocumentsNewViewModel.LabelPrint}")
                {
                    objDocument.LocalDocument = true;
                    DocumentsNewViewModel.PrintDocumentCommand.ExecuteAsync(objDocument);
                }

                else if (strSelectedAction == $"BESTELLSCHEIN {DocumentsNewViewModel.LabelMail}")
                {
                    DocumentsNewViewModel.EmailCommand.ExecuteAsync(objDocument);
                }

                return base.OnContextItemSelected(objItem);
            }
            catch (Exception objException)
            {
                DocumentsNewViewModel.HideWorkingCommand.Execute(null);
                CErrorHandling.Log( objException, true);
                return true;
            }
        }

        private void SendOrder(Document document, ESettingType targetType)
        {
            Task.Run(() => CGmWebServices.Instance.SendDocument(document, targetType)
                .ContinueWith(objTask =>
                {
                    try
                    {
                        DocumentsNewViewModel.HideWorkingCommand.Execute(null);
                        if (document != null) document.Response = objTask.Result.Content;
                        DocumentsNewViewModel.DeleteDocumentCommand.Execute(document);
                        CTools.ShowToast(Language.OrderSentSuccessfully);
                        CTools.EnableOrDisableView(_objListView, true);
                    }
                    catch (Exception objException)
                    {
                        DocumentsNewViewModel.HideWorkingCommand.Execute(null);
                        CErrorHandling.Log(objException, true);
                        CTools.EnableOrDisableView(_objListView, true);
                    }
                }));
        }

        private void DeleteOrder(Document objDocument)
        {
            DocumentsNewViewModel.DeleteDocumentCommand.Execute(objDocument);
            DocumentsNewViewModel.HideWorkingCommand.Execute(null);
        }
    }
}