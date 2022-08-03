using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XoW.Views
{
    public sealed partial class JumpToThreadContentDialog : ContentDialog
    {
        public JumpToThreadContentDialog(
            TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> primaryButtonEventHandler = null)
        {
            InitializeComponent();
            RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;

            if (primaryButtonEventHandler != null)
            {
                PrimaryButtonClick += primaryButtonEventHandler;
            }
        }
    }
}
