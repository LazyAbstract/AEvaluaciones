using Althus.Evaluaciones.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class CrearEvaluacionViewModel
    {
        private ALTHUSEvaluacionesDataContext db = new ALTHUSEvaluacionesDataContext()
            .WithConnectionStringFromConfiguration();
        public CrearEvaluacionFormModel Form { get; set; }
        public IEnumerable<Competencia> Competencias { get; set; }
        public Evaluacion evaluacion { get; set; }
        public Evaluado evaluado { get; set; }

        public CrearEvaluacionViewModel()
        {
            Form = new CrearEvaluacionFormModel();
        }

        public CrearEvaluacionViewModel(int idEvaluacion) : this()
        {
            Form.IdEvaluacion = idEvaluacion;
            evaluacion = db.Evaluacions.Single(x => x.IdEvaluacion == idEvaluacion);
            evaluado = evaluacion.Evaluado;
            Competencias = evaluacion.Cargo.Competencias.OrderBy(x => x.IdCompetencia);

            EvaluacionAbierta hola = evaluacion.EvaluacionAbiertas.SingleOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ResumenEstudiosTrayectoriaLaboral);
            if(hola != null )Form.TrayectoriaLaboral = hola.EvaluacionAbierta1;

            EvaluacionAbierta hola2 = evaluacion.EvaluacionAbiertas.SingleOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.MotivacionCargo);
            if (hola2 != null) Form.MotivacionPorCargo = hola2.EvaluacionAbierta1;


            EvaluacionAbierta hola3 = evaluacion.EvaluacionAbiertas.SingleOrDefault(x => x.IdTipoEvaluacionAbierta == TipoEvaluacionAbierta.ConclusionSugerencias);
            if (hola3 != null) Form.ConclusionSugerencia = hola3.EvaluacionAbierta1;

            int i = 0;
            foreach(var comp in evaluacion.Cargo.Competencias.OrderBy(x => x.IdCompetencia))
            {
                EvaluacionCompetencia evalcomp = evaluacion.EvaluacionCompetencias.SingleOrDefault(x => x.IdCompetencia == comp.IdCompetencia);
                if (evalcomp != null)
                {
                    Form.ValorObtenidoCompetencia.Add(evalcomp.ValorObtenido);
                    Form.Observacion.Add(evalcomp.Observacion);
                }
                else
                {
                    Form.ValorObtenidoCompetencia.Add(0);
                    Form.Observacion.Add("");
                }
                i++;
            }

            if(evaluacion.IdTipoEstadoEvaluacion == TipoEstadoEvaluacion.Finalizada)
            {
                Form.Finalizada = true;
            }

        }

        public CrearEvaluacionViewModel(CrearEvaluacionFormModel F) : this()
        {
            Form = F;
            evaluacion = db.Evaluacions.Single(x => x.IdEvaluacion == Form.IdEvaluacion);
            evaluado = evaluacion.Evaluado;
            Competencias = evaluacion.Cargo.Competencias.OrderBy(x => x.IdCompetencia);
        }
    }
}