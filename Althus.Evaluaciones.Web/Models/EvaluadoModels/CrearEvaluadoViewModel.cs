using Althus.Evaluaciones.Web.SelectListProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Althus.Evaluaciones.Web.Models.EvaluadoModels
{
    public class CrearEvaluadoViewModel
    {
        public CrearEvaluadoFormModel Form { get; set; }
        public SelectList Empresas { get; set; }
        public SelectList Cargos { get; set; }
        private EmpresaSelectListProvider eslp = new EmpresaSelectListProvider();
        private CargoSelectListProvider cslp = new CargoSelectListProvider();

        public CrearEvaluadoViewModel()
        {
            Form = new CrearEvaluadoFormModel();
            Empresas = eslp.Provide();
            Cargos = cslp.Provide();
        }

        public CrearEvaluadoViewModel(CrearEvaluadoFormModel F) : this()
        {
            Form = F;
        }
    }
}