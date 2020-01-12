using System.ComponentModel;

namespace SonoffApi.Client
{
    public enum SonoffMethods
    {
        [Description("http://{0}/zeroconf/info")]
        GetDeviceInfo
    }
}