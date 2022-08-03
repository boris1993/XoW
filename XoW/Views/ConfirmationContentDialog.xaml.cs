using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace XoW.Views
{
    public sealed partial class ConfirmationContentDialog : ContentDialog
    {
        public ConfirmationContentDialog(
            string title,
            string content = null,
            Color? foreGroundColor = null,
            TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> primaryButtonEventHandler = null,
            TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> secondaryButtonEventHandler = null)
        {
            InitializeComponent();
            RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;
            Title = title;
            Content = content;

            if (foreGroundColor != null)
            {
                Foreground = new SolidColorBrush((Color)foreGroundColor);
            }
            else
            {
                Foreground = new SolidColorBrush(Colors.Black);
            }

            if (primaryButtonEventHandler != null)
            {
                PrimaryButtonClick += primaryButtonEventHandler;
            }

            if (secondaryButtonEventHandler != null)
            {
                SecondaryButtonClick += primaryButtonEventHandler;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
