using System;
using System.Globalization;
using MvvmCross.Converters;

namespace pocketseller.core.Converters
{
    public class DateTimeStringAccountValueConverter : MvxValueConverter<DateTime, string>
    {
        protected override string Convert(DateTime objValue, Type objTargetType, object objParameter, CultureInfo objCulture)
        {
            if (objValue == default(DateTime))
                return "";

            return objValue.ToString("dd.MM.yyyy");
        }
    }
}
