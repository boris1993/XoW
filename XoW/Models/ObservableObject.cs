using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;

namespace XoW.Models
{
    public class ObservableObject : INotifyPropertyChanged
    {

        private SolidColorBrush _backgroundAndBorderColorBrush;
        private string _currentCookie;
        private string _forumName;
        private SolidColorBrush _listViewBackgroundColorBrush;
        private string _subscriptionId;
        private string _threadId;
        private int _currentPageNumber;

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
            get => _threadId;
            set
            {
                if (_threadId != value)
                {
                    _threadId = value;
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

        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            set
            {
                if (_currentPageNumber != value)
                {
                    _currentPageNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
