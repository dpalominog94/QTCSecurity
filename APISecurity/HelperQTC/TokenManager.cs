using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace HelperQTC
{
    public class TokenManager
    {
        public IConfiguration Configuration { get; set; }
        String key;
        DateTime fecha;
        SymmetricSecurityKey securityKey;
        SigningCredentials credentials;
        JwtHeader header;
        JwtSecurityTokenHandler handler; 
        public bool Error { get; set; } = false;
        public TokenManager(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration.GetValue<string>("AppSettings:Secret").ToString();
            //ConfigurationManager.AppSettings["PrivateKeyJWT"].ToString();
            securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            header = new JwtHeader(credentials);
            handler = new JwtSecurityTokenHandler();
            fecha = DateTime.Now;//.Date.ToString("yyyy-MM-dd");
        }
        #region "ETHICAL"
        public bool ValidarToken(String Token, String IV)
        {
            try
            {
                var token = handler.ReadJwtToken(Token);
                var lstClaims = new List<String>();
                foreach (var claim in token.Claims)
                {
                    lstClaims.Add(claim.Value);
                }
                //if (lstClaims.Count == 7 && token.Header.Alg == "HS256" && token.Header.Typ == "JWT")//29032021
                if (lstClaims.Count == 6 && token.Header.Alg == "HS256" && token.Header.Typ == "JWT")
                {
                    var signature = HmacSha256Digest(token.RawHeader + "." + token.RawPayload, key);
                    signature = HexString2B64String(signature).Replace("+", "-").Replace("/", "_").Replace("=", String.Empty);
                    var Hast = Encriptacion.Desencriptar(lstClaims[0], IV);
                    var FechaDescr = Encriptacion.Desencriptar(lstClaims[1], IV);
                    var API = Encriptacion.Desencriptar(lstClaims[2], IV);
                    string[] arr = Hast.Split(new string[] { "," }, StringSplitOptions.None);
                    //if (signature == token.RawSignature && lstClaims[2] == fecha) 29032021
                    if (signature == token.RawSignature && Convert.ToDateTime(FechaDescr) >= fecha && arr.Length == 4)
                    {
                        var CodigoUsuario = Encriptacion.Desencriptar(arr[0].ToString(), IV);//Encriptacion.Desencriptar(lstClaims[0], Configuration);
                        var Password = Encriptacion.Desencriptar(arr[1].ToString(), IV);//Encriptacion.Desencriptar(lstClaims[1], Configuration);
                        //var APICredencial = context.Apicredencial.FirstOrDefault(x =>
                        //x.CodigoUsuario == CodigoUsuario && x.PasswordUsuario == Password
                        //&& x.Estado == ConstantHelpers.ESTADO.ACTIVO
                        //&& x.Empresa.Estado == ConstantHelpers.ESTADO.ACTIVO);
                        //if (APICredencial != null && API == APICredencial.ApicredencialId.ToString())
                        //{
                        //    var Empresa = context.Empresa.FirstOrDefault(x => x.EmpresaId == APICredencial.EmpresaId);
                        //    CodigoEmpresa = Empresa.CodigoEmpresa;
                        //    APICredencialId = APICredencial.ApicredencialId;

                        //    EmpresaId = APICredencial.EmpresaId;
                        //    GrupoEmpresaId = Empresa.GrupoEmpresaId;
                             return true;
                        //}
                    }
                    else if (signature == token.RawSignature && Convert.ToDateTime(FechaDescr) != fecha)
                    {
                        Error = true;
                    }
                }
                return false;
            }
            catch (Exception)// ex)
            {
                return false;
            }
        }
        public string HmacSha256Digest(string message, string secret)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);

            byte[] bytes = cryptographer.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        public string HexString2B64String(string input)
        {
            return Convert.ToBase64String(HexStringToHex(input));
        }
        public byte[] HexStringToHex(string inputHex)
        {
            var resultantArray = new byte[inputHex.Length / 2];
            for (var i = 0; i < resultantArray.Length; i++)
            {
                resultantArray[i] = Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }
        /*
        public TokenEmail SendEmailToken(string plantilla, string destinatario, Int32 usuarioId)
        {
            TokenEmail TokenEmail = new TokenEmail();
            try
            {
                using (this.client = new SmtpClient(ConfigurationManager.AppSettings["clientHostSTMP"].ToString()))
                {
                    var emailToken = context.Email.FirstOrDefault(x => x.Estado == ConstantHelpers.ESTADO.ACTIVO && x.EmpresaId == null && x.Tipo == ConstantHelpers.TIPO_EMAIL.TOKEN);
                    var usuarioToken = context.UsuarioToken.FirstOrDefault(x => x.UsuarioId == usuarioId && x.Estado.Equals(ConstantHelpers.ESTADO.POR_INGRESAR));

                    TemplateRenderHelper templateRender = new TemplateRenderHelper(Parent);

                    this.from = emailToken.UsuarioRemitente;
                    this.Password = emailToken.PasswordRemitente;

                    this.client.UseDefaultCredentials = false;
                    this.client.Credentials = new NetworkCredential(from, Password);
                    this.client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["clientPortSTMP"]);
                    this.client.EnableSsl = true;
                    this.client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    this.client.Timeout = 10000;
                    this.destinatarios = destinatario.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    var mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(this.from, emailToken.NombreRemitente);

                    this.destinatarios.ToList().ForEach(x => mailMessage.To.Add(x));

                    mailMessage.Subject = emailToken.Asunto;

                    mailMessage.IsBodyHtml = true;

                    TokenEmail.TituloEmail = emailToken.Titulo;

                    if (usuarioToken != null)
                    {
                        TokenEmail.Token = usuarioToken.Token;
                    }

                    mailMessage.Body += templateRender.Render(plantilla, TokenEmail);

                    this.client.Send(mailMessage);
                }

                return TokenEmail;
            }
            catch (Exception ex)
            {
                TokenEmail.Mensaje = ex.Message + "-" + (ex.InnerException != null ? ex.InnerException.Message : String.Empty);
                return TokenEmail;
            }
        }*/
        #endregion
    }
}
