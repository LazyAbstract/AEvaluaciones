using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.Models.EmpresaModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Althus.Evaluaciones.Web.Controllers
{
    public class EmpresaController : BaseController
    {
        public ActionResult CrearEditarEmpresa(int? IdEmpresa)
        {
            CrearEditarEmpresaViewModel model = new CrearEditarEmpresaViewModel();
            if(IdEmpresa.HasValue)
            {
                Empresa empresa = db.Empresas.Single(x => x.IdEmpresa == IdEmpresa.Value);
                model.Form.IdEmpresa = empresa.IdEmpresa;
                model.Form.Empresa1 = empresa.Empresa1;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CrearEditarEmpresa(CrearEditarEmpresaFormModel Form)
        {
            if(ModelState.IsValid)
            {
                if(Form.IdEmpresa.HasValue)
                {
                    int largo = Form.Logo.ContentLength;
                    byte[] buffer = new byte[largo];
                    Form.Logo.InputStream.Read(buffer, 0, largo);
                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        Empresa empresa = db.Empresas.Single(x => x.IdEmpresa == Form.IdEmpresa.Value);
                        empresa.Empresa1 = Form.Empresa1;
                        empresa.Logo = buffer;
                        db.SubmitChanges();
                    }
                    Mensaje = "La empresa fue ediatada exitosamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    int largo = Form.Logo.ContentLength;
                    byte[] buffer = new byte[largo];
                    Form.Logo.InputStream.Read(buffer, 0, largo);
                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        Empresa empresa = new Empresa()
                        {
                            Empresa1 = Form.Empresa1,
                            Logo = buffer,
                        };
                        db.Empresas.InsertOnSubmit(empresa);
                        db.SubmitChanges();
                    }
                    Mensaje = "La empresa fue creada exitosamente.";
                    return RedirectToAction("Index");
                }
            }
            CrearEditarEmpresaViewModel model = new CrearEditarEmpresaViewModel(Form);
            return View(model);
        }
    }
}