using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SonoffApi.Client.Data
{
    internal class PowerOnStateData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PowerOnState Startup { get; set; }

    }
}
