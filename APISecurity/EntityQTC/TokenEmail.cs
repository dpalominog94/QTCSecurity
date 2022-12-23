using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQTC
{
    public class TokenEmail
    {
        public String Token { get; set; }
        public String MD5 { get; set; }
        public String Mensaje { get; set; }
        public String TituloEmail { get; set; }
        public TokenEmail()
        {
            var seed = Environment.TickCount;
            var random = new Random(seed);
            var value = random.Next(0, 5);
            Token = value.ToString(); 
            //TODO:26072021
            string letra = Guid.NewGuid().ToString();
            Token = Token.Replace("A==", letra.Substring(0, 1) + "==");


        }
    }
}
