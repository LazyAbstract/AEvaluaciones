using Althus.Evaluaciones.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Althus.Evaluaciones.Web.Controllers
{
    public class BaseController : Controller
    {
        public ALTHUSEvaluacionesDataContext db = new ALTHUSEvaluacionesDataContext()
            .WithConnectionStringFromConfiguration();

        public string Mensaje
        {
            set
            {
                TempData["Mensaje"] = value;
            }
        }

        protected Usuario UsuarioActual
        {
            get
            {
                return db.Usuarios.SingleOrDefault(x => x.NombreUsuario == User.Identity.Name);
            }
        }

    }
}