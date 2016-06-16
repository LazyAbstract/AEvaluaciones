using Althus.Evaluaciones.Core;
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
        private ALTHUSEvaluacionesDataContext db = new ALTHUSEvaluacionesDataContext()
            .WithConnectionStringFromConfiguration();
        public CrearEvaluadoFormModel Form { get; set; }
        public SelectList Empresas { get; set; }
        public IEnumerable<SelectListItemCargo> Cargos { get; set; }
        private EmpresaSelectListProvider eslp = new EmpresaSelectListProvider();
        private CargoSelectListProvider cslp = new CargoSelectListProvider();
        public Rut filtro { get; set; }

        public CrearEvaluadoViewModel()
        {
            Form = new CrearEvaluadoFormModel();
            Empresas = eslp.Provide();
            Cargos = db.vw_RelacionEmpresaCargos
                .Select(x => new SelectListItemCargo()
                {
                    Value = x.IdCargo.ToString(),
                    Text = x.Cargo,
                    OptGroup = x.Empresa
                });
        }

        public CrearEvaluadoViewModel(CrearEvaluadoFormModel F) : this()
        {
            Form = F;
        }
    }

    public class SelectListItemCargo
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
        public string OptGroup { get; set; }
    }
}