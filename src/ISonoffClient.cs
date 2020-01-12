using System.IO;
using System.Net;
using System.Threading.Tasks;
using SonoffApi.Client.Data;

namespace SonoffApi.Client
{
    public interface ISonoffClient
    {
        Task TurnSwitchOnAsync(string deviceId);
        Task TurnSwitchOffAsync(string deviceId);
        Task SetPowerOnStateAsync(string deviceId, PowerOnState powerOnState);
        Task<int> GetWifiSignalStrengthAsync(string deviceId);
        Task SetInchingAsync(string deviceId, State pulse, long pulseWidth);
        Task SetWiFiSettingsAsync(string deviceId, string ssid, string password);
        Task UnlockOTAAsync(string deviceId);
        Task OTAFleshAsync(string deviceId, string downloadUrl, string sha2356sum);
        Task<DeviceInfo> GetDeviceInfoAsync(string deviceId);
    }
}