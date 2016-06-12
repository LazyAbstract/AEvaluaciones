using Althus.Evaluaciones.Web.Models.EvaluacionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Althus.Evaluaciones.Core;

namespace Althus.Evaluaciones.Web.Controllers
{
    public class EvaluacionController : BaseController
    {
        public ActionResult ListadoEvaluaciones(int? pagina, ListadoEvaluacionesFormModel FORM)
        {
            ListadoEvaluacionesViewModel model = new ListadoEvaluacionesViewModel(FORM,db);
            IQueryable<Evaluacion> items = db.Evaluacions;
            if(ModelState.IsValid){
            }
            model.Evaluaciones = items.OrderByDescending(x=> x.FechaEvaluacion)
                .ToPagedList(pagina ?? 1, 10);
            return View(model);
        }

        public ActionResult CrearEvaluacion(int IdEvaluacion)
        {
            CrearEvaluacionViewModel model = new CrearEvaluacionViewModel(IdEvaluacion);
            return View(model);
        }

        [HttpPost]
        public ActionResult CrearEvaluacion(CrearEvaluacionFormModel Form)
        {
            if(ModelState.IsValid)
            {

            }
            CrearEvaluacionViewModel model = new CrearEvaluacionViewModel(Form);
            return View(model);
        }
    }
}