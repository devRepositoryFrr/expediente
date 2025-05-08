using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConaviWeb.Model.Proyecto
{
    public class Edificio
    {
        public int Id { get; set; }
        public int Etapa { get; set; }
        public int Manzana { get; set; }
        public string Nomenclatura {  get; set; }
        public int Viviendas {  get; set; }
        public int IdPredio { get; set; }
        public int IdUser {  get; set; }
        public int TotalManzanas { get; set; }
        public int TotalEdificios { get; set; }
        public int TotalViviendas { get; set; }
    }
}
