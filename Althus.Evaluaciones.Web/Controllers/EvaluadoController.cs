using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.Models.EvaluadoModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Althus.Evaluaciones.Web.Controllers
{
    [Authorize]
    public class EvaluadoController : BaseController
    {
        public ActionResult CrearEvaluado(Rut filtro)
        {
            CrearEvaluadoViewModel model = new CrearEvaluadoViewModel();
            if (ModelState.IsValid && filtro != null && db.Evaluados.Any(x => x.Rut == filtro.Numero))
            {
                Evaluado eval = db.Evaluados.Single(x => x.Rut == filtro.Numero);
                Mapper.Map<Evaluado, CrearEvaluadoFormModel>(eval, model.Form);
                model.Form.IdEvaluado = eval.IdEvaluado;
                Mensaje = "Se encontraron los datos del postulante.";
            }
            else if (filtro != null)
            {
                Mensaje = "No se encontró el postulante con el Rut especificado.";
            }
            model.filtro = filtro;
            return View(model);
        }

        [HttpPost]
        public ActionResult CrearEvaluado(CrearEvaluadoFormModel Form)
        {
            Evaluado evaluado = new Evaluado();
            if(Form.IdEvaluado.HasValue)
            {
                if(!Form.IdEmpresa.HasValue  || !Form.IdCargo.HasValue)
                {
                    Mensaje = "Debe Seleccionar Empre y Cargo para continuar con la evaluación.";
                    return RedirectToAction("CrearEvaluado");
                }
                else
                {
                    evaluado = db.Evaluados.Single(x => x.IdEvaluado == Form.IdEvaluado.Value);
                    string Nombreusuario = User.Identity.Name;
                    Usuario usuario = db.Usuarios.SingleOrDefault(x => x.NombreUsuario == Nombreusuario);
                    int? IdUsuarioEvaluador = null;
                    if (usuario != null) IdUsuarioEvaluador = usuario.IdUsuario;
                    Evaluacion evaluacion = new Evaluacion()
                    {
                        IdTipoEvaluacion = TipoEvaluacion.Evaluacion_Psicologica_CCU,
                        IdCargo = Form.IdCargo.Value,
                        IdEvaluado = evaluado.IdEvaluado,
                        IdUsuarioEvaluador = IdUsuarioEvaluador,
                        FechaEvaluacion = DateTime.Now,
                        IdTipoEstadoEvaluacion = TipoEstadoEvaluacion.Creada,
                    };

                    db.Evaluacions.InsertOnSubmit(evaluacion);
                    db.SubmitChanges();

                    return RedirectToAction("CrearEvaluacion", "Evaluacion", new { IdEvaluacion = evaluacion.IdEvaluacion });
                }
            }
            if(ModelState.IsValid)
            {
                
                if(Form.IdEvaluado.HasValue)
                {
                    evaluado = db.Evaluados.Single(x => x.Rut == Form.Rut.Numero);
                }
                else
                {
                    evaluado = Mapper.Map<CrearEvaluadoFormModel, Evaluado>(Form);
                    evaluado.CreadoPor = User.Identity.Name;
                    db.Evaluados.InsertOnSubmit(evaluado);
                    db.SubmitChanges();
                    Mensaje = "El Postulante fue ingresado exitosamente.";
                }
                
                string Nombreusuario = User.Identity.Name;
                Usuario usuario = db.Usuarios.SingleOrDefault(x => x.NombreUsuario == Nombreusuario);
                int? IdUsuarioEvaluador = null;
                if(usuario != null) IdUsuarioEvaluador = usuario.IdUsuario;
                Evaluacion evaluacion = new Evaluacion()
                {
                    IdTipoEvaluacion = TipoEvaluacion.Evaluacion_Psicologica_CCU,
                    IdCargo = Form.IdCargo.Value,
                    IdEvaluado = evaluado.IdEvaluado,
                    IdUsuarioEvaluador = IdUsuarioEvaluador,
                    FechaEvaluacion = DateTime.Now,
                    IdTipoEstadoEvaluacion = TipoEstadoEvaluacion.Creada,
                };

                db.Evaluacions.InsertOnSubmit(evaluacion);
                db.SubmitChanges();
                
                return RedirectToAction("CrearEvaluacion", "Evaluacion", new { IdEvaluacion = evaluacion.IdEvaluacion });
            }
            CrearEvaluadoViewModel model = new CrearEvaluadoViewModel(Form);
            return View(model);
        }

        public ActionResult GetCargo(FormCollection formCollection)
        {
            var formValue = formCollection.GetValues(0)[0];
            int IdEmpresa = Int16.Parse(formValue);
            IEnumerable<SelectListItem> items = db.Cargos
                .Where(x => x.IdEmpresa == IdEmpresa)
                .GroupBy(x => new { x.Cargo1, x.IdCargo })
                .Select(x => new SelectListItem()
                {
                    Text = x.Key.Cargo1,
                    Value = x.Key.IdCargo.ToString()
                });
            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}