using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.classes
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T target, T source, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(target, source)) return true;

            target = source;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
            return true;
        }

        protected void OnPropertyChanged(object prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(prop)));
        }
    }
}
