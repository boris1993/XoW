namespace XoW.Models
{
    public class ObservableCurrentCookie : ObservableObject
    {
        private string _currentCookie;

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
    }
}
