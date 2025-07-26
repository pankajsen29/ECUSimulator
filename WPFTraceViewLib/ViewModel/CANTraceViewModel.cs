using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using CommonHwLib;
using HwSettingsLib;
using WPFTraceViewLib.Command;

namespace WPFTraceViewLib.ViewModel
{
    internal class CANTraceViewModel : ViewModelBase
    {
        private static CommunicationManager _comManager;

        public ObservableCollection<CANData> CANMessageCollection { get; set; }
        public ObservableCollection<CANData> CANMessageCollection_Overwrite { get; set; }
        public ObservableCollection<CANData> CurrentCANMessageCollection => IsOverwriteEnabled ? CANMessageCollection_Overwrite : CANMessageCollection;

        private bool _isOverwriteEnabled = false;
        public bool IsOverwriteEnabled
        {
            get => _isOverwriteEnabled;
            set
            {
                if (_isOverwriteEnabled != value)
                {
                    _isOverwriteEnabled = value;
                    OnPropertyChanged(nameof(IsOverwriteEnabled));
                    OnPropertyChanged(nameof(CurrentCANMessageCollection));
                }
            }
        }

        private bool _isLoggingEnabled = false;
        public bool IsLoggingEnabled
        {
            get => _isLoggingEnabled;
            set
            {
                _isLoggingEnabled = value;
                OnPropertyChanged();
            }
        }
        public ConcurrentQueue<CANData> LogCANDataQueue { get; set; }

        public static Dispatcher UiDispatcher { get; private set; }

        public ICommand ClearTraceCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CANTraceViewModel()
        {
            UiDispatcher = Dispatcher.CurrentDispatcher; // store for later use
            ClearTraceCommand = new RelayCommand(new Action<object?>(HandleClearTraceCommand));
            CANMessageCollection = new ObservableCollection<CANData>();
            CANMessageCollection_Overwrite = new ObservableCollection<CANData>();
            LogCANDataQueue = new ConcurrentQueue<CANData>();
        }

        public bool OnStart(CommunicationManager comManager)
        {
            _comManager = comManager;
            _comManager.NotifyTraceView += DisplayInTraceView;
            return true;
        }

        private void DisplayInTraceView(CANData data)
        {
            UiDispatcher.BeginInvoke(() =>
            {
                CANMessageCollection.Add(data);
                AddOrUpdate(CANMessageCollection_Overwrite, data, x => x.Id == data.Id);
            });
            if (IsLoggingEnabled)
            {
                LogCANDataQueue.Enqueue(data);
            }
        }

        private void AddOrUpdate<T>(ObservableCollection<T> collection, T newItem, Func<T, bool> predicate)
        {
            var existingItem = collection.FirstOrDefault(predicate);
            if (existingItem != null)
            {
                // remove and re-add to trigger collection changed
                int index = collection.IndexOf(existingItem);
                collection[index] = newItem;
            }
            else
            {
                collection.Add(newItem);
            }
        }

        private void HandleClearTraceCommand(object? obj)
        {
            //clear the collections
            CANMessageCollection.Clear();
            CANMessageCollection_Overwrite.Clear();
        }
    }
}
