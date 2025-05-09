using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConaviWeb.Model;
using ConaviWeb.Model.Diagnostico;

namespace ConaviWeb.Data.Diagnostico
{
    public interface IDiagnosticoRepository
    {
        Task<Beneficiario> GetBeneficiario (string curp);
        Task<Beneficiario> GetCaptacion (int idUnico);
        Task<bool> InsertCaptacion(Beneficiario beneficiario);
        Task<bool> InsertVisita(Beneficiario beneficiario);
    }
}
