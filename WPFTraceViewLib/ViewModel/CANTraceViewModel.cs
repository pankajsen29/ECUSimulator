using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFTraceViewLib.Command;
using WPFTraceViewLib.Model;
using CommonHwLib;

namespace WPFTraceViewLib.ViewModel
{
    internal class CANTraceViewModel : ViewModelBase
    {
        private static CommunicationManager _comManager;

        public ObservableCollection<CANMessage> CANMessageCollection;
        public ObservableCollection<CANMessage> CANMessageCollection_Overwrite;

        private bool _isOverwriteEnabled = false;
        public bool IsOverwriteEnabled
        {
            get => _isOverwriteEnabled;
            set
            {
                _isOverwriteEnabled = value;
                OnPropertyChanged();
            }
        }

        public ICommand ClearTraceCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CANTraceViewModel()
        {
            ClearTraceCommand = new RelayCommand(new Action<object?>(HandleClearTraceCommand));
        }

        public bool OnStart(CommunicationManager comManager)
        {
            _comManager = comManager;
            return true;
        }

        private void HandleClearTraceCommand(object? obj)
        {
            //clear the collections
        }

    }
}
