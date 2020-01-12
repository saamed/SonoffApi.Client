using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

    public class SonoffClient : ISonoffClient, IDisposable
    {
        private readonly string _deviceUrl;
        private readonly HttpClient _client;

        public SonoffClient(string deviceUrl)
        {
            _deviceUrl = deviceUrl;
            _client = new HttpClient();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task<DeviceInfo> GetDeviceInfoAsync(string deviceId)
        {
            var url = $"http://{_deviceUrl}/zeroconf/info";
            using (var message = new HttpRequestMessage())
            {
                message.RequestUri = new System.Uri(url);
                message.Method = HttpMethod.Post;

                var requestObject = new DeviceRequest<EmptyData> { DeviceId = deviceId };

                var requestContent = JsonConvert.SerializeObject(requestObject);

                message.Content = new StringContent(requestContent, Encoding.UTF8, "application/json");

                try
                {
                    using (var response = await _client.SendAsync(message, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Status Code =  {response.StatusCode}. Requires custom exception");
                        }

                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var contentObject = JsonConvert.DeserializeObject<DeviceResponse<DeviceInfo>>(content);

                        return contentObject.Data;
                    }
                }
                catch (TaskCanceledException)
                {
                    throw new TimeoutException($"Timeout getting response from {url}, client timeout is set to {_client.Timeout}.");
                }
            }
        }

        public Task<int> GetWifiSignalStrengthAsync(string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public Task OTAFleshAsync(string deviceId, string downloadUrl, string sha2356sum)
        {
            throw new System.NotImplementedException();
        }

        public Task SetInchingAsync(string deviceId, State pulse, long pulseWidth)
        {
            throw new System.NotImplementedException();
        }

        public Task SetPowerOnStateAsync(string deviceId, PowerOnState powerOnState)
        {
            throw new System.NotImplementedException();
        }

        public Task SetWiFiSettingsAsync(string deviceId, string ssid, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task TurnSwitchOffAsync(string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public Task TurnSwitchOnAsync(string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public Task UnlockOTAAsync(string deviceId)
        {
            throw new System.NotImplementedException();
        }
    }
}