using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.Models.UsuarioModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using AutoMapper;
using Althus.Evaluaciones.Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Althus.Evaluaciones.Web.Controllers
{
    public class UsuarioController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UsuarioController()
        {
        }

        public UsuarioController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

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
                        || x.NombreUsuario.ToLower().Contains(filtro));
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
                Model.Form.IdTipoUsuario = user.IdTipoUsuario.GetValueOrDefault(1);
            }
            return View(Model);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearEditarUsuario(CrearEditarUsuarioFormModel Form)
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
                else
                {
                    string Password = Form.Nombre.Substring(0, 1).ToUpper() + Form.ApellidoPaterno.ToLower() 
                        + "_" + Form.Rut.Numero.ToString().Substring(0, 6);
                    var user = new ApplicationUser { UserName = Form.Correo, Email = Form.Correo };
                    var result = await UserManager.CreateAsync(user, Password);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        Usuario _user = new Usuario()
                        {
                            IdTipoUsuario = Form.IdTipoUsuario,
                            Rut = Form.Rut.Numero,
                            NombreUsuario = Form.Correo,
                            Nombre = Form.Nombre,
                            ApellidoPaterno = Form.ApellidoPaterno,
                        };

                        db.Usuarios.InsertOnSubmit(_user);
                        db.SubmitChanges();
                    }
                    AddErrors(result);
                    //IEnumerable<TipoUsuarioPermiso> Permisos = db
                    //    .TipoUsuarioPermisos.Where(x => x.IdTipoUsuario == Form.IdTipoUsuario);
                    //db.SubmitChanges();

                    Mensaje = "El Usuario fue creado exitosamente.";
                    return RedirectToAction("ListarUsuario");
                }
            }
            CrearEditarUsuarioViewModel Model = new CrearEditarUsuarioViewModel(Form);
            Model.TiposUsuario = db.TipoUsuarios.OrderBy(x => x.TipoUsuario1);
            return View(Model);
        }
    }
}