using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using XoW.Models;

namespace XoW.Services
{
    public class UserHashColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString() == "1" ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Gray);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }

    public class NewThreadButtonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            value.ToString() == Constants.TimelineForumId ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }

    public class ThreadPrevPageButtonEnableStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            ((ObservableCurrentThreadPage)value).CurrentPage == 1 ? false : true;
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
