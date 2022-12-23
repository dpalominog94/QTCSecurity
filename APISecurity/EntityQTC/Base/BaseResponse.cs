using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace EntityQTC
{
    [DataContract]
    public class BaseResponse
    {
        [JsonProperty(Order = 1)]
        [DataMember]
        public String codigoRespuesta { get; set; } = "99";
        [JsonProperty(Order = 2)]
        [DataMember]
        public String mensaje { get; set; } = "ERROR DE SISTEMA";  
    }
}
