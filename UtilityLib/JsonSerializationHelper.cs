using Newtonsoft.Json;

namespace UtilityLib
{
    public static class JsonSerializationHelper
    {
        static JsonSerializer _serializer = null;
        static JsonSerializationHelper()
        {
            _serializer = new JsonSerializer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<bool> Serialize(string file, object content)
        {
            bool bSerializeStatus = true;
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(file)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file));
                }
                using (StreamWriter sw = new StreamWriter(file, false))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    await Task.Run(() => _serializer.Serialize(writer, content));
                }
            }
            catch (IOException ioexp)
            {
                bSerializeStatus = false;
            }
            catch (Exception exp)
            {
                bSerializeStatus = false;
            }
            return bSerializeStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<object> Deserialize<T>(string file)
        {
            object reqresObj = null;
            try
            {
                using (TextReader tr = File.OpenText(file))
                using (JsonTextReader reader = new JsonTextReader(tr))
                {
                    reqresObj = await Task.Run(() => _serializer.Deserialize<T>(reader));
                }
            }
            catch (IOException ioexp)
            {
                reqresObj = null;
            }
            catch (Exception exp)
            {
                reqresObj = null;
            }
            return reqresObj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<string> SerializeObject<T>(T obj, bool toFormat)
        {
            var jsonString = string.Empty;
            Formatting formatting = toFormat ? Formatting.Indented : Formatting.None;
            try
            {
                jsonString = await Task.Run(() => JsonConvert.SerializeObject(obj, formatting));
            }
            catch (IOException ioexp)
            {
                jsonString = string.Empty;
            }
            catch (Exception exp)
            {
                jsonString = string.Empty;
            }
            return jsonString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static async Task<object> DeserializeObject<T>(string jsonString)
        {
            object reqresObj = null;
            try
            {
                reqresObj = await Task.Run(() => JsonConvert.DeserializeObject<T>(jsonString));
            }
            catch (IOException ioexp)
            {
                reqresObj = null;
            }
            catch (Exception exp)
            {
                reqresObj = null;
            }
            return reqresObj;
        }
    }
}
