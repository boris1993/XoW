namespace XoW.Models
{
    internal class ObservableCurrentThreadPage : ObservableObject
    {
        private double _currentThreadPage;
        public double CurrentPage
        {
            get
            {
                return _currentThreadPage;
            }

            set
            {
                if (value != _currentThreadPage)
                {
                    _currentThreadPage = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
