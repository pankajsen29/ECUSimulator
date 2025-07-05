using CommonHwLib;
using HardwareDriverLayer.HwSettings;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using UtilityLib;
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
            Task.Run(async () => await LoadComSettings());
        }

        public bool OnStart(CommunicationManager comManager)
        {
            _comManager = comManager;
            CommunicationSettingsJSONFile = _comManager.CommunicationSettingsFile;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task LoadComSettings()
        {
            //try to load the Settings from comSettings.json file in public Documents folder.
            //if file exists, deserialize it and load it
            if (File.Exists(CommunicationSettingsJSONFile))
            {
                IsComSettingsJSONFileFound = true;
                _comSettings = (CommunicationSettings)await JsonSerializationHelper.Deserialize<CommunicationSettings>(CommunicationSettingsJSONFile);
            }
            else
            {
                //if the file does not exist, give feedback to user and load the default settings
                IsComSettingsJSONFileFound = false;
                LoadDefaultComSettings();
            }

            if (null != _comSettings)
            {
                //Handling of Source Object Updates: each PropertyDescriptor is created by connecting them to the original object's ("CommunicationSettings") properties.
                //also PropertyDescriptor implements INPC as we need UI updates to be propagated back
                ComSettingsCollection = new ObservableCollection<PropertyDescriptor>
                {
                    new PropertyDescriptor
                    {
                        Name = "CAN HW",
                        Value = _comSettings.ACTIVE_CAN_HW,
                        Setter = (val) => _comSettings.ACTIVE_CAN_HW = Enum.Parse<CAN_HW_INTERFACE>(val.ToString())
                    },
                    new PropertyDescriptor
                    {
                        Name = "CAN ENV",
                        Value = _comSettings.ACTIVE_CAN_ENV,
                        Setter = (val) => _comSettings.ACTIVE_CAN_ENV = Enum.Parse<CAN_ENV>(val.ToString())
                    },
                    new PropertyDescriptor
                    {
                        Name = "DATA FRAME",
                        Value = _comSettings.ACTIVE_DATA_FRAME,
                        Setter = (val) => _comSettings.ACTIVE_DATA_FRAME = Enum.Parse<CAN_DATA_FRAME_TYPE>(val.ToString())
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_ARB_BAUDRATE",
                        Value = _comSettings.ACTIVE_CANFD_SETTINGS.arb_baudrate,
                        Setter = (val) => _comSettings.ACTIVE_CANFD_SETTINGS.arb_baudrate = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_ARB_TSEG1",
                        Value = _comSettings.ACTIVE_CANFD_SETTINGS.arb_tseg1,
                        Setter = (val) => _comSettings.ACTIVE_CANFD_SETTINGS.arb_tseg1 = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_ARB_TSEG2",
                        Value = _comSettings.ACTIVE_CANFD_SETTINGS.arb_tseg2,
                        Setter = (val) => _comSettings.ACTIVE_CANFD_SETTINGS.arb_tseg2 = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_ARB_SJW",
                        Value = _comSettings.ACTIVE_CANFD_SETTINGS.arb_sjw,
                        Setter = (val) => _comSettings.ACTIVE_CANFD_SETTINGS.arb_sjw = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_DATA_BAUDRATE",
                        Value = _comSettings.ACTIVE_CANFD_SETTINGS.data_baudrate,
                        Setter = (val) => _comSettings.ACTIVE_CANFD_SETTINGS.data_baudrate = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_DATA_TSEG1",
                        Value = _comSettings.ACTIVE_CANFD_SETTINGS.data_tseg1,
                        Setter = (val) => _comSettings.ACTIVE_CANFD_SETTINGS.data_tseg1 = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_DATA_TSEG2",
                        Value = _comSettings.ACTIVE_CANFD_SETTINGS.data_tseg2,
                        Setter = (val) => _comSettings.ACTIVE_CANFD_SETTINGS.data_tseg2 = Convert.ToUInt32(val)
                    },
                    new PropertyDescriptor
                    {
                        Name = "CANFD_DATA_SJW",
                        Value = _comSettings.ACTIVE_CANFD_SETTINGS.data_sjw,
                        Setter = (val) => _comSettings.ACTIVE_CANFD_SETTINGS.data_sjw = Convert.ToUInt32(val)
                    }
                };
            }
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

        /// <summary>
        /// saves the communication settings to a JSON file and 
        /// notifies the communication manager to re-initialize the driver with the new settings.
        /// </summary>
        /// <returns></returns>
        private async Task SaveComSettings()
        {
            //save the json file
            if (null != _comSettings)
            {
                await JsonSerializationHelper.Serialize(CommunicationSettingsJSONFile, _comSettings);
            }

            //notify to re - init the CAN driver with the new settings
            if (_comManager != null)
            {
                _comManager.NotifyUpdateOfCommunicationSettings();
            }
        }
    }
}
