using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConaviWeb.Model.Levantamiento
{
    public class Archivo
    {
        public int Id { get; set; }
        public string ArchivoH { get; set; }
        public int IdSeccion { get; set; }
        public int Orden { get; set; }
    }
}
