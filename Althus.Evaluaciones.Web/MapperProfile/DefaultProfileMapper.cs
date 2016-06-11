using Althus.Evaluaciones.Core;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.MapperProfile
{
    public class DefaultProfileMapper : Profile
    {
        public override string ProfileName
        {
            get
            {
                return "DefaultProfileMapper";
            }
        }

        protected override void Configure()
        {
            //base.Configure();
            CreateMap<Rut, string>()
                .ConvertUsing(x => ((Rut)x).ToString());
            CreateMap<int, Rut>()
                .ConvertUsing(x => new Rut(x));
        }
    }
}