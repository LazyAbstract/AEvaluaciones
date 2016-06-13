using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.Models.UsuarioModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.MapperProfile.UsuarioMappers
{
    public class UsuarioCrearEditarUsuarioFormModel
    {
        public override string ToString()
        {
            return "Usuario -> CrearEditarUsuarioFormModel";
        }

        protected override void Configure()
        {
            //base.Configure();
            Mapper.CreateMap<Usuario, CrearEditarUsuarioFormModel>();
        }
    }
}