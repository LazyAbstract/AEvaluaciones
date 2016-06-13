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

        [HttpPost]
        public ActionResult CrearEditarUsuario(CrearEditarUsuarioFormModel Form)
        {
            if (ModelState.IsValid)
            {
                if (Form.IdUsuario.HasValue)
                {
                    //Usuario user = db.Usuarios.Single(x => x.Id == Form.IdUsuario.Value);
                    //user.PersonaNoColaborador.Nombre = Form.Nombre;
                    //user.PersonaNoColaborador.ApellidoPaterno = Form.ApellidoPaterno;
                    //user.PersonaNoColaborador.ApellidoMaterno = Form.ApellidoMaterno;
                    //user.PersonaNoColaborador.Correo = Form.Mail;

                    //IEnumerable<UsuarioTipoUsuario> TiposUsuario = db.UsuarioTipoUsuarios.Where(x => x.IdUsuario == Form.IdUsuario);
                    //db.UsuarioTipoUsuarios.DeleteAllOnSubmit(TiposUsuario);
                    //db.SubmitChanges();

                    //foreach (var item in Form.IdTipoUsuario)
                    //{
                    //    UsuarioTipoUsuario utu = new UsuarioTipoUsuario()
                    //    {
                    //        IdUsuarioTipoUsuario = Guid.NewGuid(),
                    //        idTipoUsuario = item,
                    //        IdUsuario = Form.IdUsuario.Value,
                    //    };
                    //    db.UsuarioTipoUsuarios.InsertOnSubmit(utu);
                    //}
                    //db.SubmitChanges();

                    //string[] roles = Roles.GetAllRoles();
                    //foreach (string rol in roles)
                    //{
                    //    if (Roles.IsUserInRole(user.Nombre, rol))
                    //    {
                    //        Roles.RemoveUserFromRole(user.Nombre, rol);
                    //    }
                    //}

                    //List<int> TipoUsuarios = db.UsuarioTipoUsuarios.Where(x => x.IdUsuario == user.Id).Select(x => x.idTipoUsuario).ToList();
                    //string[] Permisos = db.TipoUsuarioPermisos.Where(x => TipoUsuarios.Contains(x.IdTipoUsuario.Value)).Select(x => x.Permiso.Valor).ToArray();

                    //foreach (string permiso in Permisos)
                    //{
                    //    if (!Roles.IsUserInRole(user.Nombre, permiso))
                    //    {
                    //        Roles.AddUserToRole(user.Nombre, permiso);
                    //    }
                    //}
                    Mensaje = "El usuario fue editado exitosamente";
                    return RedirectToAction("ListarUsuario");
                }
            }
            CrearEditarUsuarioViewModel Model = new CrearEditarUsuarioViewModel(Form);
            Model.TiposUsuario = db.TipoUsuarios.OrderBy(x => x.TipoUsuario1);
            return View(Model);
        }
    }
}