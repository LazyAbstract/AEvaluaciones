using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class CrearEvaluacionFormModel
    {
        [Required]
        public string TrayectoriaLaboral { get; set; }
        [Required]
        public string MotivacionPorCargo { get; set; }
        [Required]
        public string ConclusionSugerencia { get; set; }
        [Required]
        public List<int> ValoresObtenidosCompetencia { get; set; }
    }
}