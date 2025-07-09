using System.IO;
using System.Windows.Input;
using System.Collections.ObjectModel;
using CommonHwLib;
using UtilityLib;
using HardwareDriverLayer.HwSettings;
using WPFComSettingsViewLib.Command;
using PropertyDescriptor = WPFComSettingsViewLib.Model.PropertyDescriptor;

namespace WPFComSettingsViewLib.ViewModel
{
    public class CommunicationSettingsViewModel : ViewModelBase
    {
        private static CommunicationManager _comManager;

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

        private bool _isComSettingsJSONFileFound = false;
        public bool IsComSettingsJSONFileFound
        {
            get => _isComSettingsJSONFileFound;
            set
            {
                _isComSettingsJSONFileFound = value;
                OnPropertyChanged();
            }
        }

        public CommunicationSettings ComSettings
        {
            get { return _comManager.ComSettings; }
        }

        private ObservableCollection<PropertyDescriptor> _comSettingsCollection;
        /// <summary>
        /// This property also has to raise OnPropertyChanged(), bcause a new collection gets assigned after deserialization,
        /// without this the DataGrid will not be notified
        /// </summary>
        public ObservableCollection<PropertyDescriptor> ComSettingsCollection
        {
            get => _comSettingsCollection;
            set
            {
                _comSettingsCollection = value;
                OnPropertyChanged();
            }
        }


        public ICommand SaveComSettingsCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CommunicationSettingsViewModel()
        {
            SaveComSettingsCommand = new RelayCommand(new Action<object?>(HandleSaveComSettingsCommand));            
        }

        public bool OnStart(CommunicationManager comManager)
        {
            _comManager = comManager;
            CommunicationSettingsJSONFile = _comManager.CommunicationSettingsFile;
            Task.Run(async () => await LoadComSettings());
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task LoadComSettings()
        {
            if (File.Exists(CommunicationSettingsJSONFile))
            {
                IsComSettingsJSONFileFound = true; //if the file exists, give feedback to user
            }
            else
            {                
                IsComSettingsJSONFileFound = false; //if the file does not exist, give feedback to user saying the default settings are loaded
            }

            if (null != ComSettings)
            {
                //Handling of Source Object Updates: each PropertyDescriptor is created by connecting them to the original object's ("CommunicationSettings") properties.
                //also PropertyDescriptor implements INPC as we need UI updates to be propagated back
                ComSettingsCollection = new ObservableCollection<PropertyDescriptor>
                {
                    new PropertyDescriptor
                    {
                        Name = "CAN HW",
                        Value = ComSettings.ACTIVE_CAN_HW,
                        Setter = (val) => ComSettings.ACTIVE_CAN_HW = Enum.Parse<CAN_HW_INTERFACE>(val.ToString())
                    },
                    new PropertyDescriptor
                    {
                        Name = "CAN ENV",
                        Value = ComSettings.ACTIVE_CAN_ENV,
                        Setter = (val) => ComSettings.ACTIVE_CAN_ENV = Enum.Parse<CAN_ENV>(val.ToString())
                    },
                    new PropertyDescriptor
                    {
                        Name = "DATA FRAME",
                        Value = ComSettings.ACTIVE_DATA_FRAME,
                        Setter = (val) => ComSettings.ACTIVE_DATA_FRAME = Enum.Parse<CAN_DATA_FRAME_TYPE>(val.ToString())
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_ARB_BAUDRATE",
                        Value = ComSettings.ACTIVE_CANFD_SETTINGS.arb_baudrate,
                        Setter = (val) => ComSettings.ACTIVE_CANFD_SETTINGS.arb_baudrate = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_ARB_TSEG1",
                        Value = ComSettings.ACTIVE_CANFD_SETTINGS.arb_tseg1,
                        Setter = (val) => ComSettings.ACTIVE_CANFD_SETTINGS.arb_tseg1 = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_ARB_TSEG2",
                        Value = ComSettings.ACTIVE_CANFD_SETTINGS.arb_tseg2,
                        Setter = (val) => ComSettings.ACTIVE_CANFD_SETTINGS.arb_tseg2 = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_ARB_SJW",
                        Value = ComSettings.ACTIVE_CANFD_SETTINGS.arb_sjw,
                        Setter = (val) => ComSettings.ACTIVE_CANFD_SETTINGS.arb_sjw = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_DATA_BAUDRATE",
                        Value = ComSettings.ACTIVE_CANFD_SETTINGS.data_baudrate,
                        Setter = (val) => ComSettings.ACTIVE_CANFD_SETTINGS.data_baudrate = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_DATA_TSEG1",
                        Value = ComSettings.ACTIVE_CANFD_SETTINGS.data_tseg1,
                        Setter = (val) => ComSettings.ACTIVE_CANFD_SETTINGS.data_tseg1 = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_DATA_TSEG2",
                        Value = ComSettings.ACTIVE_CANFD_SETTINGS.data_tseg2,
                        Setter = (val) => ComSettings.ACTIVE_CANFD_SETTINGS.data_tseg2 = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_DATA_SJW",
                        Value = ComSettings.ACTIVE_CANFD_SETTINGS.data_sjw,
                        Setter = (val) => ComSettings.ACTIVE_CANFD_SETTINGS.data_sjw = Convert.ToUInt32(val)
                    }
                };
            }
        }

        private void HandleSaveComSettingsCommand(object? obj)
        {
            Task.Run(async () => await SaveComSettings());
        }

        /// <summary>
        /// saves the communication settings to a JSON file and 
        /// notifies the communication manager to re-initialize the driver with the new settings.
        /// </summary>
        /// <returns></returns>
        private async Task SaveComSettings()
        {
            //save the json file
            if (null != ComSettings)
            {
                await JsonSerializationHelper.Serialize(CommunicationSettingsJSONFile, ComSettings);
            }

            //notify to re - init the CAN driver with the new settings
            if (_comManager != null)
            {
                _comManager.NotifyUpdateOfCommunicationSettings();
            }
        }
    }
}
