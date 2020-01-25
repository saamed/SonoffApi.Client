namespace SonoffApi.Client.Data
{
    internal class DeviceResponse<T> where T : class
    {
        public int Seq { get; set; }
        public int Error { get; set; }
        public T Data { get; set; }
    }
}
