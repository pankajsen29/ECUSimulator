using CommonHwLib;
using HwSettingsLib;
using UtilityLib;
using MessageDesignerLib;
using System.Collections.Concurrent;

namespace MessageProcessorLib
{
    public class MessageProcessor
    {
        private CommunicationManager _comManagerObj;
        private Root _messageConfigRoot;
        public ConcurrentQueue<CANData> CANTxDataQueue { get; set; }
        public string LastErrorMessage { get; private set; }

        public MessageProcessor(CommunicationManager ComManagerObj)
        {
            _comManagerObj = ComManagerObj;
            CANTxDataQueue = new ConcurrentQueue<CANData>();
        }

        /// <summary>
        /// loads the given message configuration JSON file
        /// </summary>
        /// <param name="messageConfigFile"></param>
        /// <returns></returns>
        public async Task<bool> LoadMessageConfig(string messageConfigFile)
        {
            _messageConfigRoot = (Root)await JsonSerializationHelper.Deserialize<Root>(messageConfigFile);
            return _messageConfigRoot != null;
        }

        /// <summary>
        /// Supported formats for RequestHexData.DataString and ResponseHexData.DataString:
        /// "01020304050000000102030405000000010203040500000001020304050000FF"
        /// "01 02 03 04 05 00 00 00 01 02 03 04 05 00 00 00 01 02 03 04 05 00 00 00 01 02 03 04 05 00 00 FF"
        /// "0x01 0x02 0x03 0x04 0x05 0x00 0x00 0x00 0x01 0x02 0x03 0x04 0x05 0x00 0x00 0x00 0x01 0x02 0x03 0x04 0x05 0x00 0x00 0x00 0x01 0x02 0x03 0x04 0x05 0x00 0x00 0xFF"
        /// "0x010x020x030x040x050x000x000x000x010x020x030x040x050x000x000x000x010x020x030x040x050x000x000x000x010x020x030x040x050x000x000xFF"
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void HandleReceivedCANMessage()
        {
            if (null != _comManagerObj && null != _messageConfigRoot)
            {
                while (_comManagerObj.ReceivedCANDataQueue.Count > 0)
                {
                    CANData? canData;
                    if (_comManagerObj.ReceivedCANDataQueue.TryDequeue(out canData) && canData != null)
                    {
                        try
                        {
                            foreach (var message in _messageConfigRoot.Messages)
                            {
                                var requestData = Convert.FromHexString(message.Request.RequestHexData.DataString.Replace(" ", "").Replace("0x", ""));
                                var responseData = Convert.FromHexString(message.Response.ResponseHexData.DataString.Replace(" ", "").Replace("0x", ""));
                                if (message.Request.Id.Equals(canData.Id.ToString()) && Array.Equals(requestData, canData.Data))
                                {
                                    CANTxDataQueue.Enqueue(new CANData
                                    {
                                        Id = uint.Parse(message.Response.Id.Replace("0x", "")),
                                        Payload = message.Response.Payload,
                                        IsCanFdFrame = message.Response.IsCanFdFrame,
                                        Data = responseData,
                                        Length = (uint)responseData.Length
                                    });
                                }
                            }
                        }
                        catch (ArgumentNullException argNullExp)
                        {
                            LastErrorMessage = $"ArgumentNullException: {argNullExp.Message}";
                        }
                        catch (FormatException formatExp)
                        {
                            LastErrorMessage = $"FormatException: {formatExp.Message}";
                        }
                        catch (OverflowException ofExp)
                        {
                            LastErrorMessage = $"OverflowException: {ofExp.Message}";
                        }
                        catch (Exception exp)
                        {
                            LastErrorMessage = $"Exception: {exp.Message}";
                        }
                    }
                }
            }
        }
    }
}
