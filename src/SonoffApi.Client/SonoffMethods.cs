using System.ComponentModel;

namespace SonoffApi.Client
{
    public enum SonoffMethods
    {
        [Description("http://{0}:{1}/zeroconf/info")]
        DeviceInfo,
        [Description("http://{0}:{1}/zeroconf/switch")]
        SwitchOnOff,
        [Description("http://{0}:{1}/zeroconf/signal_strength")]
        WifiSignalStrength,
        [Description("http://{0}:{1}/zeroconf/ota_flash")]
        OTAFlash,
        [Description("http://{0}:{1}/zeroconf/pulse")]
        Inching,
        [Description("http://{0}:{1}/zeroconf/startup")]
        PowerOnState,
        [Description("http://{0}:{1}/zeroconf/wifi")]
        WifiSettings,
        [Description("http://{0}:{1}/zeroconf/ota_unlock")]
        UnlockOTA
    }
}