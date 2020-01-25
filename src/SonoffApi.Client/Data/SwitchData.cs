using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SonoffApi.Client.Data
{
    internal class SwitchData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public State Switch { get; set; }

    }
}
