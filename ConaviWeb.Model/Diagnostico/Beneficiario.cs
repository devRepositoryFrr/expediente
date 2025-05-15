using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConaviWeb.Model.Diagnostico
{
    public class Beneficiario
    {
        public int Id { get; set; }
        public string IdUnico { get; set; }
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
        public string Prueba { get; set; }
        public string Esquema { get; set; }
        public string Identificador { get; set; }
        public int NuVisita { get; set; }
        public DateTime? FchProgramacion { get; set; }
        public string Localizo { get; set; }
        public string Acordo { get; set; }
        public DateTime? FchAcordada { get; set; }
        public string Lugar { get; set; }
        public string Motivo { get; set; }
        public byte[] ImgInmueble { get; set; }
        public byte[] ImgCurpA { get; set; }
        public byte[] ImgIdA { get; set; }
        public byte[] ImgIdR { get; set; }
        public byte[] ImgCompDom { get; set; }
        public byte[] ImgVivienda_1 { get; set; }
        public byte[] ImgVivienda_2 { get; set; }
        public byte[] ImgVivienda_3 { get; set; }
        public byte[] ImgVivienda_4 { get; set; }
        public byte[] ImgMatMuro { get; set; }
        public byte[] ImgMatTecho { get; set; }
        public byte[] ImgMatPiso { get; set; }
        public byte[] ImgFirma { get; set; }
        public byte[] ImgCurpACIS { get; set; }
        public byte[] ImgActaNacimientoCIS { get; set; }
        public byte[] ImgFotoLugarCIS { get; set; }
        public byte[] ImgIneACIS { get; set; }
        public byte[] ImgIneRCIS { get; set; }
        public byte[] ImgEstudiosCIS { get; set; }
        public byte[] ImgCompNoPropCIS { get; set; }
        public byte[] ImgCompPropCIS { get; set; }
        public byte[] ImgCompDomCIS { get; set; }
        public byte[] ImgCurp_ACIS { get; set; }
        public byte[] ImgId_ACIS { get; set; }
        public byte[] ImgId_RCIS { get; set; }
        public byte[] ImgCompIngCIS { get; set; }
        public byte[] ImgCartaNoDerCIS { get; set; }
        public byte[] ImgFirmaCIS { get; set; }

    }
}
