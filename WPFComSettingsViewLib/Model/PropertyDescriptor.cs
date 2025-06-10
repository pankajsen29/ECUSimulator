using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComSettingsViewLib.Model
{
    public class PropertyDescriptor : NotifyPropertyChangedBase
    {
        public string Name { get; set; }

        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
}
