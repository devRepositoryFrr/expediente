using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConaviWeb.Model;
using ConaviWeb.Model.Diagnostico;
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
                        id_unico,
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
                        txtReferencia referencia
                        FROM prod_captacion.dvyr_cd cd  
                        JOIN prod_ctls.cat_estado ce on ce.id_estado=cd.cmbEntidad 
                        JOIN prod_ctls.cat_municipio cm on cm.id_estado = cd.cmbEntidad and cm.id_municipio=cd.cmbMunicipio 
                        JOIN prod_ctls.cat_localidad cl on cl.id_estado = cd.cmbEntidad and cl.id_municipio=cd.cmbMunicipio and cl.id_localidad=cd.cmbLocalidad 
                        WHERE txtCURP = @Curp;

                       ";
            return await db.QueryFirstOrDefaultAsync<Beneficiario>(sql, new { Curp = curp });
        }
    }
}
