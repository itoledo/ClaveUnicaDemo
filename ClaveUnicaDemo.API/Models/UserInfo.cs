using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClaveUnicaDemo.API.Models
{
    public class UserInfo
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }
        [JsonProperty("RolUnico")]
        public RolUnico RolUnico { get; set; }
        [JsonProperty("name")]
        public Name Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public class RolUnico
    {
        [JsonProperty("numero")]
        public int Numero { get; set; }
        [JsonProperty("DV")]
        public string DV { get; set; }
        [JsonProperty("tipo")]
        public string Tipo { get; set; }
    }

    public class Name
    {
        [JsonProperty("nombres")]
        public string[] Nombres { get; set; }
        [JsonProperty("apellidos")]
        public string[] Apellidos { get; set; }
    }
}
