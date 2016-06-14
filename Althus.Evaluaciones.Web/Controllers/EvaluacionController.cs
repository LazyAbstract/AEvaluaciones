using Althus.Evaluaciones.Web.Models.EvaluacionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Althus.Evaluaciones.Core;
using iTextSharp.text;
using System.IO;

namespace Althus.Evaluaciones.Web.Controllers
{
    //[Authorize(Roles = "Evaluacion")]
    public class EvaluacionController : BaseController
    {
        public ActionResult ListadoEvaluaciones(int? pagina, string filtro)
        {
            ListadoEvaluacionesViewModel model = new ListadoEvaluacionesViewModel();
            IQueryable<Evaluacion> items = db.Evaluacions;
            if (!User.IsInRole("Usuario") && UsuarioActual != null)
            {
                items = items.Where(x => x.IdUsuarioEvaluador == UsuarioActual.IdUsuario);
            }
            if (!String.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLower();
                items = db.Evaluacions
                    .Where(x => x.Cargo.Empresa.Empresa1.ToLower().Contains(filtro)
                        || x.Cargo.Cargo1.ToLower().Contains(filtro)
                        || x.Evaluado.Nombre.ToLower().Contains(filtro)
                        || x.Evaluado.ApellidoPaterno.ToLower().Contains(filtro)
                        || x.Evaluado.Correo.ToLower().Contains(filtro));
                model.filtro = filtro;
            }
            model.Evaluaciones = items.OrderByDescending(x => x.FechaEvaluacion)
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
            foreach (var item in Form.ValorObtenidoCompetencia)
            {
                if (item < 1 || item > 5)
                {
                    ModelState.AddModelError("Form.ValorObtenidoCompetencia[" + j.ToString() + "]", "Los valores de la evaluación deben ser entre 1 y 5.");
                }
                j++;
            }
            if (ModelState.IsValid)
            {
                //  evaluacion abierta primero, esto se puede hacer genércio
                Evaluacion evaluacion = db.Evaluacions.Single(x => x.IdEvaluacion == Form.IdEvaluacion);
                if (UsuarioActual != null) evaluacion.IdUsuarioEvaluador = UsuarioActual.IdUsuario;
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
                foreach (var competencia in Competencias)
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
                foreach (var item in Form.ValorObtenidoCompetencia)
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
                return RedirectToAction("DetalleEvaluacion", new { IdEvaluacion = evaluacion.IdEvaluacion });
            }
            CrearEvaluacionViewModel model = new CrearEvaluacionViewModel(Form);
            return View(model);
        }

        public ActionResult DetalleEvaluacion(int IdEvaluacion)
        {
            return View(db.Evaluacions.SingleOrDefault(x => x.IdEvaluacion == IdEvaluacion));
        }

        public ActionResult ExportarEvaluacionPdf(int IdEvaluacion)
        {
            Evaluacion evaluacion = db.Evaluacions.SingleOrDefault(x=>x.IdEvaluacion == IdEvaluacion);
            var path = Request.MapPath("~/Content/images/logo_CCU.png");
            var stream = new FileStream(path, FileMode.Open);
            byte[] imageByteArray = new byte[stream.Length];
            stream.Read(imageByteArray, 0, (int)stream.Length);
            GeneraEvaluacionPDF generador = new GeneraEvaluacionPDF(evaluacion, imageByteArray);
            return File(generador.GetFile(), "application/pdf", 
                String.Format("{0}{1}{2}_Evaluacion.pdf", 
                    evaluacion.FechaEvaluacion.Year.ToString("00"),
                    evaluacion.FechaEvaluacion.Month.ToString("00"),
                    evaluacion.FechaEvaluacion.Day.ToString("00")
                    ));
        }

        public ActionResult GetEvaluacionGrafico(int IdEvaluacion)
        {
            Evaluacion evaluacion = db.Evaluacions.SingleOrDefault(x => x.IdEvaluacion == IdEvaluacion);
            byte[] result = new GeneraGraficoEvaluacion().GenerarGrafico(evaluacion);
            return File(result, "image/jpeg", "Evaluacion.png");
        }
    }
}