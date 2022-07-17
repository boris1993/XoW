namespace XoW.Models
{
    public class ObservableSubscriptionId : ObservableObject
    {
        private string _subscriptionId;

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
    }
}
