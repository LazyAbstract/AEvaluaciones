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
        public ActionResult CrearEvaluado()
        {
            CrearEvaluadoViewModel model = new CrearEvaluadoViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CrearEvaluado(CrearEvaluadoFormModel Form)
        {
            if(ModelState.IsValid)
            {
                Evaluado evaluado = Mapper.Map<CrearEvaluadoFormModel, Evaluado>(Form);
                evaluado.CreadoPor = User.Identity.Name;
                db.Evaluados.InsertOnSubmit(evaluado);
                db.SubmitChanges();
                string Nombreusuario = User.Identity.Name;
                Usuario usuario = db.Usuarios.SingleOrDefault(x => x.NombreUsuario == Nombreusuario);
                int? IdUsuarioEvaluador = null;
                if(usuario != null) IdUsuarioEvaluador = usuario.IdUsuario;
                Evaluacion evaluacion = new Evaluacion()
                {
                    IdTipoEvaluacion = TipoEvaluacion.Evaluacion_Psicologica_CCU,
                    IdCargo = Form.IdCargo,
                    IdEvaluado = evaluado.IdEvaluado,
                    IdUsuarioEvaluador = IdUsuarioEvaluador,
                    FechaEvaluacion = DateTime.Now,
                };

                db.Evaluacions.InsertOnSubmit(evaluacion);
                db.SubmitChanges();
                Mensaje = "El Postulante fue ingresado exitosamente.";
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