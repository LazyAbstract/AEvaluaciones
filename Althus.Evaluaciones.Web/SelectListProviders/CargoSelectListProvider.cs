using Althus.Evaluaciones.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Althus.Evaluaciones.Web.SelectListProviders
{
    public class CargoSelectListProvider : ISelectListProvider
    {
        ALTHUSEvaluacionesDataContext db = new ALTHUSEvaluacionesDataContext()
               .WithConnectionStringFromConfiguration();
        private IEnumerable<SelectListItem> Cargos;

        public SelectList Provide()
        {
            Cargos = db.Cargos
                .OrderBy(x => x.Cargo1)
                .Select(x => new SelectListItem()
                {
                    Value = x.IdCargo.ToString(),
                    Text = x.Cargo1
                });
            return new SelectList(Cargos, "Value", "Text");
        }

        public SelectList Provide(object selected)
        {
            Cargos = db.Cargos
                .OrderBy(x => x.Cargo1)
                .Select(x => new SelectListItem()
                {
                    Value = x.IdCargo.ToString(),
                    Text = x.Cargo1
                });
            return new SelectList(Cargos, "Value", "Text", selected);
        }
    }
}