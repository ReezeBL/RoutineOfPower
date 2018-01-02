using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace RoutineOfPower.Views
{
    public abstract class AbstractViewer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}