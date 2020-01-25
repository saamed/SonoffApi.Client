using System.Threading.Tasks;
using SonoffApi.Client.Data;

namespace SonoffApi.Client
{
    public interface ISonoffClient
    {
        Task TurnSwitchOnAsync(string address, int port, string deviceId);
        Task TurnSwitchOffAsync(string address, int port, string deviceId);
        Task SetPowerOnStateAsync(string address, int port, string deviceId, PowerOnState powerOnState);
        Task<WifiSignalStrengthData> GetWifiSignalStrengthAsync(string address, int port, string deviceId);
        Task SetInchingAsync(string address, int port, string deviceId, State pulse, long pulseWidth);
        Task SetWiFiSettingsAsync(string address, int port, string deviceId, string ssid, string password);
        Task UnlockOTAAsync(string address, int port, string deviceId);
        Task OTAFlashAsync(string address, int port, string deviceId, string downloadUrl, string sha256sum);
        Task<DeviceInfoData> GetDeviceInfoAsync(string address, int port, string deviceId);
    }
}