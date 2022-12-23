using System;
using System.ComponentModel.DataAnnotations;

namespace EntityQTC
{
    public class AuthenticarRequest
    {
        [Required]
        public string Usuario { get; set; } = string.Empty;
        [Required]
        public string Clave { get; set; } = string.Empty;
        public string Tipo { get; set; }
        public string EmpresaId { get; set; } //TODO: Puede Ir NULL

        public String MaskCredencial()
        {
            var mask = String.Empty;
            mask = String.Format("Usuario: {0} Clave: {1}", Usuario.Substring(0, 3).PadRight(10, '#'), Clave.Substring(0, 3).PadRight(10, '#'));
            return mask;
        }

    }
    public class AuthenticarEncryRequest
    {
        [Required]
        public string Encry { get; set; } = string.Empty; 
    }
}
