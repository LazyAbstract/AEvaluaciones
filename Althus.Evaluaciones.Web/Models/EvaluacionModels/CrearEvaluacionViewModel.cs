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

        public CrearEvaluacionViewModel()
        {
            Form = new CrearEvaluacionFormModel();
        }

        public CrearEvaluacionViewModel(int IdEvaluacion) : this()
        {
            Evaluacion evaluacion = db.Evaluacions.Single(x => x.IdEvaluacion == IdEvaluacion);
            Competencias = evaluacion.Cargo.Competencias;
        }

        public CrearEvaluacionViewModel(CrearEvaluacionFormModel F) : this()
        {
            Form = F;
        }
    }
}