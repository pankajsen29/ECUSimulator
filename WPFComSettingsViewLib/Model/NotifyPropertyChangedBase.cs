using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFComSettingsViewLib.Model
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Base class with the methods implemented from INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Called when the property <paramref name="propertyName" /> has changed.        
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>        
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
