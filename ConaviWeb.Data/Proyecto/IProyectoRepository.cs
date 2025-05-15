using ConaviWeb.Model;
using ConaviWeb.Model.Levantamiento;
using ConaviWeb.Model.Proyecto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConaviWeb.Data.Proyecto
{
    public interface IProyectoRepository
    {
        Task<bool> InsertEdificioProyecto(Edificio edificio);
        Task<IEnumerable<Edificio>> GetEdificios(int idPredio);
        Task<bool> DropEdificio(int id);
        Task<bool> InsertPropuestaConceptual(PropuestaConceptual propuesta);
        Task<bool> InsertFileEjecutivo(string idPredio, string nameFile, string filename, string extension);
        Task<Catalogo> GetFile(int idPredio, string nameFile);
        Task<bool> UpdateEstatusFile(int idPredio, string nameFile, int estatus, string observaciones, int userId);
        Task<bool> UpdateMedidasPresupuesto(Lineas ecos);
        Task<Lineas> GetMedidasPresupuesto(int idPredio);
        Task<bool> UpdateEstatusProyEje(int idPredio, int estatus, string observaciones, int userId);
        Task<bool> UpdateEstatusWFile(string idPredio, string estatus, string nameFile, string observacionesFile, int userId);
        Task<bool> UpdateEstatusSection(int idPredio, int estatus, string observaciones, string section, int userId);
    }
}
