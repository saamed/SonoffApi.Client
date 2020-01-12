using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SonoffApi.Client.Data;

namespace SonoffApi.Client
{
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

        public Task<DeviceInfoData> GetDeviceInfoAsync(string deviceId)
        {
            return DispatchRequestWithResponseAsync<EmptyData, DeviceInfoData>(SonoffMethods.DeviceInfo, deviceId, new EmptyData());
        }

        public Task<WifiSignalStrengthData> GetWifiSignalStrengthAsync(string deviceId)
        {
            return DispatchRequestWithResponseAsync<EmptyData, WifiSignalStrengthData>(SonoffMethods.WifiSignalStrength, deviceId, new EmptyData());
        }

        public Task OTAFlashAsync(string deviceId, string downloadUrl, string sha256sum)
        {
            return DispatchRequestAsync<OTAFlashData>(SonoffMethods.OTAFlash, deviceId, new OTAFlashData() { DownloadUrl = downloadUrl, Sha256Sum = sha256sum });
        }

        public Task SetInchingAsync(string deviceId, State pulse, long pulseWidth)
        {
            return DispatchRequestAsync<InchingData>(SonoffMethods.Inching, deviceId, new InchingData() { Pulse = pulse, PulseWidth = pulseWidth });
        }

        public Task SetPowerOnStateAsync(string deviceId, PowerOnState powerOnState)
        {
            return DispatchRequestAsync<PowerOnStateData>(SonoffMethods.PowerOnState, deviceId, new PowerOnStateData() { Startup = powerOnState });
        }

        public Task SetWiFiSettingsAsync(string deviceId, string ssid, string password)
        {
            return DispatchRequestAsync<WifiSettingsData>(SonoffMethods.WifiSettings, deviceId, new WifiSettingsData() { SSID = ssid, Password = password });
        }

        public Task TurnSwitchOffAsync(string deviceId)
        {
            return DispatchRequestAsync<SwitchData>(SonoffMethods.SwitchOnOff, deviceId, new SwitchData() { Switch = State.Off });
        }

        public Task TurnSwitchOnAsync(string deviceId)
        {
            return DispatchRequestAsync<SwitchData>(SonoffMethods.SwitchOnOff, deviceId, new SwitchData() { Switch = State.On });
        }

        public Task UnlockOTAAsync(string deviceId)
        {
            return DispatchRequestAsync<EmptyData>(SonoffMethods.UnlockOTA, deviceId, new EmptyData());
        }

        protected Task DispatchRequestAsync<TReq>(SonoffMethods method, string deviceId, TReq request)
            where TReq : class, new()
        {
            return DispatchRequestWithResponseAsync<TReq, EmptyData>(method, deviceId, request, false);
        }

        protected Task<TResp> DispatchRequestWithResponseAsync<TReq, TResp>(SonoffMethods method, string deviceId, TReq request)
                    where TReq : class, new()
                    where TResp : class, new()
        {
            return DispatchRequestWithResponseAsync<TReq, TResp>(method, deviceId, request, true);
        }

        protected async Task<TResp> DispatchRequestWithResponseAsync<TReq, TResp>(SonoffMethods method, string deviceId, TReq request, bool handleReturnValue)
            where TReq : class, new()
            where TResp : class, new()
        {
            var methodUrl = method.GetDescription();
            var url = String.Format(methodUrl, _deviceUrl);

            using (var message = new HttpRequestMessage())
            {
                message.RequestUri = new System.Uri(url);
                message.Method = HttpMethod.Post;

                var requestObject = new DeviceRequest<TReq> { DeviceId = deviceId };

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

                        if (!handleReturnValue)
                        {
                            return null;
                        }

                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var contentObject = JsonConvert.DeserializeObject<DeviceResponse<TResp>>(content);

                        return contentObject.Data;
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