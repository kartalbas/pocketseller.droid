using System.Collections.Generic;
using System.Linq;
using MvvmCross.Plugin.Messenger;
using pocketseller.core.Messages;
using pocketseller.core.ModelsAPI;
using orderline.core.Resources.Languages;
using pocketseller.core.Services.Interfaces;

namespace pocketseller.core.ViewModels
{
    public class StockDocumentViewModel : BaseViewModel
    {
        #region Private properties
        #endregion

        #region Constructors

        public StockDocumentViewModel(IDataService objDataService, IDocumentService objDocumentService, ISettingService objSettingService, ILanguageService objLanguageService, IMvxMessenger objMessenger)
            : base(objDataService, objDocumentService, objSettingService, objLanguageService, objMessenger)
        {
            LogTag = GetType().Name;
            SubscriptionToken1 = objMessenger.SubscribeOnMainThread<LanguageServiceMessage>(OnLanguageChanged);
        }

        #endregion

        #region Private methods

        private void OnLanguageChanged(LanguageServiceMessage objMessage)
        {
            if (objMessage.InitializeLabels)
                Init();
        }

        #endregion

        #region Public methods

        public override void Init()
        {
            LabelTitle = Language.TabPositions;
            LabelParcels = Language.Parcels;
            LabelTotalVat = Language.OrderValue;
            LabelTotalBrutto = Language.OrderValueVat;

            LabelPos = Language.Pos;
            LabelArticlenumber = Language.Articlenumber;
            LabelCount = Language.Count;
            LabelContent = Language.Content;
            LabelAmount = Language.Amount;
            LabelUnitPrice = Language.Price;
            LabelPositionTotal = Language.PositionTotal;
            LabelArticle = Language.Article;
        }

        public override void Init(object objParam) {}

        #endregion

        #region Public properties

        private string _labelArticlenumber;
        public string LabelArticlenumber { get => _labelArticlenumber;
            set { _labelArticlenumber = value; RaisePropertyChanged(() => LabelArticlenumber); } }

        private string _labelParcels;
        public string LabelParcels { get => _labelParcels;
            set { _labelParcels = value; RaisePropertyChanged(() => LabelParcels); } }

        private string _labelTotalVat;
        public string LabelTotalVat { get => _labelTotalVat;
            set { _labelTotalVat = value; RaisePropertyChanged(() => LabelTotalVat); } }

        private string _labelTotalBrutto;
        public string LabelTotalBrutto { get => _labelTotalBrutto;
            set { _labelTotalBrutto = value; RaisePropertyChanged(() => LabelTotalBrutto); } }

        private string _labelPos;
        public string LabelPos { get => _labelPos;
            set { _labelPos = value; RaisePropertyChanged(() => LabelPos); } }

        private string _labelCount;
        public string LabelCount { get => _labelCount;
            set { _labelCount = value; RaisePropertyChanged(() => LabelCount); } }

        private string _labelContent;
        public string LabelContent { get => _labelContent;
            set { _labelContent = value; RaisePropertyChanged(() => LabelContent); } }

        private string _labelAmount;
        public string LabelAmount { get => _labelAmount;
            set { _labelAmount = value; RaisePropertyChanged(() => LabelAmount); } }

        private string _labelUnitPrice;
        public string LabelUnitPrice { get => _labelUnitPrice;
            set { _labelUnitPrice = value; RaisePropertyChanged(() => LabelUnitPrice); } }

        private string _labelPositionTotal;
        public string LabelPositionTotal { get => _labelPositionTotal;
            set { _labelPositionTotal = value; RaisePropertyChanged(() => LabelPositionTotal); } }

        private string _labelArticle;
        public string LabelArticle { get => _labelArticle;
            set { _labelArticle = value; RaisePropertyChanged(() => LabelArticle); } }

        public Order Order => DocumentService.Order;
        public List<Orderdetail> Orderdetail => DocumentService.Order.Orderdetails.OrderBy(o => o.Pos).ToList();

        #endregion
    }
}
