namespace SonoffApi.Client.Data
{
    internal class InchingData
    {
        public State Pulse { get; set; }
        /// <summary>Must be multiply of 500 in range of 500-36000000. Milliseconds</summary>

        public long PulseWidth { get; set; }
    }
}
