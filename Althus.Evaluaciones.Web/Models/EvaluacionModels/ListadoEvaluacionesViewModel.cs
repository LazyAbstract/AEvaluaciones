using Althus.Evaluaciones.Core;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class ListadoEvaluacionesViewModel
    {
        public IPagedList<Evaluacion> Evaluaciones { get; set; }
    }
}