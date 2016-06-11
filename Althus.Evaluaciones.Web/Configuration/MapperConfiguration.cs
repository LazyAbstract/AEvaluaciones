using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Althus.Evaluaciones.Web.MapperProfile;

namespace Althus.Evaluaciones.Web.Configuration
{
    public class MapperConfiguration : IConfigurable
    {
        public void Configure()
        {
            Mapper.Initialize(x => GetConfiguration(Mapper.Configuration));
        }

        private void GetConfiguration(IConfiguration configuration)
        {
            var profiles = typeof(DefaultProfileMapper).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x));
            foreach (var profile in profiles)
            {
                configuration.AddProfile(Activator.CreateInstance(profile) as Profile);
            }
        }
    }
}