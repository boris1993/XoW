using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace XoW.Views
{
    public sealed partial class ConfirmationContentDialog : ContentDialog
    {
        public ConfirmationContentDialog(string title, string content = null, Color? foreGroundColor = null, TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> primaryButtonEventHandler = null, TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> secondaryButtonEventHandler = null, string primaryButtonContent = null, string secondaryButtonContent = null)
        {
            InitializeComponent();
            RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;
            Title = title;
            Content = content;
            PrimaryButtonText = primaryButtonContent ?? ComponentContent.Confirm;
            SecondaryButtonText = secondaryButtonContent ?? ComponentContent.Cancel;

            Foreground = foreGroundColor != null
                ? new SolidColorBrush((Color)foreGroundColor)
                : new SolidColorBrush(Colors.Black);

            if (primaryButtonEventHandler != null)
            {
                PrimaryButtonClick += primaryButtonEventHandler;
            }

            if (secondaryButtonEventHandler != null)
            {
                SecondaryButtonClick += secondaryButtonEventHandler;
            }
        }
    }
}
