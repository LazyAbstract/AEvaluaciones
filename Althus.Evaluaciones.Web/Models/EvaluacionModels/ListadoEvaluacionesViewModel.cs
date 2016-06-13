using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.Models.EvaluadoModels;
using Althus.Evaluaciones.Web.SelectListProviders;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Althus.Evaluaciones.Web.Models.EvaluacionModels
{
    public class ListadoEvaluacionesViewModel
    {
        public ListadoEvaluacionesFormModel FORM { get; set; }
        public IPagedList<Evaluacion> Evaluaciones { get; set; }
        public SelectList Empresas { get; set; }
        public IEnumerable<SelectListItemCargo> Cargos { get; set; }
        private EmpresaSelectListProvider eslp = new EmpresaSelectListProvider();
        private CargoSelectListProvider cslp = new CargoSelectListProvider();

        public ListadoEvaluacionesViewModel()
        {
            FORM = new ListadoEvaluacionesFormModel();
        }

        public ListadoEvaluacionesViewModel(ListadoEvaluacionesFormModel form, ALTHUSEvaluacionesDataContext db)
        {
            FORM = form;
            Empresas = eslp.Provide();
            Cargos = db.vw_RelacionEmpresaCargos
                .Select(x => new SelectListItemCargo()
                {
                    Value = x.IdCargo.ToString(),
                    Text = x.Cargo,
                    OptGroup = x.Empresa
                });
        }
    }
}