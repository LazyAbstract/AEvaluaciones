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
        public ActionResult ListadoEvaluaciones(int? pagina)
        {
            ListadoEvaluacionesViewModel model = new ListadoEvaluacionesViewModel();
            model.Evaluaciones = db.Evaluacions.ToPagedList(pagina ?? 1, 10);
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
                //  evaluacion abierta primero, esto se puede hacer genércio
                Evaluacion evaluacion = db.Evaluacions.Single(x => x.IdEvaluacion == Form.IdEvaluacion);
                EvaluacionAbierta text1 = new EvaluacionAbierta()
                {
                    IdTipoEvaluacionAbierta = TipoEvaluacionAbierta.ResumenEstudiosTrayectoriaLaboral,
                    IdEvaluacion = evaluacion.IdEvaluacion,
                    EvaluacionAbierta1 = Form.TrayectoriaLaboral,
                };
                db.EvaluacionAbiertas.InsertOnSubmit(text1);

                EvaluacionAbierta text2 = new EvaluacionAbierta()
                {
                    IdTipoEvaluacionAbierta = TipoEvaluacionAbierta.MotivacionCargo,
                    IdEvaluacion = evaluacion.IdEvaluacion,
                    EvaluacionAbierta1 = Form.MotivacionPorCargo,
                };
                db.EvaluacionAbiertas.InsertOnSubmit(text2);

                EvaluacionAbierta text3 = new EvaluacionAbierta()
                {
                    IdTipoEvaluacionAbierta = TipoEvaluacionAbierta.ConclusionSugerencias,
                    IdEvaluacion = evaluacion.IdEvaluacion,
                    EvaluacionAbierta1 = Form.ConclusionSugerencia,
                };
                db.EvaluacionAbiertas.InsertOnSubmit(text3);
                db.SubmitChanges();

                //  evaluacion de competencias                
                int i = 0;
                foreach(var competencia in evaluacion.Cargo.Competencias.OrderBy(x => x.IdCompetencia))
                {
                    EvaluacionCompetencia evalcomp = new EvaluacionCompetencia()
                    {
                        IdEvaluacion = evaluacion.IdEvaluacion,
                        IdCompetencia = competencia.IdCompetencia,
                        ValorObtenido = Form.ValorObtenidoCompetencia[i],
                        Observacion = Form.Observacion[i],
                    };
                    db.EvaluacionCompetencias.InsertOnSubmit(evalcomp);
                    i++;
                }
                db.SubmitChanges();
                Mensaje = "La Evaluación fue ingresada correctamente.";
                return RedirectToAction("ListadoEvaluaciones");
            }
            CrearEvaluacionViewModel model = new CrearEvaluacionViewModel(Form);
            return View(model);
        }
    }
}