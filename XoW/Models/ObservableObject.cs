using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XoW.Models
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        protected ObservableObject()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
