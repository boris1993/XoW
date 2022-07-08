using System;
using System.Collections;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using XoW.Models;

namespace XoW.Services
{
    public class CookieNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            string.IsNullOrEmpty((string)value)
                ? Constants.NoCookieSelected
                : value.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }

    public class SetButtonStateByContentExistenceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => !string.IsNullOrWhiteSpace(value.ToString());

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }

    public class CookieNameToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => GlobalState.Cookies.SingleOrDefault(cookie => cookie.Name == GlobalState.ObservableObject.CurrentCookie);


        public object ConvertBack(object value, Type targetType, object parameter, string language) => ((AnoBbsCookie)value).Name;
    }

    public class ThreadIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => $"No.{value}";

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }

    public class CollapseItemWhenListIsEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is IList list)
            {
                return list.Count == 0
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }

    public class ShowItemWhenListIsEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is IList list)
            {
                return list.Count != 0
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }

    public class AiFaDianAvatarToThumbConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => new Uri($"{value}?imageView2/1/w/40/h/40");
        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }
}
