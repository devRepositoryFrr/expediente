using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConaviWeb.Commons;
using ConaviWeb.Data.Expedientes;
using ConaviWeb.Model.Expedientes;
using ConaviWeb.Services;
using static ConaviWeb.Models.AlertsViewModel;
using ConaviWeb.Model.Response;
using Microsoft.AspNetCore.Authorization;

namespace ConaviWeb.Controllers.Expedientes
{
    [Authorize]
    public class InventarioValidacionController : Controller
    {
        private readonly IExpedienteRepository _expedienteRepository;

        public InventarioValidacionController(IExpedienteRepository expedienteRepository)
        {
            _expedienteRepository = expedienteRepository;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }
            var catArea = await _expedienteRepository.GetPuestosListaValidacion();
            ViewData["AreaCatalogo"] = catArea;
            var catSoporte = await _expedienteRepository.GetTiposSoporte();
            ViewData["CatalogoSoporte"] = catSoporte;
            var catTipoDoc = await _expedienteRepository.GetTiposDocumentales();
            ViewData["CatTipoDoc"] = catTipoDoc;
            var cat = await _expedienteRepository.GetCodigosExp();
            ViewData["Catalogo"] = cat;
            //if (user.Id == 212 || user.Id == 323)
            ViewData["Modulos"] = user.Modules;
            int rol = (int)user.Rol;
            if (rol == 15)
                ViewData["btnShowValidacion"] = true;
            else
                ViewData["btnShowValidacion"] = false;
            if (TempData.ContainsKey("Alert"))
                ViewBag.Alert = TempData["Alert"].ToString();
            return View("../Expedientes/InventarioValidacion");
        }
        [HttpPost]
        public async Task<IActionResult> GetExpedientesControl([FromHeader] int slcPuesto)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return Unauthorized();
            }
            IEnumerable<Expediente> expedientes = await _expedienteRepository.GetExpedientesValidacionInventarioControl(slcPuesto);

            if (expedientes == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = expedientes });
        }
        [HttpPost]
        public async Task<IActionResult> VoBoExpedienteControl(int idExp)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }
            var success = await _expedienteRepository.VoBoExpedienteControl(idExp);
            if (!success)
            {
                var alertJson = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al enviar el VoBo del expediente");
                return Ok(alertJson);
            }
            var alert = AlertService.ShowAlert(Alerts.Success, "Se dió el VoBo al expediente con éxito");
            return Ok(alert);
        }
        [HttpPost]
        public async Task<IActionResult> RevalidacionExpedienteControl(int idExp, string observaciones)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }
            var success = await _expedienteRepository.RevalidacionExpedienteControl(idExp, observaciones);
            if (!success)
            {
                var alertJson = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al enviar a revalidación el expediente");
                return Ok(alertJson);
            }
            var alert = AlertService.ShowAlert(Alerts.Success, "Se envió a revalidación el expediente con exito");
            return Ok(alert);
        }
    }
}