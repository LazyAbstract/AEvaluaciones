using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.Models.UsuarioModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.MapperProfile.UsuarioMappers
{
    public class CrearEditarUsuarioFormModelUsuario
    {
        public override string ToString()
        {
            return "CrearEditarUsuarioFormModel -> Usuario";
        }

        protected override void Configure()
        {
            //base.Configure();
            Mapper.CreateMap<CrearEditarUsuarioFormModel, Usuario>()
                .ForMember(d => d.Rut, prop => prop.MapFrom(x => x.Rut.Numero));
        }
    }
}