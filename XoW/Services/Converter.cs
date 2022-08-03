using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using XoW.Models;

namespace XoW.Services
{
    public class CookieNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            string.IsNullOrEmpty((string)value) ? Constants.NoCookieSelected : value.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }

    public class SetButtonStateByContentExistenseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !string.IsNullOrWhiteSpace(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }

    public class CookieNameToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return GlobalState.Cookies.SingleOrDefault(cookie =>
                cookie.Name == GlobalState.ObservableObject.CurrentCookie);
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            ((AnoBbsCookie)value).Name;
    }

    public class ThreadIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => $"No.{value}";
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
