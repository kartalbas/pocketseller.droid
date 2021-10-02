
namespace pocketseller.core.ModelsAPI
{
    public enum EDocumentType
    {
        ORDER = 0,
        DELIVERY = 1,
        FACTURA = 2,
        CREDITNOTE = 3
    }

    public enum EOrderState
    {
        ORDER = 0,
        CHANGED = 1,
        DELIVERY = 2,
        FACTURA = 3,
        FACTURAIMPORTED = 4,
        FACTURAPRINTED = 5,
        DELETED = 6,
        DEFICIT = 7,
        CANCELED = 8,
        EXPORTEDTOERP = 9,
    }

    public enum ETargetDocumentType
    {
        ErpOrder = 0,
        DeliveryNote = 1,
        Factura = 2,
        CreditNote = 3
    }

    public enum EPhaseState
    {
        LOCAL = 0,
        SERVER = 1,
        ACTIVATED = 3
    }

    public enum EOrderdetailState
    {
        CHANGED,
        NEW,
        EDIT,
        DELETE
    }

    public enum ListSortDirection
    {
        Ascending = 0,
        Descending = 1
    }

    public enum EWordSearchType
    {
        BeginOfWord,
        OverAllWord
    }

    public enum EGeneralSearchType
    {
        Normal,
        LowerCase,
        UpperCase
    }

    public enum EKeyboardType
    {
        Normal,
        Phone,
        Number
    }
}
