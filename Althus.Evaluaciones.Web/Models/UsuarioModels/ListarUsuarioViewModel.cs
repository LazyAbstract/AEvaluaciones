using Althus.Evaluaciones.Core;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Models.UsuarioModels
{
    public class ListarUsuarioViewModel
    {
        public IPagedList<Usuario> Usuarios { get; set; }
        public string filtro { get; set; }
    }
}