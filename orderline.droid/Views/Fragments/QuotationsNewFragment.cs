using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.Models;
using orderline.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class QuotationsNewFragment : MvxFragment
    {
        public QuotationsNewViewModel QuotationsNewViewModel => (ViewModel as QuotationsNewViewModel);

        private MvxListView _objListView;
        private Intent _objIntentQuotationView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(orderline.droid.Resource.Layout.QuotationsNewFragment, null);

            HasOptionsMenu = true;

            var objSingletonService = (CSingletonService) Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.QuotationsNewViewModel == null)
                objSingletonService.QuotationsNewViewModel = CMvvmCrossTools.LoadViewModel<QuotationsNewViewModel>();

            ViewModel = objSingletonService.QuotationsNewViewModel;

            _objIntentQuotationView = new Intent(Activity, typeof(QuotationView));
            _objListView = objView.FindViewById<MvxListView>(orderline.droid.Resource.Id.quotationsnew_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateOptionsMenu(IMenu objMenu, MenuInflater inflater)
        {
            objMenu.Clear(); 

            objMenu.Add("")
                .SetIcon(orderline.droid.Resource.Drawable.ic_action_new_dark)
                .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnActionNewClicked))
                .SetShowAsAction(ShowAsAction.Always);

            base.OnCreateOptionsMenu(objMenu, inflater);
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(QuotationsNewViewModel.LabelEdit);
            objMenu.Add(QuotationsNewViewModel.LabelSend);
            objMenu.Add(QuotationsNewViewModel.LabelDelete);
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            try
            {
                var strSelectedAction = objItem.ToString();

                var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
                var objDocument = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Quotation;

                if (strSelectedAction == QuotationsNewViewModel.LabelSend)
                {
                    CTools.EnableOrDisableView(_objListView, false);
                    QuotationsNewViewModel.ShowWorkingCommand.Execute(null);
                    Task.Run(() => CGmWebServices.Instance.SendQuotation(objDocument)
                        .ContinueWith(objTask =>
                        {
                            try
                            {
                                QuotationsNewViewModel.HideWorkingCommand.Execute(null);
                                if (objDocument != null) objDocument.Response = objTask.Result.Content;
                                QuotationsNewViewModel.SendDocumentCommand.Execute(objDocument);
                                CTools.ShowToast(Language.QuotationSentSuccessfully);
                                CTools.EnableOrDisableView(_objListView, true);
                            }
                            catch (Exception objException)
                            {
                                QuotationsNewViewModel.HideWorkingCommand.Execute(null);
                                CErrorHandling.Log( objException, true);
                                CTools.EnableOrDisableView(_objListView, true);
                            }
                        }));
                }
                else if (strSelectedAction == QuotationsNewViewModel.LabelEdit)
                {
                    QuotationsNewViewModel.EditDocumentCommand.Execute(objDocument);
                }
                else if (strSelectedAction == QuotationsNewViewModel.LabelDelete)
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

                return base.OnContextItemSelected(objItem);
            }
            catch (Exception objException)
            {
                QuotationsNewViewModel.HideWorkingCommand.Execute(null);
                CErrorHandling.Log( objException, true);
                CTools.EnableOrDisableView(_objListView, true);
                return true;
            }
        }

        private void DeleteOrder(Quotation objDocument)
        {
            QuotationsNewViewModel.DeleteDocumentCommand.Execute(objDocument);
        }

        private bool OnActionNewClicked(IMenuItem arg)
        {
            StartActivity(_objIntentQuotationView);
            return true;
        }
    }
}