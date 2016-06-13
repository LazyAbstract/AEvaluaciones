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
            int j = 0;
            foreach(var item in Form.ValorObtenidoCompetencia)
            {
                if(item < 1 || item > 5)
                {
                    ModelState.AddModelError("Form.ValorObtenidoCompetencia[" + j.ToString() + "]", "Los valores de la evaluación deben ser entre 1 y 5.");
                }
                j++;
            }
            if(ModelState.IsValid)
            {
                //  evaluacion abierta primero, esto se puede hacer genércio
                Evaluacion evaluacion = db.Evaluacions.Single(x => x.IdEvaluacion == Form.IdEvaluacion);
                IEnumerable<Competencia> Competencias = evaluacion.Cargo.Competencias.OrderBy(x => x.IdCompetencia);
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
                foreach(var competencia in Competencias)
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

                int TotalEsperado = Competencias.Sum(x => x.ValorEsperado);
                int TotalObtenido = 0;
                foreach(var item in Form.ValorObtenidoCompetencia)
                {
                    TotalObtenido += item;
                }
                float PorcentajeIdionidad = (float)TotalObtenido / TotalEsperado;
                if (PorcentajeIdionidad > 1) PorcentajeIdionidad = 1;
                IEnumerable<TipoDiagnostico> TipoDiagnosticos = db.TipoDiagnosticos.Where(x => x.PorcentajeHasta >= PorcentajeIdionidad);
                int IdTipoDiagnostico = TipoDiagnosticos.OrderBy(x => x.PorcentajeHasta).First().IdTipoDiagnostico;
                evaluacion.PorcetajeIdioneidad = PorcentajeIdionidad;
                evaluacion.IdTipoDiagnostico = IdTipoDiagnostico;
                evaluacion.FechaEvaluacion = DateTime.Now;
                db.SubmitChanges();
                Mensaje = "La Evaluación fue ingresada correctamente.";
                return RedirectToAction("ListadoEvaluaciones");
            }
            CrearEvaluacionViewModel model = new CrearEvaluacionViewModel(Form);
            return View(model);
        }
    }
}