using Althus.Evaluaciones.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Models.EvaluadoModels
{
    public class CrearEvaluadoFormModel : IDataErrorInfo
    {
        [Required]
        [DisplayName("Empresa")]
        public int IdEmpresa { get; set; }
        [Required]
        [DisplayName("Cargo")]
        public int IdCargo { get; set; }
        [Required]
        public Rut Rut { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "El nombre no puede contener más de 255 caracteres.")]
        public string Nombre { get; set; }
        [Required]
        [DisplayName("Apellido Paterno")]
        [StringLength(255, ErrorMessage = "El apellido paterno no puede contener más de 255 caracteres.")]
        public string ApellidoPaterno { get; set; }
        [DisplayName("Apellido Materno")]
        [StringLength(255, ErrorMessage = "El nombre no puede contener más de 255 caracteres.")]
        public string ApellidoMaterno { get; set; }
        [Required]
        [DisplayName("Profesión")]
        [StringLength(255, ErrorMessage = "La profesión no puede contener más de 255 caracteres.")]
        public string Profesion { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El número de celular debe tener exactamente 8 caracteres.")]
        public string Celular { get; set; }
        [Required]
        [DisplayName("Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }
        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        #region IDataErrorInfo Members

        public string Error
        {
            get
            {
                ALTHUSEvaluacionesDataContext db = new ALTHUSEvaluacionesDataContext()
                    .WithConnectionStringFromConfiguration();
                if (db.Evaluados.Any(x => x.Correo == Correo))
                {
                    return "El Correo del postulante ingresado ya está en uso.";
                }
                if (db.Evaluados.Any(x => x.Rut == Rut.Numero))
                {
                    return "El Rut del postulante ya ingresado anteriormente.";
                }
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get { return string.Empty; }
        }

        #endregion
    }
}