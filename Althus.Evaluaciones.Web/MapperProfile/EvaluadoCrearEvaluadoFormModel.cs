using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.Models.EvaluadoModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.MapperProfile
{
    public class EvaluadoCrearEvaluadoFormModel : Profile
    {
        public override string ToString()
        {
            return "CrearEvaluadoFormModel -> Evaluado";
        }

        protected override void Configure()
        {
            //base.Configure();
            Mapper.CreateMap<Evaluado, CrearEvaluadoFormModel>()
                .ForMember(d => d.Rut, prop => prop.MapFrom(x => new Rut(x.Rut)));
        }
    }
}