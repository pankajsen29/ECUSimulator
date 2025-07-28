using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using Timer = System.Timers.Timer;
using CommonHwLib;
using HwSettingsLib;
using WPFTraceViewLib.Command;
using LoggerLib;

namespace WPFTraceViewLib.ViewModel
{
    internal class CANTraceViewModel : ViewModelBase
    {
        private static CommunicationManager? _comManager;

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
                EnableLogging();
            }
        }
        public ConcurrentQueue<CANData> LogCANDataQueue { get; set; }

        public static Dispatcher? UiDispatcher { get; private set; }

        public ICommand ClearTraceCommand { get; private set; }

        private Logger? CANLogger = null;
        private Timer? CANLogTimer = null;

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
            if (null != _comManager)
            {
                _comManager.NotifyTraceView += DisplayInTraceView;
                InitCANLogger(comManager.CANLogFile);
            }
            return true;
        }       

        private void DisplayInTraceView(CANData data)
        {
            if (null != UiDispatcher)
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

        private void InitCANLogger(string canlogfile)
        {
            CANLogger = new Logger(canlogfile);

            CANLogTimer = new Timer();
            CANLogTimer.Interval = 1;
            CANLogTimer.AutoReset = false;
            CANLogTimer.Elapsed += delegate
            {
                try
                {
                    CANLogTimerTick();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    CANLogTimer.Start();
                }
            };
            CANLogTimer.Enabled = false;
        }

        private void CANLogTimerTick()
        {
            if (IsLoggingEnabled && LogCANDataQueue.Count > 0)
            {
                while (LogCANDataQueue.TryDequeue(out CANData? data) && null != data)
                {
                    var message = string.Format("{0} {1} {2} {3}", data.Timestamp.PadRight(30, ' '), data.Id.ToString().PadRight(15, ' '), data.Dlc.ToString().PadRight(5, ' '), BitConverter.ToString(data.Data).Replace("-", " "));
                    CANLogger?.WriteToLog(message, false);
                }
            }
        }

        private void EnableLogging()
        {
            if (IsLoggingEnabled)
            {
                if (null != CANLogTimer) CANLogTimer.Enabled = true;
            }
            else
            {
                if (null != CANLogTimer) CANLogTimer.Enabled = false;
            }
        }
    }
}
