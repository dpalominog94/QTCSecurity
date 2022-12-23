using EntityQTC;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace EntityQTC
{
    public class AuthenticarResponse : BaseResponse
    {

        [JsonProperty(Order = 100)]
        [DataMember]
        public String Access_token { get; set; } = String.Empty;

        [JsonProperty(Order = 110)]
        [DataMember]
        public String Token_type { get; set; } = "Bearer";
        [JsonProperty(Order = 120)]
        [DataMember]
        public String Expires_in { get; set; } = String.Empty;
        [JsonProperty(Order = 130)]
        [DataMember]
        public String Security_type { get; set; } = String.Empty;
        [JsonProperty(Order = 140)]
        [DataMember]
        public String Nota { get; set; } = "Swagger Activo";
    }
}
