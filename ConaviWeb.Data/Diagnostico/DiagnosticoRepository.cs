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
        public async Task<Beneficiario> GetBeneficiario(string curp)
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
                            prueba,id_esquema esquema,identificador
                            FROM prod_captacion.dvyr_cd cd  
                            JOIN prod_ctls.cat_estado ce on ce.id_estado=cd.cmbEntidad 
                            JOIN prod_ctls.cat_municipio cm on cm.id_estado = cd.cmbEntidad and cm.id_municipio=cd.cmbMunicipio 
                            JOIN prod_ctls.cat_localidad cl on cl.id_estado = cd.cmbEntidad and cl.id_municipio=cd.cmbMunicipio and cl.id_localidad=cd.cmbLocalidad
                            JOIN prod_predios.captacion c on c.id_unico = cd.id_unico
                         WHERE txtCURP = @Curp;

                       ";
            return await db.QueryFirstOrDefaultAsync<Beneficiario>(sql, new { Curp = curp });
        }
        public async Task<Beneficiario> GetCaptacion(int idUnico)
        {
            var db = DbConnection();
            var sql = @"
                        select id_unico,prueba,id_esquema,identificador,fch_creacion 
                        from prod_predios.captacion
                        WHERE id_unico = @IdUnico;

                       ";
            return await db.QueryFirstOrDefaultAsync<Beneficiario>(sql, new { IdUnico = idUnico });
        }
        public async Task<bool> InsertCaptacion(Beneficiario beneficiario)
        {
            var db = DbConnection();
            var sql = @"
                        INSERT INTO prod_predios.captacion
                        (id_unico, prueba, id_esquema, identificador)
                        VALUES(@IdUnico,@Prueba,@Esquema,@Identificador);
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
                        (id_captacion, nu_visita, fch_programacion, localizo, acordo, fch_acordada, lugar, motivo)
                        VALUES(@Id,@NuVisita,@FchProgramacion,@Localizo,@Acordo,@FchAcordada,@Lugar,@Motivo);
                        ";

            var result = await db.ExecuteAsync(sql, new
            {
                beneficiario.Id,
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
