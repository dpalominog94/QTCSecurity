using EntityQTC;
using HelperQTC;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using WebAPI.Service;

namespace WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/")]
    [ApiController] 
    public class LoginController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        private IUserService _userService;
        public LoginController(IUserService userService, IConfiguration configuration)
        {
            Configuration = configuration;
            _userService = userService; 
        } 

        #region "POS"
        [HttpPost]
        [Route("v{version:apiVersion}/Authenticar")]
        public IActionResult Authenticar([FromBody] AuthenticarRequest auth)
        {
            var responseError = new BaseResponse(); bool valError = false;
            var response = new AuthenticarResponse();
            var EstadoDobleSeguridad = Configuration.GetValue<string>("AppSettings:FlagSecurity");
            var Llave = Configuration.GetValue<string>("ConnectionStrings:DataBaseLlave");
            var IV = Configuration.GetValue<string>("AppSettings:IV"); 
            response.Security_type = EstadoDobleSeguridad == "false" ? "Token" : "Token y Codigo"; 
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var request = JsonConvert.SerializeObject(auth, Formatting.None).Replace(" ", "").Replace("\"","'");
                    var Encry = Encriptacion.Encriptar(request, IV);
                    var _Res = _userService.ServicioQTC() + "api/Login/Authenticar" + "?Encry=" + Encry;

                    string stringResponse = wc.DownloadString(_Res);
                    response = JsonConvert.DeserializeObject<AuthenticarResponse>(stringResponse);
                    if (response.codigoRespuesta != "00")
                    {
                        valError = true; responseError.codigoRespuesta = response.codigoRespuesta; responseError.mensaje = response.mensaje;
                    }
                } 
            }
            catch (WebException ex)
            {
                valError = true;
                responseError.codigoRespuesta = "98";
                responseError.mensaje = ex.Message;
            }
            catch (Exception ex)
            {
                valError = true;
                responseError.codigoRespuesta = "99";
                responseError.mensaje = ex.Message +"|"+ ex.StackTrace;
            }

            if (valError)
            {
                return Ok(responseError);
            }
            else
            {
                response.mensaje = "Ok!";
                return Ok(response);
            } 
        }  
        [HttpPost]
        [Route("v{version:apiVersion}/AuthenticarValidacion")]
        [Authorize]
        public IActionResult AutenticarValidacion()
        {
            var response = new BaseResponse();
            TokenManager Token = new TokenManager(Configuration);
            var token = HttpContext.GetTokenAsync("access_token");
            var LlavePublica = Configuration.GetValue<string>("AppSettings:IV");
            if (Token.ValidarToken(token.Result, LlavePublica))
            { 
                response.codigoRespuesta = "00";
                response.mensaje = "Token Vigente"; 
            }
            else
            { 
                response.codigoRespuesta = "99";
                response.mensaje = "Token Invalido"; 
            } 
            return Ok(response);
        }

        [HttpGet]
        [Route("v{version:apiVersion}/OlvideClave")]
        [Authorize]
        public IActionResult OlvideClave(String CodigoVerificacion,String Usuario,String NuevaClave)
        {

            return Ok();
        }

        [HttpPost]
        [Route("v{version:apiVersion}/Escucha")]
        public IActionResult Escucha(string Usuario)
        {
            String response = string.Empty; 
            var BackUrl = _userService.ServicioQTC() + "api/Login/Escucha";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    client.Headers.Add("Accept", "text/json"); 
                    var request = new AuthenticarRequest();
                    request.Usuario = Usuario;request.Clave = "123";
                    var Request = JsonConvert.SerializeObject(request, Formatting.None);
                    var stringResponse = client.UploadString(BackUrl, "POST", Request); 
                    response = stringResponse;  
                    return Ok(response);
                }
            }
            catch (WebException ex)
            { 
                response = ex.Message;
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            return Ok(response); 
        }

        [HttpGet]
        [Route("v{version:apiVersion}/Redirect")]
        [Authorize]
        public IActionResult Redirect()
        {

            return Ok();
        }
        #endregion

    }
}
