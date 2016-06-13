using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class CrearEvaluacionFormModel
    {
        public int IdEvaluacion { get; set; }
        [Required]
        [DisplayName("Resumen Estudios y Trayectoria Laboral")]
        public string TrayectoriaLaboral { get; set; }
        [Required]
        [DisplayName("Motivación por el Cargo")]
        public string MotivacionPorCargo { get; set; }
        [Required]
        [DisplayName("Conclusión y Sugerencias")]
        public string ConclusionSugerencia { get; set; }
        [Required]
        public List<int> ValorObtenidoCompetencia { get; set; }
        public List<string> Observacion { get; set; }
    }
}