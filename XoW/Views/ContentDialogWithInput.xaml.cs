using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XoW.Views
{
    public sealed partial class ContentDialogWithInput : ContentDialog
    {
        public ContentDialogWithInput(string title, string primaryButtonText, TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> primaryButtonEventHandler = null)
        {
            InitializeComponent();
            RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;

            Title = title;
            PrimaryButtonText = primaryButtonText;

            if (primaryButtonEventHandler != null)
            {
                PrimaryButtonClick += primaryButtonEventHandler;
            }
        }
    }
}
