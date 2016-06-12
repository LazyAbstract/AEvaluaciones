using Althus.Evaluaciones.Web.Models.EvaluacionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Althus.Evaluaciones.Web.Controllers
{
    public class EvaluacionController : BaseController
    {
        public ActionResult ListadoEvaluaciones(int? pagina)
        {
            ListadoEvaluacionesViewModel model = new ListadoEvaluacionesViewModel();
            model.Evaluaciones = db.Evaluacions.ToPagedList(pagina ?? 1, 10);
            return View(model);
        }

        public ActionResult CrearPostulante()
        {
            return View();
        }

        public ActionResult CrearEvaluacion(int IdEvaluacion)
        {
            CrearEvaluacionViewModel model = new CrearEvaluacionViewModel();
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