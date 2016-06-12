using Althus.Evaluaciones.Core;
using Althus.Evaluaciones.Web.Models.EvaluadoModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.MapperProfile
{
    public class CrearEvaluadoFormModelEvaluado : Profile
    {
        public override string ToString()
        {
            return "CrearEvaluadoFormModel -> Evaluado";
        }

        protected override void Configure()
        {
            //base.Configure();
            Mapper.CreateMap<CrearEvaluadoFormModel, Evaluado>()
                .ForMember(d => d.Rut, prop => prop.MapFrom(x => x.Rut.Numero))
                .ForMember(d => d.FechaCreacion, prop => prop.MapFrom(x => DateTime.Now));
        }
    }
}