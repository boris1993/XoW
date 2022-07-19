namespace XoW.Models
{
    public class ObservableForumName : ObservableObject
    {
        private string _forumName;

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
    }
}
