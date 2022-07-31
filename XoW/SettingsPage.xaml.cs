using System;
using System.Linq;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XoW.Models;
using XoW.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace XoW
{
    public sealed partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void OnCookieClicked(object sender, ItemClickEventArgs args)
        {
            var selectedCookie = args.ClickedItem as AnonBbsCookie;
            var cookieName = selectedCookie.Name;
            var cookieValue = selectedCookie.Cookie;

            ApplicationConfigurationHelper.SetCurrentCookie(cookieName);
            HttpClientService.ApplyCookie(cookieValue);
        }

        private void OnSubscriptionIdTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var newSubscriptionId = TextBoxSubscriptionId.Text;
            UpdateSubscriptionId(newSubscriptionId);
        }

        private async void OnLoadImageButtonClicked(object sender, RoutedEventArgs args)
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add("*");

            var storageFile = await filePicker.PickSingleFileAsync();

            if (storageFile == null)
            {
                return;
            }

            var fileMimeType = storageFile.ContentType;
            if (!fileMimeType.StartsWith("image/"))
            {
                throw new AppException(ErrorMessage.FileIsNotImage);
            }

            var cookie = await QrCodeService.DecodeBarcodeFromStorageFileAsync(storageFile);

            if (!GlobalState.Cookies.Contains(cookie))
            {
                GlobalState.Cookies.Add(cookie);
                ApplicationConfigurationHelper.AddCookie(cookie);
            }
        }

        private async void OnDeleteCookieButtonClicked(object sender, RoutedEventArgs args)
        {
            var dialog = new ContentDialog
            {
                RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme,
                PrimaryButtonText = ComponentContent.Confirm,
                SecondaryButtonText = ComponentContent.Cancel,
                DefaultButton = ContentDialogButton.Primary,
                Title = ConfirmationMessage.DeleteCookieConfirmation,
            };

            var result = await dialog.ShowAsync();

            switch (result)
            {
                case ContentDialogResult.Primary:
                    var cookieName = ((Button)sender).DataContext?.ToString();
                    DeleteCookie(cookieName);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 保存是否开启夜间模式的配置，并设定应用全局主题
        /// </summary>
        private void OnNightModeSwitchToggled(object sender, RoutedEventArgs args)
        {
            var isDarkModeEnabled = ((ToggleSwitch)sender).IsOn;
            ApplicationConfigurationHelper.SetDarkThemeEnabled(isDarkModeEnabled);

            #region 设定应用全局主题
            var frameworkElementRoot = Window.Current.Content as FrameworkElement;
            frameworkElementRoot.RequestedTheme = isDarkModeEnabled ? ElementTheme.Dark : ElementTheme.Light;
            #endregion

            #region 设定部分手动指定颜色的控件的新颜色
            var borderAndBackgroundColor = isDarkModeEnabled ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.LightGray);
            GlobalState.BackgroundAndBorderColor.ColorBrush = borderAndBackgroundColor;

            var listViewBackgroundColor = isDarkModeEnabled ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            GlobalState.ListViewAndInputBackgroundColor.ColorBrush = listViewBackgroundColor;
            #endregion
        }

        /// <summary>
        /// 应用加载时会触发此事件，此时载入是否开启夜间模式的设定
        /// </summary>
        private void OnNightModeSwitchLoaded(object sender, RoutedEventArgs args)
        {
            var isDarkModeEnabled = ApplicationConfigurationHelper.IsDarkThemeEnabled();

            // 该操作会触发 ToggleSwitch 的 Toggled 事件
            ((ToggleSwitch)sender).IsOn = isDarkModeEnabled;
        }

        private async void OnGenerateSubscriptionButtonClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(GlobalState.SubscriptionId?.SubscriptionId))
            {
                var dialog = new ContentDialog
                {
                    RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme,
                    PrimaryButtonText = ComponentContent.Confirm,
                    SecondaryButtonText = ComponentContent.Cancel,
                    DefaultButton = ContentDialogButton.Primary,
                    Title = ConfirmationMessage.GenerateNewSubscriptionIdConfirmationTitle,
                    Content = ConfirmationMessage.GenerateNewSubscriptionIdConfirmationContent,
                    Foreground = new SolidColorBrush(Colors.Red),
                };

                var result = await dialog.ShowAsync();

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        GenerateNewSubscriptionId();
                        break;
                    default:
                        break;
                }

                return;
            }

            GenerateNewSubscriptionId();
        }

        private void DeleteCookie(string cookieName)
        {
            if (cookieName == GlobalState.CurrentCookie.CurrentCookie)
            {
                ApplicationConfigurationHelper.RemoveCurrentCookie();
                GlobalState.CurrentCookie.CurrentCookie = null;
            }

            ApplicationConfigurationHelper.DeleteCookie(cookieName);
            GlobalState.Cookies.Remove(GlobalState.Cookies.Where(cookie => cookie.Name == cookieName).Single());
        }

        private void GenerateNewSubscriptionId()
        {
            var newSubscriptionId = Guid.NewGuid().ToString();
            UpdateSubscriptionId(newSubscriptionId);
        }

        private void UpdateSubscriptionId(string newSubscriptionId)
        {
            GlobalState.SubscriptionId.SubscriptionId = newSubscriptionId;
            ApplicationConfigurationHelper.SetSubscriptionId(newSubscriptionId);
        }

    }
}
