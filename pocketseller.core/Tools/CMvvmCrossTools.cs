using MvvmCross;
using MvvmCross.ViewModels;

namespace pocketseller.core.Tools
{
    public class CMvvmCrossTools
    {
        public static T LoadViewModel<T>() where T : MvxViewModel
        {
            var resolver = Mvx.IoCProvider.Resolve<IMvxViewModelLoader>();
            var result = (T)resolver.LoadViewModel(MvxViewModelRequest<T>.GetDefaultRequest(), null);
            return result;
        }
    }
}
