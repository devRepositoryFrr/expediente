using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConaviWeb.Model;
using ConaviWeb.Model.Levantamiento;

namespace ConaviWeb.Data.Levantamiento
{
    public interface ILevantamientoRepository
    {
        Task<IEnumerable<Catalogo>> GetEstados();
        Task<IEnumerable<Catalogo>> GetMunicipios(string estado);
        Task<IEnumerable<Catalogo>> GetLocalidades(string estado, string cvemun);
        Task<IEnumerable<Catalogo>> GetSeccion();
        Task<IEnumerable<Catalogo>> GetSeccionxPredio(int id);
        Task<IEnumerable<Archivo>> GetArchivo();
        Task<IEnumerable<Archivo>> GetArchivoxPredio(int id);
        Task<bool> InsertFormatoLevantamiento(Predio predio);
        Task<bool> InsertPropuestaConceptual(PropuestaConceptual propuesta);
        Task<bool> InsertPropuestaFile(string idPredio, string filename, string estatus, string observacion);
        Task<PropuestaConceptual> GetPropuestaConceptual(int id);
        Task<Predio> GetFormatoLevantamiento(int id);
        Task<IEnumerable<Predio>> GetPrediosAdquisicion();
        Task<IEnumerable<Predio>> GetFullPrediosAdquisicion();
        Task<bool> DropPredio(int id);
        Task<bool> InsertFilePredio(string idPredio, string idFile, string filename, string extension);
        Task<bool> InsertRepFoto(string idPredio, string filename);
        Task<bool> InsertCriteriosTecnicos(string idPredio, string filename);
        Task<bool> InsertConciliacionFile(string idPredio, string filename);
        Task<bool> InsertFinalPropuesta(string idPredio, string viviendas, string cajones, string niveles);
        Task<Catalogo> GetFile(int idPredio, int idFile);
        Task<Catalogo> GetRepFoto(int idPredio);
        Task<Catalogo> GetCriteriosTecnicos(int idPredio);
        //Task<PropuestaConceptual> GetPropuestaFile(int idPredio);
        Task<bool> ValidarArchivo(string idPredio, int idFile);
        Task<bool> RechazarArchivo(int idPredio, int idFile);
        Task<bool> RequerirArchivo(string idPredio, int idFile, int estatus);
        Task<Catalogo> GetEstatusReqFile(int idPredio, int idFile);
        Task<IEnumerable<Catalogo>> GetEdosPrediosList(int ejFiscal);
    }
}
