using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XoW.Models;
using XoW.Services;
using XoW.Utils;

namespace XoW.Views
{
    public sealed partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void OnCookieClicked(object sender, ItemClickEventArgs args)
        {
            var selectedCookie = args.ClickedItem as AnoBbsCookie;
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
            var storageFile = await CommonUtils.OpenFilePickerForSingleImageAsync();

            var cookie = await QrCodeService.DecodeBarcodeFromStorageFileAsync(storageFile);

            if (!GlobalState.Cookies.Contains(cookie))
            {
                GlobalState.Cookies.Add(cookie);
                ApplicationConfigurationHelper.AddCookie(cookie);
            }
        }

        private async void OnDeleteCookieButtonClicked(object sender, RoutedEventArgs args)
        {
            await new ConfirmationContentDialog(
                ConfirmationMessage.DeleteCookieConfirmation,
                primaryButtonEventHandler: (_sender, _args) =>
                {
                    var cookieName = ((Button)sender).DataContext?.ToString();
                    DeleteCookie(cookieName);
                }).ShowAsync();
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
            GlobalState.ObservableObject.BackgroundAndBorderColorBrush = borderAndBackgroundColor;

            var listViewBackgroundColor = isDarkModeEnabled ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            GlobalState.ObservableObject.ListViewBackgroundColorBrush = listViewBackgroundColor;
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
            if (!string.IsNullOrEmpty(GlobalState.ObservableObject.SubscriptionId))
            {
                await new ConfirmationContentDialog(
                    ConfirmationMessage.GenerateNewSubscriptionIdConfirmationTitle,
                    ConfirmationMessage.GenerateNewSubscriptionIdConfirmationContent,
                    Colors.Red,
                    (_, _) => GenerateNewSubscriptionId()).ShowAsync();

                return;
            }

            GenerateNewSubscriptionId();
        }

        private static void DeleteCookie(string cookieName)
        {
            if (cookieName == GlobalState.ObservableObject.CurrentCookie)
            {
                ApplicationConfigurationHelper.RemoveCurrentCookie();
                GlobalState.ObservableObject.CurrentCookie = null;
            }

            ApplicationConfigurationHelper.DeleteCookie(cookieName);
            GlobalState.Cookies.Remove(GlobalState.Cookies.Single(cookie => cookie.Name == cookieName));
        }

        private static void GenerateNewSubscriptionId()
        {
            var newSubscriptionId = Guid.NewGuid().ToString();
            UpdateSubscriptionId(newSubscriptionId);
        }

        private static void UpdateSubscriptionId(string newSubscriptionId)
        {
            GlobalState.ObservableObject.SubscriptionId = newSubscriptionId;
            ApplicationConfigurationHelper.SetSubscriptionId(newSubscriptionId);
        }
    }
}
