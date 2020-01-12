using System;

namespace SonoffApi.Client.Data
{
    internal class DeviceRequest<T> where T : class, new()
    {
        public string DeviceId { get; set; }
        public T Data { get; set; }
    }
}
