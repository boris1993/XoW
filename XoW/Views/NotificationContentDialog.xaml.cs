using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace XoW.Views
{
    public sealed partial class NotificationContentDialog : ContentDialog
    {
        public NotificationContentDialog(bool isErrorPopup, string content)
        {
            this.InitializeComponent();
            this.RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;
            this.Title = isErrorPopup ? ComponentContent.Error : ComponentContent.Notification;
            this.Content = content;
        }
    }
}
