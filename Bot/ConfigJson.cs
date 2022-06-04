using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordBot
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string CommandPrefix { get; private set; }

        [JsonProperty("mamonPhotoURL")]
        public string mamonPhotoURL { get; private set; }

        [JsonProperty("IDPawla")]
        public ulong IDPawla { get; private set; }

        [JsonProperty("MemeFolderRoot")]
        public string MemeFolderRoot { get; private set; }

    }
}
