using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Models.EmpresaModels
{
    public class CrearEditarEmpresaFormModel
    {
        private ALTHUSEvaluacionesDataContext db = new ALTHUSEvaluacionesDataContext()
           .WithConnectionStringFromConfiguration();
        public int? IdEmpresa { get; set; }
        [Required]
        [DisplayName("Nombre Empresa")]
        public string Empresa1 { get; set; }
        [AvalidFile(Allowed = new string[] { ".png" }, MaxLength = 1024 * 1024 , ErrorMessage = "El archivo debe tener extensión .png y pesar, cómo máximo, 1MG.")]
        public HttpPostedFileBase Logo { get; set; }

        public string Error
        {
            get { return String.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Logo":
                        if (Logo == null || (Logo != null && Logo.ContentLength <= 0))
                        {
                            return "Debe subir un archivo válido.";
                        }
                        break;
                    case "Empresa1":
                        if(db.Empresas.Any(x => x.Empresa1 == Empresa1) && !IdEmpresa.HasValue)
                        {
                            return "El Nombre de la empresa ya está en uso.";
                        }
                        break;
                }
                return String.Empty;
            }

        }
    }
}