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
                Mensaje = "El Postulante fue ingresado exitosamente.";
                return RedirectToAction("");
            }
            CrearEvaluadoViewModel model = new CrearEvaluadoViewModel(Form);
            return View(model);
        }
    }
}