using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace XoW.Utils
{
    public static class ThemeHelper
    {
        public static void ApplyTheme(bool isDarkThemeEnabled)
        {
            var frameworkElementRoot = Window.Current.Content as FrameworkElement;
            if (frameworkElementRoot != null)
            {
                frameworkElementRoot.RequestedTheme = isDarkThemeEnabled ? ElementTheme.Dark : ElementTheme.Light;
            }

            GlobalState.ObservableObject.BackgroundAndBorderColorBrush = isDarkThemeEnabled
                ? new SolidColorBrush(Colors.Black)
                : new SolidColorBrush(Colors.LightGray);

            GlobalState.ObservableObject.ListViewBackgroundColorBrush = isDarkThemeEnabled
                ? new SolidColorBrush(Colors.Black)
                : new SolidColorBrush(Colors.White);
        }
    }
}
