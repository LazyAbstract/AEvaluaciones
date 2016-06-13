using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Althus.Evaluaciones.Core
{
    public partial class Evaluado
    {
        public string NombreCompleto
        {
            get
            {
                return this.Nombre + " " + this.ApellidoPaterno + " " + this.ApellidoMaterno;
            }
        }
    }
}
