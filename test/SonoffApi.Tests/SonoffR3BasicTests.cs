using SonoffApi.Client;
using Xunit;
using Zeroconf;
using System.Linq;
using System.Threading.Tasks;
using SonoffApi.Client.Data;

namespace SonoffApi.Tests
{
    public class SonoffR3BasicTests
    {
        [Fact(Skip = "A real device must be in DIY mode")]
        public async Task should_return_device_info()
        {
            var serviceData = GetServiceData();
            var client = new SonoffClient($"{serviceData.ipAddress}:{serviceData.port}");

            DeviceInfoData deviceInfo = null;
            System.Exception exception = null;
            try
            {
                deviceInfo = await client.GetDeviceInfoAsync(serviceData.deviceId);
            }
            catch (System.Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
            Assert.NotNull(deviceInfo);
        }

        [Fact]
        public async Task should_turn_switch_on()
        {
            var serviceData = GetServiceData();
            var client = new SonoffClient($"{serviceData.ipAddress}:{serviceData.port}");
            
            System.Exception exception = null;
            DeviceInfoData deviceInfo = null;
            try
            {
                await client.TurnSwitchOnAsync(serviceData.deviceId);
                
                deviceInfo = await client.GetDeviceInfoAsync(serviceData.deviceId);
            }
            catch (System.Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
            Assert.NotNull(deviceInfo);
            Assert.Equal(State.on, deviceInfo.Switch);
        }

        [Fact]
        public async Task should_turn_switch_off()
        {
            var serviceData = GetServiceData();
            var client = new SonoffClient($"{serviceData.ipAddress}:{serviceData.port}");

            System.Exception exception = null;
            DeviceInfoData deviceInfo = null;
            try
            {
                await client.TurnSwitchOffAsync(serviceData.deviceId);

                deviceInfo = await client.GetDeviceInfoAsync(serviceData.deviceId);
            }
            catch (System.Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
            Assert.NotNull(deviceInfo);
            Assert.Equal(State.off, deviceInfo.Switch);
        }

        private static (string ipAddress, int port, string deviceId) GetServiceData()
        {
            var serviceName = "_ewelink._tcp.local.";
            var host = ZeroconfResolver.ResolveAsync(serviceName).Result[0];
            var service = host.Services[serviceName];
            var ipAddress = host.IPAddress;
            var port = service.Port;
            var deviceId = service.Properties.First()["id"];

            return (ipAddress, port, deviceId);
        }
    }
}
