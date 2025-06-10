using CommonHwLib;
using HardwareDriverLayer.HwSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UtilityLib;
using Windows.Media.Devices;
using WPFComSettingsViewLib.Command;
using PropertyDescriptor = WPFComSettingsViewLib.Model.PropertyDescriptor;

namespace WPFComSettingsViewLib.ViewModel
{
    public class CommunicationSettingsViewModel : ViewModelBase
    {
        public static CommunicationManager _comManager;

        private CommunicationSettings _comSettings;

        private string _comSettingsJSONFile = string.Empty;
        public string CommunicationSettingsJSONFile
        {
            get { return _comSettingsJSONFile; }
            set
            {
                _comSettingsJSONFile = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Don't need ObservableCollection here, as we don't need to add/remove rows (here each property) dynamically.
        /// </summary>
        public List<PropertyDescriptor> ComSettingsCollection { get; set; }

        public ICommand SaveComSettingsCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CommunicationSettingsViewModel()
        {
            SaveComSettingsCommand = new RelayCommand(new Action<object?>(HandleSaveComSettingsCommand));
            Task.Run(async () => await LoadComSettings());
        }

        public bool OnStart(CommunicationManager comManager)
        {
            _comManager = comManager;
            return true;
        }

        private void LoadDefaultComSettings()
        {
            _comSettings = new CommunicationSettings
            {
                ACTIVE_CAN_HW = CAN_HW_INTERFACE.e_VECTOR_XL,
                ACTIVE_CAN_ENV = CAN_ENV.e_CANFD,
                ACTIVE_DATA_FRAME = CAN_DATA_FRAME_TYPE.e_FRAME_STD,
                ACTIVE_CANFD_SETTINGS = new CANFD_SETTINGS
                {
                    arb_baudrate = 500000,
                    arb_tseg1 = 7,
                    arb_tseg2 = 2,
                    arb_sjw = 2,
                    data_baudrate = 2000000,
                    data_tseg1 = 7,
                    data_tseg2 = 2,
                    data_sjw = 2
                }
            };
        }

        private void HandleSaveComSettingsCommand(object? obj)
        {
            Task.Run(async () => await SaveComSettings());
        }

        private async Task SaveComSettings()
        {
            //save the json file
            //var jsonSettings = (string)(await JsonSerializationHelper.SerializeObject<CommunicationSettings>(comSettings, true));

            //notify to re-init the CAN driver with the new settings
            //if (_comManager != null)
            //{
            //    _comManager.NotifyUpdateOfCommunicationSettings();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task LoadComSettings()
        {
            //todo: load the comSettings from a json file (if exists) in Documents folder.
            //if file exists, deserialize it and load it

            //if the file does not exist, inform the user and load the default settings
            LoadDefaultComSettings();

            //Handle Source Object Updates: Ensure your source object implements INPC if you need UI updates from code changes:
            ComSettingsCollection = new List<PropertyDescriptor>
            {
                new PropertyDescriptor { Name = "CAN HW", Value = _comSettings.ACTIVE_CAN_HW.ToString() },
                new PropertyDescriptor { Name = "CAN ENV", Value = _comSettings.ACTIVE_CAN_ENV.ToString() },
                new PropertyDescriptor { Name = "DATA FRAME", Value = _comSettings.ACTIVE_DATA_FRAME.ToString() },
                new PropertyDescriptor { Name = "arb_baudrate", Value = _comSettings.ACTIVE_CANFD_SETTINGS.arb_baudrate.ToString() },
                new PropertyDescriptor { Name = "arb_tseg1", Value = _comSettings.ACTIVE_CANFD_SETTINGS.arb_tseg1.ToString() },
                new PropertyDescriptor { Name = "arb_tseg2", Value = _comSettings.ACTIVE_CANFD_SETTINGS.arb_tseg2.ToString() },
                new PropertyDescriptor { Name = "arb_sjw", Value = _comSettings.ACTIVE_CANFD_SETTINGS.arb_sjw.ToString() },
                new PropertyDescriptor { Name = "data_baudrate", Value = _comSettings.ACTIVE_CANFD_SETTINGS.data_baudrate.ToString() },
                new PropertyDescriptor { Name = "data_tseg1", Value = _comSettings.ACTIVE_CANFD_SETTINGS.data_tseg1.ToString() },
                new PropertyDescriptor { Name = "data_tseg2", Value = _comSettings.ACTIVE_CANFD_SETTINGS.data_tseg2.ToString() },
                new PropertyDescriptor { Name = "data_sjw", Value = _comSettings.ACTIVE_CANFD_SETTINGS.data_sjw.ToString() }
            };
        }
    }
}
