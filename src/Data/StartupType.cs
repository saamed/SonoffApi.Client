using System.ComponentModel;

namespace SonoffApi.Client.Data
{
    public enum PowerOnState
    {
        [Description("on")]
        On,
        [Description("off")]
        Off,
        [Description("stay")]
        Stay
    }
}
