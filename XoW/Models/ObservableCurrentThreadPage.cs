namespace XoW.Models
{
    internal class ObservableCurrentThreadPage : ObservableObject
    {
        private int _currentThreadPage;
        public int CurrentPage
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
