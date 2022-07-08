using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XoW.Services;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace XoW.Views
{
    public sealed partial class ReportThreadContentDialog : ContentDialog
    {
        private readonly string _threadId;

        public ReportThreadContentDialog(string threadId)
        {
            InitializeComponent();
            RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;
            _threadId = threadId;
        }

        private async void ContentDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var threadId = _threadId;
            var reportReason = TextBoxReportReason.Text;

            var newReportThreadContent = $">>{threadId}\n{reportReason}";

            await AnoBbsApiClient.PostNewReport(newReportThreadContent);
        }
    }
}
