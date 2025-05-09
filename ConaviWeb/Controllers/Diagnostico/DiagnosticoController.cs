using System.Collections.Generic;
using System.Threading.Tasks;
using ConaviWeb.Commons;
using ConaviWeb.Data.Diagnostico;
using ConaviWeb.Model.Diagnostico;
using ConaviWeb.Model.Levantamiento;
using ConaviWeb.Model.Response;
using ConaviWeb.Services;
using Microsoft.AspNetCore.Mvc;
using static ConaviWeb.Models.AlertsViewModel;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ConaviWeb.Controllers.Diagnostico
{
    public class DiagnosticoController : Controller
    {
        private readonly IDiagnosticoRepository _diagnosticoRepository;
        public DiagnosticoController(IDiagnosticoRepository diagnosticoRepository)
        {
            _diagnosticoRepository = diagnosticoRepository; 
        }
        public IActionResult Index()
        {
            if (TempData.ContainsKey("Alert"))
                ViewBag.Alert = TempData["Alert"].ToString();
            return View("../Diagnostico/Index");
        }
        [HttpGet]
        [Route("GetBenef/{curp?}")]
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
        [HttpGet]
        [Route("GetCaptacion/{idUnico?}")]
        public async Task<IActionResult> GetCaptacion(int idUnico)
        {
            var beneficiario = await _diagnosticoRepository.GetCaptacion(idUnico);

            if (beneficiario == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = beneficiario });
        }
        [HttpPost]
        public async Task<IActionResult> InsertDiagnostico(Beneficiario beneficiario)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            var success = false;
            success = await _diagnosticoRepository.InsertCaptacion(beneficiario);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar los datos");
                return RedirectToAction("Index");
            }
            TempData["Alert"] = AlertService.ShowAlert(Alerts.Success, "Se registró correctamente");
            return RedirectToAction("Index");
        }
    }
}
