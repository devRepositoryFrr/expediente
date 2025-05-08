using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConaviWeb.Model.Diagnostico
{
    public class Beneficiario
    {
        public string Nombre { get; set; }
        public string PrimerAp { get; set; }
        public string SegundoAp { get; set; }
        public string Genero { get; set; }
        public string Telefono { get; set; }
        public string TelAlt { get; set; }
        public string Entidad { get; set; }
        public string Municipio { get; set; }
        public string Localidad { get; set; }
        public string Calle { get; set; }
        public string NumeroExt { get; set; }
        public string NumeroInt { get; set; }
        public string Colonia { get; set; }
        public string Cp { get; set; }
        public string Referencia { get; set; }
    }
}
