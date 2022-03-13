using MvvmCross.Plugin.Messenger;
using pocketseller.core.Services.Interfaces;
using pocketseller.core.ViewModels;

namespace pocketseller.core.Services
{
    public class CSingletonService : CBaseService, ISingletonService, IBaseService
    {
        public CSingletonService(IMvxMessenger objMessenger)
            : base(objMessenger)
        {
            LogTag = GetType().Name;
        }
        public LoginViewModel LoginViewModel { get; set; }

        public DocumentsNewViewModel DocumentsNewViewModel { get; set; }
        public QuotationsNewViewModel QuotationsNewViewModel { get; set; }
        public QuotationsSentViewModel QuotationsSentViewModel { get; set; }

        public StockToPrintViewModel StockToPrintViewModel { get; set; }
        public StockToPrintedViewModel StockToPrintedViewModel { get; set; }
        public StockToCancelViewModel StockToCancelViewModel { get; set; }

        public ImportToDeliveryViewModel ImportToDeliveryViewModel { get; set; }
        public ImportToFacturaViewModel ImportToFacturaViewModel { get; set; }
        public ImportToDeleteViewModel ImportToDeleteViewModel { get; set; }

        public FacturaAdressViewModel FacturaAllViewModel { get; set; }
        public FacturaDefectViewModel FacturaDefectViewModel { get; set; }
        public FacturaDayViewModel FacturaDayViewModel { get; set; }
        public FacturaWeekViewModel FacturaWeekViewModel { get; set; }

        public DocumentOutstandingViewModel DocumentOutstandingViewModel { get; set; }
        public DocumentOrderViewModel DocumentOrderViewModel { get; set; }
        public DocumentInfoViewModel DocumentInfoViewModel { get; set; }
        public DocumentdetailViewModel DocumentdetailViewModel { get; set; }
        public DocumentAddressViewModel DocumentAddressViewModel { get; set; }
        public DocumentAccountViewModel DocumentAccountViewModel { get; set; }
    }
}
