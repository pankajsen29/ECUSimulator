using UtilityLib;

namespace MessageDesignerLib
{
    public class MessageConfigManager
    {
        /// <summary>
        /// basic message for user to use as a template for creating new messages.
        /// </summary>
        /// <returns></returns>
        public async Task<string> LoadMessageDefinition()
        {
            var message = new Message
            {
                Name = "cmd_1",
                Request = new Request
                {
                    Id = "0x11",
                    Payload = 16,
                    IsCanFdFrame = true,
                    RequestHexData = new RequestHexData
                    {
                        IsRuleBased = false,
                        DataString = "01 02 03 04 05 00 00 00 01 02 03 04 05 00 00 00",
                        DataPatterns = new List<Pattern>
                        {
                            new Pattern { Index = 0, RangeStart = 0, RangeEnd = 0 }
                        }
                    }
                },
                Response = new Response
                {
                    Id = "0x12",
                    Payload = 32,
                    IsCanFdFrame = true,
                    ResponseHexData = new ResponseHexData
                    {
                        IsRuleBased = false,
                        DataString = "01 02 03 04 05 00 00 00 01 02 03 04 05 00 00 00 01 02 03 04 05 00 00 00 01 02 03 04 05 00 00 FF",
                        DataSubstitutions = new List<Substitution>
                        {
                            new Substitution { SourceDataIndexFromRequest = 0, DestinationDataIndexToResponse = 0 }
                        }
                    }
                }
            };

            return (string)(await JsonSerializationHelper.SerializeObject<Message>(message, true));
        }

        /// <summary>
        /// loads the given message configuration JSON file and returns the content as a JSON string.
        /// </summary>
        /// <param name="messageConfigFile"></param>
        /// <returns></returns>
        public async Task<string> LoadMessageConfig(string messageConfigFile)
        {
            var root = (Root)await JsonSerializationHelper.Deserialize<Root>(messageConfigFile);
            if (root == null)
            {
                return string.Empty;
            }
            return (string)(await JsonSerializationHelper.SerializeObject<Root>(root, true));
        }

        /// <summary>
        /// adds a new json-string message to the existing messages configuration file, and returns true if successful.
        /// </summary>
        /// <param name="newMessageJson"></param>
        /// <param name="messageConfigFile"></param>
        /// <returns></returns>
        public async Task<bool> AddToMessagesConfig(string newMessageJson, string messageConfigFile)
        {
            var message = (Message)await JsonSerializationHelper.DeserializeObject<Message>(newMessageJson);
            if (null == message)
            {
                return false;
            }
            Root? root = null;
            if (File.Exists(messageConfigFile))
            {
                root = (Root)await JsonSerializationHelper.Deserialize<Root>(messageConfigFile);
                if (root == null)
                {
                    root = new Root();
                }
                if (root.Messages == null)
                {
                    root.Messages = new List<Message>();
                }
                root.Messages.Add(message);
            }
            else
            {
                root = new Root();
                root.Messages = new List<Message>();
                root.Messages.Add(message);
            }
            await JsonSerializationHelper.Serialize(messageConfigFile, root);

            root = null;
            root = (Root)await JsonSerializationHelper.Deserialize<Root>(messageConfigFile);
            if (root == null)
            {
                return false;
            }
            return true;
        }      
    }
}
