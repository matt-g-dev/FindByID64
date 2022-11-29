using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FindByID64
{
    interface ISysID64
    {
        public ulong? ID64 { get; set; }
    }

    internal class SysID64 : ISysID64
    {
        [JsonPropertyName("id64")]
        public ulong? ID64 { get; set; }
    }

    internal class SysID64SystemAddress : ISysID64
    {
        [JsonPropertyName("SystemAddress")]
        public ulong? ID64 { get; set; }
    }

    internal class SysID64SystemID64 : ISysID64
    {
        [JsonPropertyName("systemId64")]
        public ulong? ID64 { get; set; }
    }

    public enum ID64Key { id64, SystemAddress, systemId64 }

}
