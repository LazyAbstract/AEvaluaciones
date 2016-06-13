using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.Models.UsuarioModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using AutoMapper;

namespace Althus.Evaluaciones.Web.Controllers
{
    public class UsuarioController : BaseController
    {
        public ActionResult ListarUsuario(int? pagina, string filtro)
        {
            ListarUsuarioViewModel Model = new ListarUsuarioViewModel();
            Model.filtro = filtro;
            IEnumerable<Usuario> Users = db.Usuarios.OrderBy(x => x.ApellidoPaterno);
            if (!String.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLower();
                Users = db.Usuarios
                    .Where(x => x.Nombre.ToLower().Contains(filtro)
                        || x.Nombre.ToLower().Contains(filtro)
                        || x.ApellidoPaterno.ToLower().Contains(filtro)
                        || x.ApellidoMaterno.ToLower().Contains(filtro)
                        || x.Correo.ToLower().Contains(filtro));
            }
            Model.Usuarios = Users.ToPagedList(pagina ?? 1, 10);
            return View(Model);
        }

        public ActionResult CrearEditarUsuario(int? IdUsuario)
        {
            CrearEditarUsuarioViewModel Model = new CrearEditarUsuarioViewModel();
            Model.TiposUsuario = db.TipoUsuarios.OrderBy(x => x.TipoUsuario1);
            if (IdUsuario.HasValue)
            {
                Model.Form.CreacionUsuario = false;
                Usuario user = db.Usuarios.Single(x => x.IdUsuario == IdUsuario.Value);
                Mapper.Map<Usuario, CrearEditarUsuarioFormModel>(user, Model.Form);
                Model.Form.IdTipoUsuario = user.IdTipoUsuario;
            }
            return View(Model);
        }
    }
}