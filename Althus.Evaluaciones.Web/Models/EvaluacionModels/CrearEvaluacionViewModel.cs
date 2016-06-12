using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class CrearEvaluacionViewModel
    {
        public CrearEvaluacionFormModel Form { get; set; }

        public CrearEvaluacionViewModel()
        {
            Form = new CrearEvaluacionFormModel();
        }

        public CrearEvaluacionViewModel(CrearEvaluacionFormModel F) : this()
        {
            Form = F;
        }
    }
}