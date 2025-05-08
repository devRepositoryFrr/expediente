using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConaviWeb.Model.Proyecto
{
    public class Lineas
    {
        public int? id { get; set; }
        public string cmb_ecotecnias { get; set; }
        public float? txt_acristalamiento { get; set; }
        public float? txt_mef_techo { get; set; }
        public float? txt_mef_muro { get; set; }
        public float? txt_reflec_techo { get; set; }
        public float? txt_reflec_muro { get; set; }
        public int? cmb_solar { get; set; }
        public string txt_solar { get; set; }
        public int? cmb_inhodoro { get; set; }
        public int? cmb_regadera { get; set; }
        public int? cmb_llaves_bano { get; set; }
        public int? cmb_llaves_cocina { get; set; }
        public int? cmb_pluvial { get; set; }
        public int? txt_lamparas { get; set; }
        public int? txt_leds { get; set; }
        public int? txt_bombeo { get; set; }
        public int? cmb_tipocombustible { get; set; }
        public string txt_tipocombustible { get; set; }
        public int? cmb_calentador { get; set; }
        public int? cmb_calentador_solar { get; set; }
        public int? cmb_estufa { get; set; }
        public int? txt_arbol { get; set; }
        public float? txt_reforz_agua { get; set; }
        public float? txt_obrpoz { get; set; }
        public float? txt_reforzdrenaje { get; set; }
        public int? cmb_trataprovagua { get; set; }
        public int? cmb_stmaguares { get; set; }
        public float? txt_refene { get; set; }
        public int? txt_lamsolac { get; set; }
        public int? txt_bomsol { get; set; }
        public int? cmb_sisfot { get; set; }
        public int? cmb_aerogenerador { get; set; }
        public string cmb_motrices { get; set; }
        public string cmb_obra_preventivas { get; set; }
        public string txt_obra_preventiva { get; set; }
        public float? volumen_demolicion { get; set; }
        public int? estatus { get; set; }
        public string txt_estatus { get; set; }
        public string fecha_registro { get; set; }
        public string fecha_update { get; set; }
        public int idUser { get; set; }
        public int idPredio { get; set; }
    }
}
