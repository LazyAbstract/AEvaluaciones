using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class ListadoEvaluacionesFormModel
    {
        [DisplayName("Empresa")]
        public int? IdEmpresa { get; set; }

        [DisplayName("Cargo")]
        public int? IdCargo { get; set; }
    }
}
