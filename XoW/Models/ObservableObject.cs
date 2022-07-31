using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;

namespace XoW.Models
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SolidColorBrush _backgroundAndBorderColorBrush;
        private SolidColorBrush _listViewBackgroundColorBrush;
        private string _currentCookie;
        private string _threadId;
        private string _forumName;
        private string _subscriptionId;


        public SolidColorBrush BackgroundAndBorderColorBrush
        {
            get => _backgroundAndBorderColorBrush;
            set
            {
                if (value != _backgroundAndBorderColorBrush)
                {
                    _backgroundAndBorderColorBrush = value;
                    OnPropertyChanged();
                }
            }
        }

        public SolidColorBrush ListViewBackgroundColorBrush
        {
            get => _listViewBackgroundColorBrush;
            set
            {
                if (value != _listViewBackgroundColorBrush)
                {
                    _listViewBackgroundColorBrush = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentCookie
        {
            get => _currentCookie;
            set
            {
                if (value != _currentCookie)
                {
                    _currentCookie = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ThreadId
        {
            get { return _threadId; }
            set
            {
                if (_threadId != value)
                {
                    _threadId = $"No.{value}";
                    OnPropertyChanged();
                }
            }
        }

        public string ForumName
        {
            get => _forumName;
            set
            {
                if (_forumName != value)
                {
                    _forumName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SubscriptionId
        {
            get => _subscriptionId;
            set
            {
                if (_subscriptionId != value)
                {
                    _subscriptionId = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
