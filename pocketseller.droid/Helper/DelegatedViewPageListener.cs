using Android.Support.V4.View;

namespace pocketseller.droid.Helper
{
    public delegate void SimpleOnPageChangeListenerSelectedHandler(int iPosition);
    public delegate void SimpleOnPageChangeListenerScrollStateChangedHandler(int iState);
    public delegate void SimpleOnPageChangeListenerScrolledHandler(int iPosition, float fPositionOffset, int iPositionOffsetPixels);

    
    public class DelegatedViewPageListener : ViewPager.SimpleOnPageChangeListener
    {
        public event SimpleOnPageChangeListenerSelectedHandler PageSelected;
        public event SimpleOnPageChangeListenerScrollStateChangedHandler PageScrollStateChanged;
        public event SimpleOnPageChangeListenerScrolledHandler PageScrolled;

        public override void OnPageSelected(int iPosition)
        {
            if (PageSelected != null)
                PageSelected(iPosition);
            base.OnPageSelected(iPosition);
        }

        public override void OnPageScrollStateChanged(int iState)
        {
            if (PageScrollStateChanged != null)
                PageScrollStateChanged(iState);
            base.OnPageScrollStateChanged(iState);
        }

        public override void OnPageScrolled(int iPosition, float fPositionOffset, int iPositionOffsetPixels)
        {
            if (PageScrolled != null)
                PageScrolled(iPosition, fPositionOffset, iPositionOffsetPixels);
            base.OnPageScrolled(iPosition, fPositionOffset, iPositionOffsetPixels);
        }
    }
}