using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using pocketseller.core.ModelConverter;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources.Languages;
using pocketseller.core.Services;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.Tools;
using pocketseller.core.ViewModels;
using pocketseller.droid.Helper;
using Quotation = pocketseller.core.Models.Quotation;

namespace pocketseller.droid.Views.Fragments
{
    public sealed class QuotationsSentFragment : MvxFragment
    {
        public QuotationsSentViewModel QuotationsSentViewModel => (ViewModel as QuotationsSentViewModel);
        private MvxListView _objListView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(orderline.droid.Resource.Layout.QuotationsSentFragment, null);

            HasOptionsMenu = true;

            var objSingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.QuotationsSentViewModel == null)
                objSingletonService.QuotationsSentViewModel = CMvvmCrossTools.LoadViewModel<QuotationsSentViewModel>();

            ViewModel = objSingletonService.QuotationsSentViewModel;

            _objListView = objView.FindViewById<MvxListView>(orderline.droid.Resource.Id.quotationsent_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }
        public override void OnCreateOptionsMenu(IMenu objMenu, MenuInflater inflater)
        {
            objMenu.Clear();

            objMenu.Add("")
                .SetIcon(orderline.droid.Resource.Drawable.ic_action_download_dark)
                .SetOnMenuItemClickListener(new DelegatedMenuItemListener(OnRefreshActionClicked))
                .SetShowAsAction(ShowAsAction.Always);

            base.OnCreateOptionsMenu(objMenu, inflater);
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(QuotationsSentViewModel.LabelDelete);
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objDocument = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Quotation;

            if (strSelectedAction == QuotationsSentViewModel.LabelDelete)
            {
                CTools.ShowDialog(Activity
                    , Language.Attention
                    , Language.DoYouWantDelete
                    , Language.Yes
                    , (sender, args) => DeleteQuotation(objDocument)
                    , Language.No
                    , (sender, args) => { }
                    , string.Empty
                    , null);
            }

            return base.OnContextItemSelected(objItem);
        }

        private void DeleteQuotation(Quotation objDocument)
        {
            CTools.EnableOrDisableView(_objListView, false);
            QuotationsSentViewModel.ShowWorkingCommand.Execute(null);
            Task.Run(() => CGmWebServices.Instance.ChangeQuotationState( EOrderState.DELETED, QuoToProxyQuotation.CreateQuotation(objDocument)))
                .ContinueWith(task =>
                {
                    try
                    {
                        QuotationsSentViewModel.HideWorkingCommand.Execute(null);
                        if (objDocument != null) objDocument.Response = task.Result.Content;
                        QuotationsSentViewModel.DeleteDocumentCommand.Execute(objDocument);
                        CTools.EnableOrDisableView(_objListView, true);
                    }
                    catch (Exception objException)
                    {
                        QuotationsSentViewModel.HideWorkingCommand.Execute(null);
                        CErrorHandling.Log( objException, true);
                        CTools.EnableOrDisableView(_objListView, true);
                    }
                });
        }

        private bool OnRefreshActionClicked(IMenuItem objMenuItem)
        {
            RefreshQuotations(EOrderState.ORDER);
            return true;
        }

        private void RefreshQuotations(EOrderState enmState)
        {
            CTools.EnableOrDisableView(_objListView, false);
            CGmWebServices.Instance.RefreshQuotations(enmState,
                () => QuotationsSentViewModel.ShowWorkingCommand.Execute(null),
                objQuotations =>
                {
                    QuotationsSentViewModel.ReplaceCommand.Execute(objQuotations);
                    QuotationsSentViewModel.HideWorkingCommand.Execute(null);
                    CTools.EnableOrDisableView(_objListView, true);
                },
                objException =>
                {
                    QuotationsSentViewModel.HideWorkingCommand.Execute(null);
                    CErrorHandling.Log(objException, true);
                    CTools.EnableOrDisableView(_objListView, true);
                });
        }
    }
}