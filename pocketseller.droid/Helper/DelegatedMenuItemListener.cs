using System;
using Android.Views;
using Object = Java.Lang.Object;

namespace pocketseller.droid.Helper
{
    public class DelegatedMenuItemListener : Object, IMenuItemOnMenuItemClickListener
    {
        private readonly Func<IMenuItem, bool> _objHandler;

        public DelegatedMenuItemListener(Func<IMenuItem, bool> objHandler)
        {
            if (objHandler != null)
                _objHandler = objHandler;
        }

        public bool OnMenuItemClick(IMenuItem objItem)
        {
            return _objHandler(objItem);
        }
    }
}