using EntityQTC; 
using HelperQTC;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;

namespace WebAPI.Service
{

    public interface IUserService
    {
        String ServicioQTC();
        AuthenticarResponse Authenticate(AuthenticarRequest Auth); 
    }

    public class _UserService : IUserService
    {
        public IConfiguration Configuration { get; set; }
        public String cx { get; set; }
        public String consultaAPI { get; set; }
        private List<AuthenticarResponse> _users = new List<AuthenticarResponse>();

        public _UserService(IConfiguration configuration)//, IOptions<AppSettings> appSettings)
        {
            Configuration = configuration;
            var CadenaSinLlave = Configuration.GetValue<string>("ConnectionStrings:DataBase");
            var CadenaLlave = Configuration.GetValue<string>("ConnectionStrings:DataBaseLlave");
            cx = Encriptacion.Desencriptar(CadenaSinLlave, CadenaLlave); 
            consultaAPI = Configuration.GetValue<string>("AppSettings:ConsultaAPI");
        }
        public AuthenticarResponse Authenticate(AuthenticarRequest Auth)
        {
            var AutenticarEmpresa = new AuthenticarResponse();  
            using (NpgsqlConnection cn = new NpgsqlConnection(this.cx)) 
            {
                cn.Open();
                using (var cmd = new NpgsqlCommand(consultaAPI, cn)) {
                      NpgsqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read()) 
                    {
                        var myString = dr[2]; //The 0 stands for "the 0'th column", so the first column of the result.
                                              // Do somthing with this rows string, for example to put them in to a list
                        var UrlBackEnd = dr[2];
                        foreach (var item in dr)
                        {

                        }
                    } 
                    dr.Close();
                }
                cn.Close(); 
            }   
            return AutenticarEmpresa;
        }
        public String ServicioQTC()
        {
            String Response=string.Empty; 
            using (NpgsqlConnection cn = new NpgsqlConnection(this.cx))
            {
                cn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(consultaAPI, cn))
                {
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Response = dr[2].ToString();
                    }
                    dr.Close();
                }
                cn.Close();
            }
            return Response;
        }
        //public IEnumerable<AuthenticarResponse> GetAll()
        //{
        //    return _users;
        //}

    }

    //    /*****************************/
    //    public Int32 EmpresaId { get; set; }
    //    public String CodigoEmpresa { get; set; } = "00";
    //    public Int32 APICredencialId { get; set; } = 0;
    //    String fecha;
    //    private CoreDBContext context = null;
    //    /*****************************/
    //    public IConfiguration Configuration { get; set; }
    //    // users hardcoded for simplicity, store in a db with hashed passwords in production applications
    //    private List<AuthEmpresa> _users = new List<AuthEmpresa>();
    //    private readonly AppSettings _appSettings;
    //public ServiceUser(IConfiguration configuration, IOptions<AppSettings> appSettings)
    //{
    //    Configuration = configuration;
    //    _appSettings = appSettings.Value;
    //    fecha = DateTime.Now.Date.ToString("yyyy-MM-dd");
    //    var Connect = Configuration.GetConnectionString("STKDataBase");
    //    context = new CoreDBContext(Connect);
    //}

    //public AuthEmpresa Authenticate(AutenticarEmpresaBE Auth)
    //{
    //    AuthEmpresa AutenticarEmpresa = new AuthEmpresa();
    //    //TODO: AutenticarEmpresa.mensaje = "TRANSACCIÓN CORRECTA";
    //    AutenticarEmpresa.codigoRespuesta = "00";
    //    try
    //    {
    //        Auth.usuario = Encriptacion.Encriptar(Auth.usuario.ToString(), Configuration);
    //        Auth.clave = Encriptacion.Encriptar(Auth.clave.ToString(), Configuration);
    //        #region "GENERAR TOKEN"

    //        //TODO: 01032021 var APICredencial = context.Apicredencial.FirstOrDefault(x => x.CodigoUsuario == Auth.usuario && x.PasswordUsuario == Auth.clave);  
    //        var APICredencial = context.Apicredencial.FirstOrDefault(x => x.CodigoUsuario == Auth.usuario
    //        && x.PasswordUsuario == Auth.clave);

    //        var tokenString = String.Empty;
    //        if (APICredencial != null)
    //        {
    //            if (APICredencial.Estado == "DES")
    //            {
    //                AutenticarEmpresa.codigoRespuesta = "02";
    //                return AutenticarEmpresa;
    //            }
    //            if (APICredencial.Estado == "DEL")
    //            {
    //                AutenticarEmpresa.codigoRespuesta = "31";
    //                return AutenticarEmpresa;
    //            }

    //            var Empresa = context.Empresa.FirstOrDefault(x => x.EmpresaId == APICredencial.EmpresaId);
    //            CodigoEmpresa = Empresa.CodigoEmpresa;
    //            EmpresaId = APICredencial.EmpresaId;
    //            // authentication successful so generate jwt token 
    //            var tokenHandler = new JwtSecurityTokenHandler();
    //            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

    //            var _fechafin = fecha + " 23:59:59";
    //            var fechafin = DateTime.Parse(_fechafin);
    //            var tokenDescriptor = new SecurityTokenDescriptor();
    //            ///////
    //            var _n = System.Guid.NewGuid().ToString("N").Substring(1, 6);
    //            string Hast = Encriptacion.Encriptar(APICredencial.CodigoUsuario.ToString(), Configuration) + "," +
    //                          Encriptacion.Encriptar(APICredencial.PasswordUsuario.ToString(), Configuration) + "," +
    //                          Encriptacion.Encriptar(_n, Configuration);
    //            tokenDescriptor.Subject = new ClaimsIdentity(new Claim[]
    //                {
    //                new Claim("Hast", Encriptacion.Encriptar(Hast,Configuration)),
    //                new Claim("Fecha", Encriptacion.Encriptar(fecha,Configuration)),
    //                new Claim("API", Encriptacion.Encriptar(APICredencial.ApicredencialId.ToString(),Configuration)),
    //                //TODO: 24032021
    //                //new Claim("CodigoUsuario", Encriptacion.Encriptar(APICredencial.CodigoUsuario.ToString(),Configuration)),
    //                //new Claim("Password", Encriptacion.Encriptar(APICredencial.PasswordUsuario.ToString(),Configuration)),
    //                //new Claim("Fecha",fecha ),
    //                //new Claim("APICredencialId", APICredencial.ApicredencialId.ToString())
    //                });
    //            tokenDescriptor.Expires = fechafin.AddDays(10);// DateTime.Now.AddDays(1),
    //            tokenDescriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
    //            ///////
    //            var timeSpan = (fechafin - DateTime.Now);
    //            var timeMinuts = (timeSpan.Hours * 60) + timeSpan.Minutes;
    //            AutenticarEmpresa.expira_en = "" + timeMinuts;

    //            var _token = tokenHandler.CreateToken(tokenDescriptor);
    //            var stringtoken = tokenHandler.WriteToken(_token);
    //            AutenticarEmpresa.token = stringtoken;
    //            AutenticarEmpresa.refresh_token = stringtoken;
    //        }
    //        else
    //        {
    //            APICredencial = context.Apicredencial.FirstOrDefault(x => x.CodigoUsuario == Auth.usuario
    //            && x.Estado == ConstantHelpers.ESTADO.ACTIVO);

    //            if (APICredencial != null)
    //            {
    //                AutenticarEmpresa.codigoRespuesta = "01";
    //                if (APICredencial.Intentos == 3)
    //                {
    //                    AutenticarEmpresa.codigoRespuesta = "02";
    //                    APICredencial.Estado = ConstantHelpers.ESTADO.DESACTIVADO;
    //                    APICredencial.Salt = KeyGenerator.GetUniqueKey(99);
    //                    var url = Configuration.GetValue<string>("Url:STKWeb") +
    //                    "/Intranet/Control/UnlockCredential?Salt=" + APICredencial.Salt;
    //                    var empresa = context.Empresa.FirstOrDefault(x => x.Estado == ConstantHelpers.ESTADO.ACTIVO && x.EmpresaId == APICredencial.EmpresaId);
    //                    if (empresa != null)
    //                    {
    //                        Email email = null;
    //                        email = context.Email.FirstOrDefault(x => x.Estado == ConstantHelpers.ESTADO.ACTIVO && x.GrupoEmpresaId == empresa.GrupoEmpresaId
    //                        && x.Tipo == "ALE");
    //                        if (email == null)
    //                        {
    //                            email = context.Email.FirstOrDefault(x => x.Estado == ConstantHelpers.ESTADO.ACTIVO && x.EmpresaId == APICredencial.EmpresaId
    //                            && x.Tipo == "ALE");

    //                        }
    //                        EmailLogic emailLogic = new EmailLogic(Configuration);
    //                        emailLogic.SendMailBloqueoCredencial(email, APICredencial.Salt, url);
    //                    }


    //                }
    //                else
    //                {
    //                    if (APICredencial.Intentos == null)
    //                        APICredencial.Intentos = 0;

    //                    APICredencial.Intentos += 1;
    //                }
    //                context.SaveChanges();
    //            }
    //            else
    //            {
    //                AutenticarEmpresa.codigoRespuesta = "01";
    //            }

    //        }

    //        #endregion

    //        context.SaveChanges();
    //    }
    //    catch (Exception)
    //    {
    //        AutenticarEmpresa.codigoRespuesta = "99";
    //    }


    //    return AutenticarEmpresa;
    //}

    //public IEnumerable<AuthEmpresa> GetAll()
    //{
    //    return _users;
    //}

}
