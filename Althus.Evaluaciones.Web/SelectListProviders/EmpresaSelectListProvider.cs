using Althus.Evaluaciones.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Althus.Evaluaciones.Web.SelectListProviders
{
    public class EmpresaSelectListProvider : ISelectListProvider
    {
        ALTHUSEvaluacionesDataContext db = new ALTHUSEvaluacionesDataContext()
                .WithConnectionStringFromConfiguration();
        private IEnumerable<SelectListItem> Empresas;

        public SelectList Provide()
        {
            Empresas = db.Empresas
                .OrderBy(x => x.Empresa1)
                .Select(x => new SelectListItem()
                {
                    Value = x.IdEmpresa.ToString(),
                    Text = x.Empresa1
                });
            return new SelectList(Empresas, "Value", "Text");
        }

        public SelectList Provide(object selected)
        {
            Empresas = db.Empresas
                .OrderBy(x => x.Empresa1)
                .Select(x => new SelectListItem()
                {
                    Value = x.IdEmpresa.ToString(),
                    Text = x.Empresa1
                });
            return new SelectList(Empresas, "Value", "Text", selected);
        }
    }
}