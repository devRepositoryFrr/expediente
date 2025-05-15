using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConaviWeb.Model;
using ConaviWeb.Model.Diagnostico;
using ConaviWeb.Model.Proyecto;
using Dapper;
using MySql.Data.MySqlClient;

namespace ConaviWeb.Data.Diagnostico
{
    public class DiagnosticoRepository : IDiagnosticoRepository
    {
        private readonly MySQLConfiguration _connectionString;
        public DiagnosticoRepository(MySQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected MySqlConnection DbConnection()
        {
            return new MySqlConnection(_connectionString.ExpConnectionString);
        }
        public async Task<IEnumerable<Beneficiario>> GetBeneficiario(string curp)
        {
            var db = DbConnection();
            var sql = @"
                        SELECT 
                            cd.id_unico idUnico,
                            txtCURP curp,
                            txtNombre nombre,
                            txtPrimer_apellido primerAp,
                            txtSegundo_apellido segundoAp,
                            if(cmbId_genero = 1, 'Hombre','Mujer') genero,
                            txtTelefono telefono,
                            txtTelefonoAlterno telAlt,
                            ce.nombre_estado entidad,
                            cm.nombre_municipio municipio,
                            cl.nombre_localidad localidad,
                            txtNumExt numeroExt,
                            txtCalle calle,
                            txtNumInt numeroInt,
                            txtColonia colonia,
                            cmbCp cp,
                            txtReferencia referencia,
                            prueba,ces.descripcion esquema,identificador,
                            nu_visita NuVisita,fch_programacion FchProgramacion,localizo,acordo,fch_acordada FchAcordada,lugar,motivo
                            FROM prod_captacion.dvyr_cd cd  
                            JOIN prod_ctls.cat_estado ce on ce.id_estado=cd.cmbEntidad 
                            JOIN prod_ctls.cat_municipio cm on cm.id_estado = cd.cmbEntidad and cm.id_municipio=cd.cmbMunicipio 
                            JOIN prod_ctls.cat_localidad cl on cl.id_estado = cd.cmbEntidad and cl.id_municipio=cd.cmbMunicipio and cl.id_localidad=cd.cmbLocalidad
                            LEFT JOIN prod_predios.captacion c on c.id_unico = cd.id_unico
                            LEFT JOIN prod_predios.captacion_visita cv on cv.id_unico = cd.id_unico 
                            LEFT JOIN prod_predios.c_esquema ces on ces.id = c.id_esquema
                         WHERE txtCURP = @Curp;

                       ";
            return await db.QueryAsync<Beneficiario>(sql, new { Curp = curp });
        }
        public async Task<Beneficiario> GetImgCD(string curp)
        {
            var db = DbConnection();
            var sql = @"
                        SELECT 
                            id_unico IdUnico,
                            IF(OCTET_LENGTH(imgInmueble) = 0,null,imgInmueble) ImgInmueble,
                            IF(OCTET_LENGTH(imgCurpA) = 0,null,imgCurpA) ImgCurpA,
                            IF(OCTET_LENGTH(imgIdA) = 0,null,imgIdA) ImgIdA,
                            IF(OCTET_LENGTH(imgIdR) = 0,null,imgIdR) ImgIdR,
                            IF(OCTET_LENGTH(imgCompDom) = 0,null,imgCompDom) ImgCompDom,
                            IF(OCTET_LENGTH(imgVivienda_1) = 0,null,imgVivienda_1) ImgVivienda_1,
                            IF(OCTET_LENGTH(imgVivienda_2) = 0,null,imgVivienda_2) ImgVivienda_2,
                            IF(OCTET_LENGTH(imgVivienda_3) = 0,null,imgVivienda_3) ImgVivienda_3,
                            IF(OCTET_LENGTH(imgVivienda_4) = 0,null,imgVivienda_4) ImgVivienda_4,
                            IF(OCTET_LENGTH(imgMatMuro) = 0,null,imgMatMuro) ImgMatMuro,
                            IF(OCTET_LENGTH(imgMatTecho) = 0,null,imgMatTecho) ImgMatTecho,
                            IF(OCTET_LENGTH(imgMatPiso) = 0,null,imgMatPiso) ImgMatPiso,
                            IF(OCTET_LENGTH(imgFirma) = 0,null,imgFirma) ImgFirma
                            FROM prod_captacion.dvyr_cd
                        WHERE txtCURP = @Curp;

                       ";
            return await db.QueryFirstOrDefaultAsync<Beneficiario>(sql, new { Curp = curp });
        }
        public async Task<Beneficiario> GetImgCIS(string curp)
        {
            var db = DbConnection();
            var sql = @"
                        SELECT 
                            id_unico IdUnico,
                            IF(OCTET_LENGTH(imgCurpA) = 0,null,imgCurpA) ImgCurpACIS,
                            IF(OCTET_LENGTH(imgActaNacimiento) = 0,null,imgActaNacimiento) ImgActaNacimientoCIS,
                            IF(OCTET_LENGTH(imgFotoLugar) = 0,null,imgFotoLugar) ImgFotoLugarCIS,
                            IF(OCTET_LENGTH(imgIneA) = 0,null,imgIneA) ImgIneACIS,
                            IF(OCTET_LENGTH(imgIneR) = 0,null,imgIneR) ImgIneRCIS,
                            IF(OCTET_LENGTH(imgEstudios) = 0,null,imgEstudios) ImgEstudiosCIS,
                            IF(OCTET_LENGTH(imgCompNoProp) = 0,null,imgCompNoProp) ImgCompNoPropCIS,
                            IF(OCTET_LENGTH(imgCompProp) = 0,null,imgCompProp) ImgCompPropCIS,
                            IF(OCTET_LENGTH(imgCompDom) = 0,null,imgCompDom) ImgCompDomCIS,
                            IF(OCTET_LENGTH(imgCurp_A) = 0,null,imgCurp_A) ImgCurp_ACIS,
                            IF(OCTET_LENGTH(imgId_A) = 0,null,imgId_A) ImgId_ACIS,
                            IF(OCTET_LENGTH(imgId_R) = 0,null,imgId_R) ImgId_RCIS,
                            IF(OCTET_LENGTH(imgCompIng) = 0,null,imgCompIng) ImgCompIngCIS,
                            IF(OCTET_LENGTH(imgCartaNoDer) = 0,null,imgCartaNoDer) ImgCartaNoDerCIS,
                            IF(OCTET_LENGTH(imgFirma) = 0,null,imgFirma) ImgFirmaCIS
                        FROM prod_captacion.dvyr_cis 
                        WHERE id_unico in (select id_unico from prod_captacion.dvyr_cd where txtCURP = @Curp);

                       ";
            return await db.QueryFirstOrDefaultAsync<Beneficiario>(sql, new { Curp = curp });
        }
        public async Task<bool> InsertCaptacion(Beneficiario beneficiario)
        {
            var db = DbConnection();
            var sql = @"
                        INSERT INTO prod_predios.captacion
                        (id_unico, prueba, id_esquema, identificador)
                        VALUES(@IdUnico,@Prueba,@Esquema,@Identificador) 
                        ON DUPLICATE KEY UPDATE prueba = @Prueba,
                        id_esquema = @Esquema, identificador = @Identificador;
                        ";

            var result = await db.ExecuteAsync(sql, new
            {
                beneficiario.IdUnico,
                beneficiario.Prueba,
                beneficiario.Esquema,
                beneficiario.Identificador  
            });

            return result > 0;
        }
        public async Task<bool> InsertVisita(Beneficiario beneficiario)
        {
            var db = DbConnection();
            var sql = @"
                        INSERT INTO prod_predios.captacion_visita
                        (id_unico, nu_visita, fch_programacion, localizo, acordo, fch_acordada, lugar, motivo)
                        VALUES(@IdUnico,@NuVisita,@FchProgramacion,@Localizo,@Acordo,@FchAcordada,@Lugar,@Motivo);
                        ";

            var result = await db.ExecuteAsync(sql, new
            {
                beneficiario.IdUnico,
                beneficiario.NuVisita,
                beneficiario.FchProgramacion,
                beneficiario.Localizo,
                beneficiario.Acordo,
                beneficiario.FchAcordada,
                beneficiario.Lugar,
                beneficiario.Motivo
            });

            return result > 0;
        }
    }
}
