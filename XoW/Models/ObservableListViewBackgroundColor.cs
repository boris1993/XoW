using Windows.UI.Xaml.Media;

namespace XoW.Models
{
    public class ObservableListViewBackgroundColor : ObservableObject
    {
        private SolidColorBrush _colorBrush;

        public SolidColorBrush ColorBrush
        {
            get => _colorBrush;
            set
            {
                if (value != _colorBrush)
                {
                    _colorBrush = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
