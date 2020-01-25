namespace SonoffApi.Client.Data
{
    public class DeviceInfoData
    {
        public State Switch { get; set; }
        public PowerOnState Startup { get; set; }
        public State Pulse { get; set; }
        public long PulseWidth { get; set; }
        public string Ssid { get; set; }
        public bool OtaUnlock { get; set; }
    }
}