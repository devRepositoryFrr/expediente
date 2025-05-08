using System.Collections.Generic;
using System.Threading.Tasks;
using ConaviWeb.Data.Diagnostico;
using ConaviWeb.Services;
using Microsoft.AspNetCore.Mvc;
using static ConaviWeb.Models.AlertsViewModel;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ConaviWeb.Controllers.Diagnostico
{
    public class DHomeController : Controller
    {
        private readonly IDiagnosticoRepository _diagnosticoRepository;
        public DHomeController(IDiagnosticoRepository diagnosticoRepository)
        {
            _diagnosticoRepository = diagnosticoRepository; 
        }
        public IActionResult Index()
        {
            return View("../Diagnostico/Index");
        }
        [HttpGet]
        [Route("Download/{curp?}")]
        public async Task<IActionResult> GetBenef(string curp)
        {
            var beneficiario = await _diagnosticoRepository.GetBeneficiario(curp);
            
            if (beneficiario == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = beneficiario });
        }
    }
}
