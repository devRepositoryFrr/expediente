using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConaviWeb.Commons;
using ConaviWeb.Data.Expedientes;
using ConaviWeb.Model.Expedientes;
using ConaviWeb.Model.Response;
using ConaviWeb.Services;
using static ConaviWeb.Models.AlertsViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConaviWeb.Controllers.Expedientes
{
    public class NoExpedientableController : Controller
    {
        private readonly IExpedienteRepository _expedienteRepository;
        public NoExpedientableController(IExpedienteRepository expedienteRepository)
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
            //var idUserPuesto = await _expedienteRepository.GetIdUserPuesto(user.Cargo);
            var inventario = await _expedienteRepository.GetInventarioControl(user.IdCargo);
            ViewBag.IdInv = inventario != null ? inventario.Id : 0;
            var cat = await _expedienteRepository.GetTiposSoporte();
            ViewData["Catalogo"] = cat;
            var catTipoDoc = await _expedienteRepository.GetTiposDocumentales();
            ViewData["CatTipoDoc"] = catTipoDoc;
            var catClave = await _expedienteRepository.GetCodigosExp();
            ViewData["ClaveInterna"] = catClave;
            ViewBag.NombreR = inventario != null ? inventario.NombreResponsableAT : "";
            ViewBag.FechaElab = inventario != null ? inventario.FechaElaboracion : "";
            ViewBag.FechaTrans = inventario != null ? inventario.FechaTransferencia : "";
            ViewData["Modulos"] = user.Modules;
            int rol = (int)user.Rol;
            if (rol == 15)
            {
                var catPuesto = await _expedienteRepository.GetPuestosLista();
                ViewBag.AreaCatalogo = (new SelectList(catPuesto, "IdPuesto", "Puesto", user.IdCargo));
            }
            else
            {
                var catPuesto = await _expedienteRepository.GetPuestoUser(user.IdCargo);
                ViewBag.AreaCatalogo = new SelectList(catPuesto, "IdPuesto", "Puesto", user.IdCargo);
            }
            if (TempData.ContainsKey("Alert"))
                ViewBag.Alert = TempData["Alert"].ToString();
            return View("../Expedientes/NoExpedientable");
        }
        [HttpPost]
        public async Task<IActionResult> InsertInventarioNoExpedientable(Inventario inventario)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }

            var success = await _expedienteRepository.InsertInventarioNoExpedientable(inventario);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar el inventario");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ExpedientesNoExpedientables()
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }
            var inventario = await _expedienteRepository.GetInventarioControl(user.IdCargo);

            IEnumerable<Expediente> expedientes = new List<Expediente>();
            expedientes = await _expedienteRepository.GetExpedientesNoExpedientables(user.Id, inventario!=null ? inventario.Id : 0);
            if (expedientes == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = expedientes });
        }
        [HttpPost]
        public async Task<IActionResult> GetExpedientesNoExpedientablesByIdInv([FromForm] int id)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }
            IEnumerable<Expediente> expedientes = new List<Expediente>();
            expedientes = await _expedienteRepository.GetExpedientesNoExpedientablesByIdInv(id);
            if (expedientes == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = expedientes });
        }
        [HttpPost]
        public async Task<IActionResult> GetNoExpedientable([FromForm] int id)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }
            Expediente expediente = new();
            expediente = await _expedienteRepository.GetNoExpedientable(id);
            if (expediente == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Id de expediente no encontrado");
                return Ok(alert);
            }
            expediente.UserName = user.Name;
            return Ok(expediente);
        }
        [HttpPost]
        public async Task<IActionResult> GetCaratulaNoExpedientable([FromForm] int id, int legajo)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }
            Caratula caratula = await _expedienteRepository.GetCaratulaNoExpedientable(id, legajo);
            if (caratula == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Id de expediente no encontrado");
                return Ok(alert);
            }
            caratula.UserName = user.Name;
            return Ok(caratula);
        }
        [HttpPost]
        public async Task<IActionResult> DropExpediente(ExpedienteNoExpedientable expediente)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }
            var success = await _expedienteRepository.DropExpedienteNoExpedientable(expediente.Id);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al eliminar el expediente");
                return RedirectToAction("Index");
            }
            TempData["Alert"] = AlertService.ShowAlert(Alerts.Success, "Se eliminó el expediente con éxito");
            return RedirectToAction("Index");
        }
    }
}
