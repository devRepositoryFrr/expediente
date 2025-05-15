using ConaviWeb.Model;
using ConaviWeb.Model.Levantamiento;
using ConaviWeb.Model.Proyecto;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ConaviWeb.Data.Proyecto
{
    public class ProyectoRepository : IProyectoRepository
    {
        private readonly MySQLConfiguration _connectionString;
        public ProyectoRepository(MySQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }
        protected MySqlConnection DbConnection()
        {
            return new MySqlConnection(_connectionString.ExpConnectionString);
        }
        public async Task<bool> InsertEdificioProyecto(Edificio edificio)
        {
            var db = DbConnection();
            var sql = @"
                        INSERT INTO prod_predios.edificios
                        (`etapa`, `manzana`, `nomenclatura`,`no_viviendas`,`idPredio`,`idUser`)
                        VALUES (@etapa,@manzana,@nomenclatura,@viviendas,@idpredio,@iduser);
                        ";

            var result = await db.ExecuteAsync(sql, new
            {
                etapa = edificio.Etapa,
                manzana = edificio.Manzana,
                nomenclatura = edificio.Nomenclatura,
                viviendas = edificio.Viviendas,
                idpredio = edificio.IdPredio,
                iduser = edificio.IdUser
            });

            return result > 0;
        }
        public async Task<IEnumerable<Edificio>> GetEdificios(int idPredio)
        {
            var db = DbConnection();
            var sql = @"
                        SELECT ed.id Id, ed.etapa Etapa, ed.manzana Manzana, ed.nomenclatura Nomenclatura, ed.no_viviendas Viviendas
                        FROM prod_predios.edificios ed
                        WHERE ed.idPredio = @IdPredio;
                       ";
            return await db.QueryAsync<Edificio>(sql, new { IdPredio = idPredio });
        }
        public async Task<bool> DropEdificio(int id)
        {
            var db = DbConnection();
            var sql = @"
                        delete from prod_predios.edificios where id = @Id;";
            var result = await db.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
        public async Task<bool> InsertPropuestaConceptual(PropuestaConceptual propuesta)
        {
            var db = DbConnection();
            var sql = @"
                        INSERT INTO prod_predios.propuesta_conceptual
                        (superficie_terreno, superficie_desplante, superficie_donacion, superficie_equipamiento, superficie_construccion, superficie_restriccion, superficie_recreativa, superficie_areas_verdes, superficie_circulaciones_vehiculares, superficie_circulaciones_peatonales, cajones_estacionamiento, superficie_cajones_estacionamiento, total_viviendas, viviendas_unifamiliar, superficie_viviendas_unifamiliar, niveles_viviendas_unifamililar, viviendas_multifamiliar, superficie_viviendas_multifamiliar, niveles_viviendas_multifamiliar, viviendas_renta, superficie_viviendas_renta, niveles_vivienda_renta, estatus, id_predio, id_usuario)
                        VALUES (@SuperficieTerreno, @SuperficieDesplante, @SuperficieDonacion, @SuperficieEquipamiento, @SuperficieConstruccion, @SuperficieRestriccion, @SuperficieRecreativa, @SuperficieAreasverdes, @SuperficieCirculacionesVehiculares, @SuperficieCirculacionesPeatonales, @CajonesEstacionamiento, @SuperficieCajonesEstacionamiento, @TotalViviendas, @ViviendasUnifamiliar, @SuperficieViviendasUnifamiliar, @NivelesViviendasUnifamiliar, @ViviendasMultifamiliar, @SuperficieViviendasMultifamiliar, @NivelesViviendasMultifamiliar, @ViviendasRenta, @SuperficieViviendasRenta, @NivelesViviendaRenta, 1, @IdPredio, @IdUsuario)
                        ON DUPLICATE KEY UPDATE superficie_terreno = @SuperficieTerreno, superficie_desplante = @SuperficieDesplante, superficie_donacion = @SuperficieDonacion, superficie_equipamiento = @SuperficieEquipamiento, superficie_construccion = @SuperficieConstruccion, superficie_restriccion = @SuperficieRestriccion, superficie_recreativa = @SuperficieRecreativa, superficie_areas_verdes = @SuperficieAreasverdes, superficie_circulaciones_vehiculares = @SuperficieCirculacionesVehiculares, superficie_circulaciones_peatonales = @SuperficieCirculacionesPeatonales, cajones_estacionamiento = @CajonesEstacionamiento, superficie_cajones_estacionamiento = @SuperficieCajonesEstacionamiento, total_viviendas = @TotalViviendas, viviendas_unifamiliar = @ViviendasUnifamiliar, superficie_viviendas_unifamiliar = @SuperficieViviendasUnifamiliar, niveles_viviendas_unifamililar = @NivelesViviendasUnifamiliar, viviendas_multifamiliar = @ViviendasMultifamiliar, superficie_viviendas_multifamiliar = @SuperficieViviendasMultifamiliar, niveles_viviendas_multifamiliar = @NivelesViviendasMultifamiliar, viviendas_renta = @ViviendasRenta, superficie_viviendas_renta = @SuperficieViviendasRenta, niveles_vivienda_renta = @NivelesViviendaRenta, id_predio = @IdPredio, id_usuario = 212, fecha_update = NOW();
                        ";

            var result = await db.ExecuteAsync(sql, new
            {
                SuperficieTerreno = propuesta.SuperficieTerreno,
                SuperficieDesplante = propuesta.SuperficieDesplante,
                SuperficieDonacion = propuesta.SuperficieDonacion,
                SuperficieEquipamiento = propuesta.SuperficieEquipamiento,
                SuperficieConstruccion = propuesta.SuperficieConstruccion,
                SuperficieRestriccion = propuesta.SuperficieRestriccion,
                SuperficieRecreativa = propuesta.SuperficieRecreativa,
                SuperficieAreasverdes = propuesta.SuperficieAreasVerdes,
                SuperficieCirculacionesVehiculares = propuesta.SuperficieCirculacionesVehiculares,
                SuperficieCirculacionesPeatonales = propuesta.SuperficieCirculacionesPeatonales,
                CajonesEstacionamiento = propuesta.CajonesEstacionamiento,
                SuperficieCajonesEstacionamiento = propuesta.SuperficieCajonesEstacionamiento,
                TotalViviendas = propuesta.TotalViviendas,
                ViviendasUnifamiliar = propuesta.ViviendasUnifamiliar,
                SuperficieViviendasUnifamiliar = propuesta.SuperficieViviendasUnifamiliar,
                NivelesViviendasUnifamiliar = propuesta.NivelesViviendasUnifamiliar,
                ViviendasMultifamiliar = propuesta.ViviendasMultifamiliar,
                SuperficieViviendasMultifamiliar = propuesta.SuperficieViviendasMultifamiliar,
                NivelesViviendasMultifamiliar = propuesta.NivelesViviendasMultifamiliar,
                ViviendasRenta = propuesta.ViviendasRenta,
                SuperficieViviendasRenta = propuesta.SuperficieViviendasRenta,
                NivelesViviendaRenta = propuesta.NivelesViviendasRenta,
                Estatus = propuesta.Estatus,
                IdPredio = propuesta.IdPredio,
                IdUsuario = propuesta.IdUser
            });

            return result > 0;
        }
        public async Task<bool> InsertFileEjecutivo(string idPredio, string nameFile, string filename, string extension)
        {
            var db = DbConnection();
            var sql = @"
                        INSERT INTO prod_predios.files_ejecutivo(nombre_archivo, extension, nameFile, idPredio)
                        VALUES(@NomArchivo, @Ext, @NameFile, @IdPredio);
                       ";
            var result = await db.ExecuteAsync(sql, new
            {
                NomArchivo = filename,
                Ext = extension,
                NameFile = nameFile,
                IdPredio = idPredio
            });
            return result > 0;
        }
        public async Task<Catalogo> GetFile(int idPredio, string nameFile)
        {
            var db = DbConnection();
            var sql = @"
                        SELECT idPredio Id, nombre_archivo Descripcion, estatus Clave, ifnull(observaciones, observaciones_file) Ico
                        FROM prod_predios.files_ejecutivo
                        WHERE idPredio = @IdPredio and nameFile = @NameFile
                        ORDER BY files_ejecutivo.id DESC LIMIT 1;
                       ";
            return await db.QueryFirstOrDefaultAsync<Catalogo>(sql, new { IdPredio = idPredio, NameFile = nameFile });
        }
        public async Task<bool> UpdateEstatusFile(int idPredio, string nameFile, int estatus, string observaciones, int userId)
        {
            var db = DbConnection();
            var sql = @"
                        UPDATE prod_predios.files_ejecutivo set estatus = @Estatus, fecha_cambio_estatus = now(), observaciones = @Observaciones, idUser = @UserId
                        WHERE idPredio = @IdPredio AND nameFile = @NameFile
                        ORDER BY id DESC
                        LIMIT 1;
                       ";
            var result = await db.ExecuteAsync(sql, new
            {
                NameFile = nameFile,
                Estatus = estatus,
                Observaciones = observaciones,
                UserId = userId,
                IdPredio = idPredio
            });
            return result > 0;
        }
        public async Task<bool> UpdateEstatusSection(int idPredio, int estatus, string observaciones, string section, int userId)
        {
            var sql = "";
            switch (section)
            {
                case "1a":
                    sql = @"
                        UPDATE prod_predios.propuesta_conceptual set estatus = @Estatus, fecha_update = now(), observaciones = @Observaciones, id_usuario = @UserId
                        WHERE id_predio = @IdPredio;
                       ";
                    break;
                case "4a":
                    sql = @"
                        UPDATE prod_predios.presupuesto set estatus_section_4a = @Estatus, fecha_cambio_estatus = now(), observaciones_section_4a = @Observaciones, idUser = @UserId
                        WHERE idPredio = @IdPredio;
                       ";
                    break;
                case "4b":
                    sql = @"
                        UPDATE prod_predios.presupuesto set estatus_section_4b = @Estatus, fecha_cambio_estatus = now(), observaciones_section_4b = @Observaciones, idUser = @UserId
                        WHERE idPredio = @IdPredio;
                       ";
                    break;
                case "5a":
                    sql = @"
                        UPDATE prod_predios.presupuesto set estatus_section_5a = @Estatus, fecha_cambio_estatus = now(), observaciones_section_5a = @Observaciones, idUser = @UserId
                        WHERE idPredio = @IdPredio;
                       ";
                    break;
                case "6a":
                    sql = @"
                        UPDATE prod_predios.presupuesto set estatus_section_6a = @Estatus, fecha_cambio_estatus = now(), observaciones_section_6a = @Observaciones, idUser = @UserId
                        WHERE idPredio = @IdPredio;
                       ";
                    break;
                case "6b":
                    sql = @"
                        UPDATE prod_predios.presupuesto set estatus_section_6b = @Estatus, fecha_cambio_estatus = now(), observaciones_section_6b = @Observaciones, idUser = @UserId
                        WHERE idPredio = @IdPredio;
                       ";
                    break;
                default:
                    break;
            }
            var db = DbConnection();
            //var sql = @"
            //            UPDATE prod_predios.presupuesto set  = @Estatus, fecha_cambio_estatus = now(), observaciones = @Observaciones, idUser = @UserId
            //            WHERE idPredio = @IdPredio;
            //           ";
            var result = await db.ExecuteAsync(sql, new
            {
                Estatus = estatus,
                Observaciones = observaciones,
                UserId = userId,
                IdPredio = idPredio
            });
            return result > 0;
        }
        public async Task<bool> UpdateMedidasPresupuesto(Lineas ecos)
        {
            var db = DbConnection();
            var sql = @"
                        INSERT INTO prod_predios.presupuesto(
                            `cmb_ecotecnias`,
                            `txt_acristalamiento`,
                            `txt_mef_techo`,
                            `txt_mef_muro`,
                            `txt_reflec_techo`,
                            `txt_reflec_muro`,
                            `cmb_solar`,
                            `txt_solar`,
                            `cmb_inhodoro`,
                            `cmb_regadera`,
                            `cmb_llaves_bano`,
                            `cmb_llaves_cocina`,
                            `cmb_pluvial`,
                            `txt_lamparas`,
                            `txt_leds`,
                            `txt_bombeo`,
                            `cmb_tipocombustible`,
                            `txt_tipocombustible`,
                            `cmb_calentador`,
                            `cmb_calentador_solar`,
                            `cmb_estufa`,
                            `txt_arbol`,
                            `txt_reforz_agua`,
                            `txt_obrpoz`,
                            `txt_reforzdrenaje`,
                            `cmb_trataprovagua`,
                            `cmb_stmaguares`,
                            `txt_refene`,
                            `txt_lamsolac`,
                            `txt_bomsol`,
                            `cmb_sisfot`,
                            `cmb_aerogenerador`,
                            `cmb_motrices`,
                            `cmb_obra_preventivas`,
                            `txt_obra_preventiva`,
                            `volumen_demolicion`,
                            `estatus_section_4a`,
                            `estatus_section_4b`,
                            `estatus_section_5a`,
                            `estatus_section_6a`,
                            `estatus_section_6b`,
                            `idPredio`,
                            `idUser`)
                        VALUES(@cmb_ecotecnias,
                            @txt_acristalamiento,
                            @txt_mef_techo,
                            @txt_mef_muro,
                            @txt_reflec_techo,
                            @txt_reflec_muro,
                            @cmb_solar,
                            @txt_solar,
                            @cmb_inhodoro,
                            @cmb_regadera,
                            @cmb_llaves_bano,
                            @cmb_llaves_cocina,
                            @cmb_pluvial,
                            @txt_lamparas,
                            @txt_leds,
                            @txt_bombeo,
                            @cmb_tipocombustible,
                            @txt_tipocombustible,
                            @cmb_calentador,
                            @cmb_calentador_solar,
                            @cmb_estufa,
                            @txt_arbol,
                            @txt_reforz_agua,
                            @txt_obrpoz,
                            @txt_reforzdrenaje,
                            @cmb_trataprovagua,
                            @cmb_stmaguares,
                            @txt_refene,
                            @txt_lamsolac,
                            @txt_bomsol,
                            @cmb_sisfot,
                            @cmb_aerogenerador,
                            @cmb_motrices,
                            @cmb_obra_preventivas,
                            @txt_obra_preventiva,
                            @volumen_demolicion,
                            1,
                            1,
                            1,
                            1,
                            1,
                            @idPredio,
                            @idUser)
                        ON DUPLICATE KEY UPDATE
                            `cmb_ecotecnias` = @cmb_ecotecnias,
                            `txt_acristalamiento` = @txt_acristalamiento,
                            `txt_mef_techo` = @txt_mef_techo,
                            `txt_mef_muro` = @txt_mef_muro,
                            `txt_reflec_techo` = @txt_reflec_techo,
                            `txt_reflec_muro` = @txt_reflec_muro,
                            `cmb_solar` = @cmb_solar,
                            `txt_solar` = @txt_solar,
                            `cmb_inhodoro` = @cmb_inhodoro,
                            `cmb_regadera` = @cmb_regadera,
                            `cmb_llaves_bano` = @cmb_llaves_bano,
                            `cmb_llaves_cocina` = @cmb_llaves_cocina,
                            `cmb_pluvial` = @cmb_pluvial,
                            `txt_lamparas` = @txt_lamparas,
                            `txt_leds` = @txt_leds,
                            `txt_bombeo` = @txt_bombeo,
                            `cmb_tipocombustible` = @cmb_tipocombustible,
                            `txt_tipocombustible` = @txt_tipocombustible,
                            `cmb_calentador` = @cmb_calentador,
                            `cmb_calentador_solar` = @cmb_calentador_solar,
                            `cmb_estufa` = @cmb_estufa,
                            `txt_arbol` = @txt_arbol,
                            `txt_reforz_agua` = @txt_reforz_agua,
                            `txt_obrpoz` = @txt_obrpoz,
                            `txt_reforzdrenaje` = @txt_reforzdrenaje,
                            `cmb_trataprovagua` = @cmb_trataprovagua,
                            `cmb_stmaguares` = @cmb_stmaguares,
                            `txt_refene` = @txt_refene,
                            `txt_lamsolac` = @txt_lamsolac,
                            `txt_bomsol` = @txt_bomsol,
                            `cmb_sisfot` = @cmb_sisfot,
                            `cmb_aerogenerador` = @cmb_aerogenerador,
                            `cmb_motrices` = @cmb_motrices,
                            `cmb_obra_preventivas` = @cmb_obra_preventivas,
                            `txt_obra_preventiva` = @txt_obra_preventiva,
                            `volumen_demolicion` = @volumen_demolicion,
                            `fecha_update` = now(),
                            `idUser` = @idUser;
                       ";
            var result = await db.ExecuteAsync(sql, new
            {
                cmb_ecotecnias = ecos.cmb_ecotecnias,
                txt_acristalamiento = ecos.txt_acristalamiento,
                txt_mef_techo = ecos.txt_mef_techo,
                txt_mef_muro = ecos.txt_mef_muro,
                txt_reflec_techo = ecos.txt_reflec_techo,
                txt_reflec_muro = ecos.txt_reflec_muro,
                cmb_solar = ecos.cmb_solar,
                txt_solar = ecos.txt_solar,
                cmb_inhodoro = ecos.cmb_inhodoro,
                cmb_regadera = ecos.cmb_regadera,
                cmb_llaves_bano = ecos.cmb_llaves_bano,
                cmb_llaves_cocina = ecos.cmb_llaves_cocina,
                cmb_pluvial = ecos.cmb_pluvial,
                txt_lamparas = ecos.txt_lamparas,
                txt_leds = ecos.txt_leds,
                txt_bombeo = ecos.txt_bombeo,
                cmb_tipocombustible = ecos.cmb_tipocombustible,
                txt_tipocombustible = ecos.txt_tipocombustible,
                cmb_calentador = ecos.cmb_calentador,
                cmb_calentador_solar = ecos.cmb_calentador_solar,
                cmb_estufa = ecos.cmb_estufa,
                txt_arbol = ecos.txt_arbol,
                txt_reforz_agua = ecos.txt_reforz_agua,
                txt_obrpoz = ecos.txt_obrpoz,
                txt_reforzdrenaje = ecos.txt_reforzdrenaje,
                cmb_trataprovagua = ecos.cmb_trataprovagua,
                cmb_stmaguares = ecos.cmb_stmaguares,
                txt_refene = ecos.txt_refene,
                txt_lamsolac = ecos.txt_lamsolac,
                txt_bomsol = ecos.txt_bomsol,
                cmb_sisfot = ecos.cmb_sisfot,
                cmb_aerogenerador = ecos.cmb_aerogenerador,
                cmb_motrices = ecos.cmb_motrices,
                cmb_obra_preventivas = ecos.cmb_obra_preventivas,
                txt_obra_preventiva = ecos.txt_obra_preventiva,
                volumen_demolicion = ecos.volumen_demolicion,
                estatus = ecos.estatus,
                idPredio = ecos.idPredio,
                idUser = ecos.idUser
            });
            return result > 0;
        }
        public async Task<Lineas> GetMedidasPresupuesto(int idPredio)
        {
            var db = DbConnection();
            var sql = @"
                        SELECT f.id, p.cmb_ecotecnias, p.txt_acristalamiento, p.txt_mef_techo, p.txt_mef_muro, p.txt_reflec_techo, p.txt_reflec_muro, p.cmb_solar, p.txt_solar, p.cmb_inhodoro, p.cmb_regadera, p.cmb_llaves_bano, p.cmb_llaves_cocina, p.cmb_pluvial, p.txt_lamparas, p.txt_leds, p.txt_bombeo, p.cmb_tipocombustible, p.txt_tipocombustible, p.cmb_calentador, p.cmb_calentador_solar, p.cmb_estufa, p.txt_arbol, p.txt_reforz_agua, p.txt_obrpoz, p.txt_reforzdrenaje, p.cmb_trataprovagua, p.cmb_stmaguares, p.txt_refene, p.txt_lamsolac, p.txt_bomsol, p.cmb_sisfot, p.cmb_aerogenerador, p.cmb_motrices, p.cmb_obra_preventivas, p.txt_obra_preventiva, p.volumen_demolicion, p.estatus_section_4a, p.observaciones_section_4a txt_estatus_section_4a, p.estatus_section_4b, p.observaciones_section_4b txt_estatus_section_4b, p.estatus_section_5a, p.observaciones_section_5a txt_estatus_section_5a, p.estatus_section_6a, p.observaciones_section_6a txt_estatus_section_6a, p.estatus_section_6b, p.observaciones_section_6b txt_estatus_section_6b, p.idPredio
                        FROM prod_predios.formato_predio f
                        LEFT JOIN prod_predios.presupuesto p ON p.idPredio = f.id
                        WHERE f.id = @IdPredio;
                       ";
            return await db.QueryFirstOrDefaultAsync<Lineas>(sql, new { IdPredio = idPredio });
        }
        public async Task<bool> UpdateEstatusProyEje(int idPredio, int estatus, string observaciones, int userId)
        {
            var db = DbConnection();
            var sql = @"
                        UPDATE prod_predios.presupuesto set estatus = @Estatus, fecha_cambio_estatus = now(), txt_estatus = @Observaciones, idUser = @UserId
                        WHERE idPredio = @IdPredio;
                       ";
            var result = await db.ExecuteAsync(sql, new
            {
                Estatus = estatus,
                Observaciones = observaciones,
                UserId = userId,
                IdPredio = idPredio
            });
            return result > 0;
        }
        public async Task<bool> UpdateEstatusWFile(string idPredio, string estatus, string nameFile, string observacionesFile, int userId)
        {
            var db = DbConnection();
            var sql = @"
                        UPDATE prod_predios.files_ejecutivo set estatus = @Estatus, fecha_cambio_estatus = now(), observaciones_file = @Observaciones, idUser = @UserId
                        WHERE idPredio = @IdPredio and nameFile = @NameFile
                        ORDER BY id DESC
                        LIMIT 1;
                       ";
            var result = await db.ExecuteAsync(sql, new
            {
                Estatus = estatus,
                NameFile = nameFile,
                Observaciones = observacionesFile,
                UserId = userId,
                IdPredio = idPredio
            });
            return result > 0;
        }
    }
}