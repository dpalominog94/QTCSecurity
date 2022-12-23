using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQTC
{
    public class API
    {
        public string Correo { get; set; }
        public string Log { get; set; }
    }

    public class AppSettings
    { 
        public string Secret { get; set; }
        public bool FlagSecurity { get; set; }
    }

    public class ConnectionStrings
    {
        public string DataBase { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; } 
    }
    public class Root
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public string Deploy { get; set; }
        public URL URL { get; set; }
        public API API { get; set; }
        public AppSettings AppSettings { get; set; }
        public Logging Logging { get; set; } 
    }

    public class URL
    {
        public string Log { get; set; }
    }

}
