using MvvmCross.Plugin.Messenger;

namespace pocketseller.core.Messages
{
    public enum EOrderView
    {
        Stock_Print,
        Stock_Printed,
        Stock_Cancel,
        Import_Delivery,
        Import_Factura,
        Import_FacturaImported,
        Import_Delete,
        Deficit_Order,
        Deficit_Article,
        SourceChanged
    }

    public class OrdersViewServiceMessage : MvxMessage
    {
        public OrdersViewServiceMessage(object sender, EOrderView enmOrderView)
            : base(sender)
        {
            EOrderView = enmOrderView;
        }

        public EOrderView EOrderView { get; set; }
    }
}
