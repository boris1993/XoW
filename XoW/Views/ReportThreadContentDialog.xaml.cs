using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XoW.Services;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace XoW.Views
{
    public sealed partial class ReportThreadContentDialog : ContentDialog
    {
        public ReportThreadContentDialog()
        {
            InitializeComponent();
            RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;
        }

        private async void ContentDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var threadId = GlobalState.ObservableObject.ThreadId;
            var reportReason = TextBoxReportReason.Text;

            var newReportThreadContent = $">>{threadId}\n{reportReason}";

            await AnoBbsApiClient.PostNewReport(newReportThreadContent);
        }
    }
}
