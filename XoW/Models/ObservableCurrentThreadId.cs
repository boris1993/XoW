namespace XoW.Models
{
    public class ObservableCurrentThreadId : ObservableObject
    {
        private string _threadId;

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
    }
}
