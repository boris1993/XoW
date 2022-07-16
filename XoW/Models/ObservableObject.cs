using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XoW.Models
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        protected ObservableObject() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
