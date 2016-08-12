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
            if(!User.IsInRole("Usuario") && UsuarioActual != null)
            {
                items = items.Where(x => x.IdUsuarioEvaluador == UsuarioActual.IdUsuario);
            }
            if (!String.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLower();
                items = items
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
            Evaluacion evaluacion = db.Evaluacions.Single(x => x.IdEvaluacion == Form.IdEvaluacion);
            if (UsuarioActual != null) evaluacion.IdUsuarioEvaluador = UsuarioActual.IdUsuario;
            IEnumerable<Competencia> Competencias = evaluacion.Cargo.Competencias.OrderBy(x => x.IdCompetencia);

            #region Si la Evaluación no está completa
            if (!Form.Finalizada)
            {
                EvaluacionAbierta text1 = evaluacion.EvaluacionAbiertas
                    .SingleOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ResumenEstudiosTrayectoriaLaboral);
                if (text1 != null)
                {
                    text1.EvaluacionAbierta1 = String.IsNullOrEmpty(Form.TrayectoriaLaboral) ? "" : Form.TrayectoriaLaboral;
                }
                else
                {
                    text1 = new EvaluacionAbierta()
                    {
                        IdTipoEvaluacionAbierta = TipoEvaluacionAbierta.ResumenEstudiosTrayectoriaLaboral,
                        IdEvaluacion = evaluacion.IdEvaluacion,
                        EvaluacionAbierta1 = String.IsNullOrEmpty(Form.TrayectoriaLaboral) ? "" : Form.TrayectoriaLaboral,
                    };
                    db.EvaluacionAbiertas.InsertOnSubmit(text1);
                }

                EvaluacionAbierta text2 = evaluacion.EvaluacionAbiertas
                    .SingleOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.MotivacionCargo);
                if (text2 != null)
                {
                    text2.EvaluacionAbierta1 = String.IsNullOrEmpty(Form.MotivacionPorCargo) ? "" : Form.MotivacionPorCargo;
                }
                else
                {
                    text2 = new EvaluacionAbierta()
                    {
                        IdTipoEvaluacionAbierta = TipoEvaluacionAbierta.MotivacionCargo,
                        IdEvaluacion = evaluacion.IdEvaluacion,
                        EvaluacionAbierta1 = String.IsNullOrEmpty(Form.MotivacionPorCargo) ? "" : Form.MotivacionPorCargo,
                    };
                    db.EvaluacionAbiertas.InsertOnSubmit(text2);
                }

                EvaluacionAbierta text3 = evaluacion.EvaluacionAbiertas
                   .SingleOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ConclusionSugerencias);
                if (text3 != null)
                {
                    text3.EvaluacionAbierta1 = String.IsNullOrEmpty(Form.ConclusionSugerencia) ? "" : Form.ConclusionSugerencia;
                }
                else
                {
                    text3 = new EvaluacionAbierta()
                    {
                        IdTipoEvaluacionAbierta = TipoEvaluacionAbierta.ConclusionSugerencias,
                        IdEvaluacion = evaluacion.IdEvaluacion,
                        EvaluacionAbierta1 = String.IsNullOrEmpty(Form.ConclusionSugerencia) ? "" : Form.ConclusionSugerencia,
                    };
                    db.EvaluacionAbiertas.InsertOnSubmit(text3);
                }

                //  evaluacion de competencias    
                int i = 0;
                foreach (var competencia in Competencias.OrderBy(x => x.IdCompetencia))
                {
                    EvaluacionCompetencia evalcomp = evaluacion.EvaluacionCompetencias.SingleOrDefault(x => x.IdCompetencia == competencia.IdCompetencia);
                    if(evalcomp != null)
                    {
                        evalcomp.ValorObtenido = Form.ValorObtenidoCompetencia[i];
                        evalcomp.Observacion = Form.Observacion[i];
                    }
                    else
                    {
                        evalcomp = new EvaluacionCompetencia()
                        {
                            IdEvaluacion = evaluacion.IdEvaluacion,
                            IdCompetencia = competencia.IdCompetencia,
                            ValorObtenido = Form.ValorObtenidoCompetencia[i],
                            Observacion = Form.Observacion[i],
                        };
                        db.EvaluacionCompetencias.InsertOnSubmit(evalcomp);
                    }
                    i++;
                }                

                evaluacion.IdTipoEstadoEvaluacion = TipoEstadoEvaluacion.EnProgreso;
                db.SubmitChanges();
                Mensaje = "La Evaluación no ha sido finalizada! Recurde que debe completar todos los campos para ver el detalle.";
                return RedirectToAction("ListadoEvaluaciones", new { IdEvaluacion = evaluacion.IdEvaluacion });
            }
            #endregion

            #region Si la evaluación está completa
            else
            {
                if(ModelState.IsValid)
                {
                    EvaluacionAbierta text1 = evaluacion.EvaluacionAbiertas
                    .SingleOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ResumenEstudiosTrayectoriaLaboral);
                    if (text1 != null)
                    {
                        text1.EvaluacionAbierta1 = Form.TrayectoriaLaboral;
                    }
                    else
                    {
                        text1 = new EvaluacionAbierta()
                        {
                            IdTipoEvaluacionAbierta = TipoEvaluacionAbierta.ResumenEstudiosTrayectoriaLaboral,
                            IdEvaluacion = evaluacion.IdEvaluacion,
                            EvaluacionAbierta1 = Form.TrayectoriaLaboral,
                        };
                        db.EvaluacionAbiertas.InsertOnSubmit(text1);
                    }

                    EvaluacionAbierta text2 = evaluacion.EvaluacionAbiertas
                        .SingleOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.MotivacionCargo);
                    if (text2 != null)
                    {
                        text2.EvaluacionAbierta1 = Form.MotivacionPorCargo;
                    }
                    else
                    {
                        text2 = new EvaluacionAbierta()
                        {
                            IdTipoEvaluacionAbierta = TipoEvaluacionAbierta.MotivacionCargo,
                            IdEvaluacion = evaluacion.IdEvaluacion,
                            EvaluacionAbierta1 = Form.MotivacionPorCargo,
                        };
                        db.EvaluacionAbiertas.InsertOnSubmit(text2);
                    }

                    EvaluacionAbierta text3 = evaluacion.EvaluacionAbiertas
                       .SingleOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ConclusionSugerencias);
                    if (text3 != null)
                    {
                        text3.EvaluacionAbierta1 = Form.ConclusionSugerencia;
                    }
                    else
                    {
                        text3 = new EvaluacionAbierta()
                        {
                            IdTipoEvaluacionAbierta = TipoEvaluacionAbierta.ConclusionSugerencias,
                            IdEvaluacion = evaluacion.IdEvaluacion,
                            EvaluacionAbierta1 = Form.ConclusionSugerencia,
                        };
                        db.EvaluacionAbiertas.InsertOnSubmit(text3);
                    }

                    //  evaluacion de competencias                
                    int i = 0;
                    List<int> ValoresCalculoIdioneidad = new List<int>();
                    foreach (var competencia in Competencias)
                    {
                        EvaluacionCompetencia evalcomp = evaluacion.EvaluacionCompetencias.SingleOrDefault(x => x.IdCompetencia == competencia.IdCompetencia);
                        if (evalcomp != null)
                        {
                            evalcomp.ValorObtenido = Form.ValorObtenidoCompetencia[i];
                            evalcomp.Observacion = Form.Observacion[i];
                        }
                        else
                        {
                            evalcomp = new EvaluacionCompetencia()
                            {
                                IdEvaluacion = evaluacion.IdEvaluacion,
                                IdCompetencia = competencia.IdCompetencia,
                                ValorObtenido = Form.ValorObtenidoCompetencia[i],
                                Observacion = Form.Observacion[i],
                            };
                            db.EvaluacionCompetencias.InsertOnSubmit(evalcomp);
                        }
                        db.SubmitChanges();

                        if (evalcomp.ValorObtenido > evalcomp.Competencia.ValorEsperado)
                        {
                            ValoresCalculoIdioneidad.Add(evalcomp.Competencia.ValorEsperado);
                        }
                        else
                        {
                            ValoresCalculoIdioneidad.Add(evalcomp.ValorObtenido);
                        }
                        i++;
                    }
                    int TotalEsperado = Competencias.Sum(x => x.ValorEsperado);
                    int TotalObtenido = 0;
                    foreach (var item in ValoresCalculoIdioneidad)
                    {
                        TotalObtenido += item;
                    }
                    float PorcentajeIdionidad = (float)TotalObtenido / TotalEsperado;
                    //if (PorcentajeIdionidad > 1) PorcentajeIdionidad = 1;
                    IEnumerable<TipoDiagnostico> TipoDiagnosticos = db.TipoDiagnosticos.Where(x => x.PorcentajeHasta >= PorcentajeIdionidad);
                    int IdTipoDiagnostico = TipoDiagnosticos.OrderBy(x => x.PorcentajeHasta).First().IdTipoDiagnostico;
                    evaluacion.PorcetajeIdioneidad = PorcentajeIdionidad;
                    evaluacion.IdTipoDiagnostico = IdTipoDiagnostico;
                    evaluacion.FechaEvaluacion = DateTime.Now;
                    db.SubmitChanges();

                    evaluacion.IdTipoEstadoEvaluacion = TipoEstadoEvaluacion.Finalizada;
                    db.SubmitChanges();
                    Mensaje = "La Evaluación ha sido finalizada correctamente.";
                    return RedirectToAction("DetalleEvaluacion", new { IdEvaluacion = evaluacion.IdEvaluacion });
                }
            }
            #endregion

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
            GeneraEvaluacionPDF generador = 
                new GeneraEvaluacionPDF(evaluacion, evaluacion.Cargo.Empresa.Logo.ToArray());
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