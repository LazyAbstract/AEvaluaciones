using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace Althus.Evaluaciones.Web.Configuration
{
    public class InitSimpleMembership : IConfigurable
    {
        public void Configure()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("AppDb", "Usuario", "IdUsuario", "NombreUsuario", autoCreateTables: true);
            }
        }
    }
}