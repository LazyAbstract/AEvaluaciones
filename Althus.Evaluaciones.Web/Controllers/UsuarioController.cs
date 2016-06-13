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
    [Authorize(Roles = "Usuario")]
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
                    
                    Usuario _user = db.Usuarios.Single(x => x.IdUsuario == Form.IdUsuario);
                    int AntigupoTipoUsuario = _user.IdTipoUsuario.Value;
                    _user.Nombre = Form.Nombre;
                    _user.ApellidoPaterno = Form.ApellidoPaterno;
                    _user.IdTipoUsuario = Form.IdTipoUsuario;

                    if(AntigupoTipoUsuario != Form.IdTipoUsuario)
                    {
                        var user = UserManager.FindByName(_user.NombreUsuario);
                        var roles = UserManager.GetRoles(user.Id);

                        foreach (var rol in db.TipoUsuarioPermisos.Where(x => x.IdTipoUsuario == AntigupoTipoUsuario).Select(x => x.Permiso.Permiso1))
                        {
                            UserManager.RemoveFromRole(user.Id, rol);
                        }

                        foreach (var permiso in _user.TipoUsuario.TipoUsuarioPermisos.Select(x => x.Permiso.Permiso1))
                        {
                            var roleresult = UserManager.AddToRole(user.Id, permiso);
                        }
                    }

                    db.SubmitChanges();
                    Mensaje = "El usuario fue editado exitosamente";
                    return RedirectToAction("ListarUsuario");
                }
                else
                {
                    string Password = Form.Rut.Numero.ToString().Substring(0, 6);
                    var user = new ApplicationUser { UserName = Form.Correo, Email = Form.Correo };                  
                    var result = await UserManager.CreateAsync(user, Password);
                    if (result.Succeeded)
                    {
                        foreach(var permiso in db.TipoUsuarioPermisos.Where(x => x.IdTipoUsuario == Form.IdTipoUsuario).Select(x => x.Permiso.Permiso1))
                        {
                            var roleresult = UserManager.AddToRole(user.Id, permiso);
                        }
              
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
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
                    //AddErrors(result);
                    Mensaje = "El Usuario fue creado exitosamente.";
                    return RedirectToAction("ListarUsuario");
                }
            }
            CrearEditarUsuarioViewModel Model = new CrearEditarUsuarioViewModel(Form);
            Model.TiposUsuario = db.TipoUsuarios.OrderBy(x => x.TipoUsuario1);
            if (Form.IdUsuario.HasValue) Model.Form.CreacionUsuario = false;
            return View(Model);
        }

        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetContrasena(int IdUsuario)
        {
            Usuario _user = db.Usuarios.Single(x => x.IdUsuario == IdUsuario);
            var user = await UserManager.FindByNameAsync(_user.NombreUsuario);
            string Password = _user.Rut.ToString().Substring(0, 6);
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, Password);
            Mensaje = "La contraseña fue reseteada exitosamente";
            return RedirectToAction("ListarUsuario");
        }
    }
}