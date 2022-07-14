using System;
using Windows.UI.Xaml.Data;

namespace XoW.Services
{
    public class CookieNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            string.IsNullOrEmpty((string)value) ? Constants.NoCookieSelected : value.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
