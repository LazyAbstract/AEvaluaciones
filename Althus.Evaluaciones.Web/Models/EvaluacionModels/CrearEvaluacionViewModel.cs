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
        }

        public CrearEvaluacionViewModel(CrearEvaluacionFormModel F) : this()
        {
            Form = F;
        }
    }
}