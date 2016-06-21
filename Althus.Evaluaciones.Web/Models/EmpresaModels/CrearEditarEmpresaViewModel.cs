using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.Models.EmpresaModels
{
    public class CrearEditarEmpresaViewModel
    {
        public CrearEditarEmpresaFormModel Form { get; set; }

        public CrearEditarEmpresaViewModel ()
	    {
            Form = new CrearEditarEmpresaFormModel();
	    }

        public CrearEditarEmpresaViewModel(CrearEditarEmpresaFormModel F) : this()
        {
            Form = F;
        }
    }
}