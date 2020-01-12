using System.Threading.Tasks;
using SonoffApi.Client.Data;

namespace SonoffApi.Client
{
    public interface ISonoffClient
    {
        Task TurnSwitchOnAsync(string deviceId);
        Task TurnSwitchOffAsync(string deviceId);
        Task SetPowerOnStateAsync(string deviceId, PowerOnState powerOnState);
        Task<WifiSignalStrengthData> GetWifiSignalStrengthAsync(string deviceId);
        Task SetInchingAsync(string deviceId, State pulse, long pulseWidth);
        Task SetWiFiSettingsAsync(string deviceId, string ssid, string password);
        Task UnlockOTAAsync(string deviceId);
        Task OTAFlashAsync(string deviceId, string downloadUrl, string sha256sum);
        Task<DeviceInfoData> GetDeviceInfoAsync(string deviceId);
    }
}