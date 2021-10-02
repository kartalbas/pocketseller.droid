using Android.App;
using Android.Content;
using Java.Lang;

namespace pocketseller.droid.Services
{
    public static class ObjectExtensions
    {
        public static void StartActivity(this object o, Intent intent)
        {
            var context = o as Context;
            if (context != null)
            {
                context.StartActivity(intent);
            }
            else
            {
                intent.SetFlags(ActivityFlags.NewTask);
                Application.Context.StartActivity(intent);
            }
        }

        public static JavaObject<T> ToJavaObject<T>(this T o)
        {
            return new JavaObject<T>(o);
        }
    }

    public class JavaObject<T> : Object
    {
        public JavaObject(T obj)
        {
            Value = obj;
        }

        public T Value { get; private set; }
    }
}