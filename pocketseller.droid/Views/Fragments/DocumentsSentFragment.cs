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

namespace pocketseller.droid.Views.Fragments
{
    public sealed class DocumentsSentFragment : MvxFragment
    {
        public DocumentsSentViewModel DocumentsSentViewModel => (ViewModel as DocumentsSentViewModel);
        private MvxListView _objListView;

        private string LogTag;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var objView = this.BindingInflate(pocketseller.droid.Resource.Layout.DocumentsSentFragment, null);

            LogTag = GetType().ToString();

            var objSingletonService = (CSingletonService)Mvx.IoCProvider.Resolve<ISingletonService>();

            if (objSingletonService.DocumentsSentViewModel == null)
                objSingletonService.DocumentsSentViewModel = CMvvmCrossTools.LoadViewModel<DocumentsSentViewModel>();

            ViewModel = objSingletonService.DocumentsSentViewModel;

            _objListView = objView.FindViewById<MvxListView>(pocketseller.droid.Resource.Id.documentssentview_listview);
            RegisterForContextMenu(_objListView);

            return objView;
        }

        public override void OnCreateContextMenu(IContextMenu objMenu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(objMenu, v, menuInfo);
            objMenu.Add(DocumentsSentViewModel.LabelActivate);
            objMenu.Add(DocumentsSentViewModel.LabelDelete);
        }

        public override bool OnContextItemSelected(IMenuItem objItem)
        {
            var strSelectedAction = objItem.ToString();

            var objListItemView = (AdapterView.AdapterContextMenuInfo)objItem.MenuInfo;
            var objDocument = (_objListView.Adapter.GetRawItem(objListItemView.Position)) as Document;

            if (strSelectedAction == DocumentsSentViewModel.LabelActivate)
            {
                DocumentsSentViewModel.ActivateDocumentCommand.Execute(objDocument);
                DocumentsSentViewModel.HideWorkingCommand.Execute(null);
            }
            else if (strSelectedAction == DocumentsSentViewModel.LabelDelete)
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

        private void DeleteOrder(Document objDocument)
        {
            DocumentsSentViewModel.DeleteDocumentCommand.Execute(objDocument);
            DocumentsSentViewModel.HideWorkingCommand.Execute(null);
        }
    }
}