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

        public async Task<DeviceInfo> GetDeviceInfoAsync(string deviceId)
        {
            var response = await DispatchRequestWithResponseAsync<EmptyData, DeviceInfo>(SonoffMethods.GetDeviceInfo, deviceId, new EmptyData());

            return response;
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

        public async Task TurnSwitchOffAsync(string deviceId)
        {
            await DispatchRequestAsync<SwitchData>(SonoffMethods.GetDeviceInfo, deviceId, new SwitchData() { Switch = State.Off });
        }

        public async Task TurnSwitchOnAsync(string deviceId)
        {
            await DispatchRequestAsync<SwitchData>(SonoffMethods.GetDeviceInfo, deviceId, new SwitchData() { Switch = State.On });
        }

        public Task UnlockOTAAsync(string deviceId)
        {
            throw new System.NotImplementedException();
        }

        protected async Task DispatchRequestAsync<TReq>(SonoffMethods method, string deviceId, TReq request)
            where TReq : class, new()
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
                    }
                }
                catch (TaskCanceledException)
                {
                    throw new TimeoutException($"Timeout getting response from {url}, client timeout is set to {_client.Timeout}.");
                }
            }
        }

        protected async Task<TResp> DispatchRequestWithResponseAsync<TReq, TResp>(SonoffMethods method, string deviceId, TReq request)
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