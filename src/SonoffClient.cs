using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SonoffApi.Client.Data;

namespace SonoffApi.Client
{
    public class SonoffClient : ISonoffClient
    {
        private readonly HttpClient _client;

        public SonoffClient(HttpClient client)
        {
            _client = client;
        }

        public Task<DeviceInfoData> GetDeviceInfoAsync(string address, int port, string deviceId)
        {
            return DispatchRequestWithResponseAsync<EmptyData, DeviceInfoData>(address, port, SonoffMethods.DeviceInfo, deviceId, new EmptyData());
        }

        public Task<WifiSignalStrengthData> GetWifiSignalStrengthAsync(string address, int port, string deviceId)
        {
            return DispatchRequestWithResponseAsync<EmptyData, WifiSignalStrengthData>(address, port, SonoffMethods.WifiSignalStrength, deviceId, new EmptyData());
        }

        public Task OTAFlashAsync(string address, int port, string deviceId, string downloadUrl, string sha256sum)
        {
            return DispatchRequestAsync(address, port, SonoffMethods.OTAFlash, deviceId, new OTAFlashData() { DownloadUrl = downloadUrl, Sha256Sum = sha256sum });
        }

        public Task SetInchingAsync(string address, int port, string deviceId, State pulse, long pulseWidth)
        {
            return DispatchRequestAsync(address, port, SonoffMethods.Inching, deviceId, new InchingData() { Pulse = pulse, PulseWidth = pulseWidth });
        }

        public Task SetPowerOnStateAsync(string address, int port, string deviceId, PowerOnState powerOnState)
        {
            return DispatchRequestAsync(address, port, SonoffMethods.PowerOnState, deviceId, new PowerOnStateData() { Startup = powerOnState });
        }

        public Task SetWiFiSettingsAsync(string address, int port, string deviceId, string ssid, string password)
        {
            return DispatchRequestAsync(address, port, SonoffMethods.WifiSettings, deviceId, new WifiSettingsData() { SSID = ssid, Password = password });
        }

        public Task TurnSwitchOffAsync(string address, int port, string deviceId)
        {
            return DispatchRequestAsync(address, port, SonoffMethods.SwitchOnOff, deviceId, new SwitchData() { Switch = State.off });
        }

        public Task TurnSwitchOnAsync(string address, int port, string deviceId)
        {
            return DispatchRequestAsync(address, port, SonoffMethods.SwitchOnOff, deviceId, new SwitchData() { Switch = State.on });
        }

        public Task UnlockOTAAsync(string address, int port, string deviceId)
        {
            return DispatchRequestAsync(address, port, SonoffMethods.UnlockOTA, deviceId, new EmptyData());
        }

        protected Task DispatchRequestAsync<TReq>(string address, int port, SonoffMethods method, string deviceId, TReq request)
            where TReq : class, new()
        {
            return DispatchRequestWithResponseAsync<TReq, EmptyData>(address, port, method, deviceId, request, false);
        }

        protected Task<TResp> DispatchRequestWithResponseAsync<TReq, TResp>(string address, int port, SonoffMethods method, string deviceId, TReq request)
                    where TReq : class, new()
                    where TResp : class, new()
        {
            return DispatchRequestWithResponseAsync<TReq, TResp>(address, port, method, deviceId, request, true);
        }

        protected async Task<TResp> DispatchRequestWithResponseAsync<TReq, TResp>(string address, int port, SonoffMethods method, string deviceId, TReq request, bool handleReturnValue)
            where TReq : class, new()
            where TResp : class, new()
        {
            var methodUrl = method.GetDescription();
            var url = string.Format(methodUrl, address, port);

            using (var message = new HttpRequestMessage())
            {
                message.RequestUri = new Uri(url);
                message.Method = HttpMethod.Post;

                var requestObject = new DeviceRequest<TReq> { DeviceId = deviceId, Data = request };

                var requestContent = JsonConvert.SerializeObject(requestObject);

                message.Content = new StringContent(requestContent, Encoding.UTF8, "application/json");

                try
                {
                    using (var response = await _client.SendAsync(message, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Status Code =  {response.StatusCode}. Requires custom exception");
                        }

                        if (!handleReturnValue)
                        {
                            return null;
                        }

                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var stringObject = JsonConvert.DeserializeObject<DeviceResponse<string>>(content);
                        var data = JsonConvert.DeserializeObject<TResp>(stringObject.Data);

                        return data;
                    }
                }
                catch (TaskCanceledException)
                {
                    throw new TimeoutException($"Timeout getting response from {url}, client timeout is set to {_client.Timeout}.");
                }
            }
        }

    }
}