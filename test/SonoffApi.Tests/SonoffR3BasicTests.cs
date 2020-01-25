using SonoffApi.Client;
using Xunit;
using Zeroconf;
using System.Linq;
using System.Threading.Tasks;
using SonoffApi.Client.Data;
using Xunit.Abstractions;
using System.Net.Http;
using System;

namespace SonoffApi.Tests
{
    public class SonoffR3BasicTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly HttpClient _httpClient;

        public SonoffR3BasicTests(ITestOutputHelper output)
        {
            this.output = output;
            _httpClient = new HttpClient();
        }

        [Fact(Skip = "A real device must be in DIY mode")]
        public async Task should_turn_switch_on()
        {
            var serviceData = await GetServiceData();
            var client = new SonoffClient(_httpClient);

            Exception exception = null;
            DeviceInfoData deviceInfo = null;
            try
            {
                await client.TurnSwitchOnAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);

                deviceInfo = await client.GetDeviceInfoAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
            Assert.NotNull(deviceInfo);
            Assert.Equal(State.on, deviceInfo.Switch);
        }

        [Fact(Skip = "A real device must be in DIY mode")]
        public async Task should_turn_switch_off()
        {
            var serviceData = await GetServiceData();
            var client = new SonoffClient(_httpClient);

            Exception exception = null;
            DeviceInfoData deviceInfo = null;
            try
            {
                await client.TurnSwitchOffAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);

                deviceInfo = await client.GetDeviceInfoAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
            Assert.NotNull(deviceInfo);
            Assert.Equal(State.off, deviceInfo.Switch);
        }

        [Fact(Skip = "A real device must be in DIY mode")]
        public async Task should_change_power_on_state()
        {
            var serviceData = await GetServiceData();
            var client = new SonoffClient(_httpClient);
            var beforeDeviceInfo = await client.GetDeviceInfoAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);

            Exception exception = null;
            DeviceInfoData deviceInfo = null;
            try
            {
                await client.SetPowerOnStateAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId, PowerOnState.off);

                deviceInfo = await client.GetDeviceInfoAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);
                await client.SetPowerOnStateAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId, PowerOnState.on);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
            Assert.NotNull(deviceInfo);
            Assert.Equal(PowerOnState.on, deviceInfo.Startup);

            await client.SetPowerOnStateAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId, beforeDeviceInfo.Startup);
        }

        [Fact(Skip = "A real device must be in DIY mode")]
        public async Task should_get_wifi_signal_strength()
        {
            var serviceData = await GetServiceData();
            var client = new SonoffClient(_httpClient);

            Exception exception = null;
            WifiSignalStrengthData data = null;
            try
            {
                data = await client.GetWifiSignalStrengthAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
            Assert.NotNull(data);
        }

        [Fact(Skip = "A real device must be in DIY mode")]
        public async Task should_set_inching()
        {
            var serviceData = await GetServiceData();
            var client = new SonoffClient(_httpClient);

            Exception exception = null;

            try
            {
                var deviceInfo = await client.GetDeviceInfoAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);
                await client.SetInchingAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId, State.on, 1000);
                await Task.Delay(5000); // time to observe inching
                await client.SetInchingAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId, deviceInfo.Pulse, deviceInfo.PulseWidth);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
        }

        [Theory(Skip = "A real device must be in DIY mode. It changes WiFi settings so use it carefully")]
        [InlineData("ssid", "password")]
        public async Task should_set_wifi_settings(string ssid, string password)
        {
            var serviceData = await GetServiceData();
            var client = new SonoffClient(_httpClient);

            Exception exception = null;

            try
            {
                await client.SetWiFiSettingsAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId, ssid, password);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
        }

        [Fact(Skip = "A real device must be in DIY mode")]
        public async Task should_unlock_ota()
        {
            var serviceData = await GetServiceData();
            var client = new SonoffClient(_httpClient);

            Exception exception = null;
            DeviceInfoData beforeDeviceInfo = null;
            DeviceInfoData afterDeviceInfo = null;
            try
            {
                beforeDeviceInfo = await client.GetDeviceInfoAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);

                if (beforeDeviceInfo.OtaUnlock)
                {
                    output.WriteLine("OTA already unlocked. Test will show nothing.");
                    return;
                }

                await client.UnlockOTAAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);

                afterDeviceInfo = await client.GetDeviceInfoAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
            Assert.NotNull(beforeDeviceInfo);
            Assert.NotNull(afterDeviceInfo);
            Assert.NotEqual(beforeDeviceInfo.OtaUnlock, afterDeviceInfo.OtaUnlock);
        }

        [Theory(Skip = "A real device must be in DIY mode. Be sure you want to flash the device")]
        [InlineData("downloadUrl", "sha256sum")]
        public async Task should_flash_ota(string downloadUrl, string sha256sum)
        {
            var serviceData = await GetServiceData();
            var client = new SonoffClient(_httpClient);

            Exception exception = null;
            try
            {
                await client.OTAFlashAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId, downloadUrl, sha256sum);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
        }

        [Fact(Skip = "A real device must be in DIY mode")]
        public async Task should_return_device_info()
        {
            var serviceData = await GetServiceData();
            var client = new SonoffClient(_httpClient);

            DeviceInfoData deviceInfo = null;
            Exception exception = null;
            try
            {
                deviceInfo = await client.GetDeviceInfoAsync(serviceData.ipAddress, serviceData.port, serviceData.deviceId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
            Assert.NotNull(deviceInfo);
        }

        private static async Task<(string ipAddress, int port, string deviceId)> GetServiceData()
        {
            var serviceName = "_ewelink._tcp.local.";
            var hosts = await ZeroconfResolver.ResolveAsync(serviceName);
            var host = hosts.First();
            var service = host.Services[serviceName];
            var ipAddress = host.IPAddress;
            var port = service.Port;
            var deviceId = service.Properties.First()["id"];

            return (ipAddress, port, deviceId);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
