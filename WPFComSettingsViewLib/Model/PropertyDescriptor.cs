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
                Setter?.Invoke(value); // Update source object
            }
        }
        public Action<object> Setter { get; set; }

    }
}
