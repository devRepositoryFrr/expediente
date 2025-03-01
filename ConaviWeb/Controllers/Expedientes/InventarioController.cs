﻿using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ConaviWeb.Controllers.Expedientes
{
    [Authorize]
    public class InventarioController : Controller
    {
        private readonly IExpedienteRepository _expedienteRepository;

        public InventarioController(IExpedienteRepository expedienteRepository)
        {
            _expedienteRepository = expedienteRepository;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            var idUserPuesto = await _expedienteRepository.GetIdUserPuesto(user.Cargo);
            var inventario = await _expedienteRepository.GetInventarioControl(user.Cargo);
            var cat = await _expedienteRepository.GetCodigosExp();
            ViewData["Catalogo"] = cat;
            //var catPuesto = await _expedienteRepository.GetPuestosLista();
            //ViewBag.AreaCatalogo = (new SelectList(catPuesto, "Id", "Clave", idUserPuesto));
            ViewBag.NombreResponsable = inventario != null ? inventario.NombreResponsableAT : "";
            ViewBag.IdInv = inventario != null ? inventario.Id : 0;
            //ViewBag.FechaElab = inventario != null ? inventario.FechaElaboracion.ToString("dd/MM/yyyy") : "";
            ViewBag.FechaElab = inventario != null ? inventario.FechaElaboracion : "";
            ViewBag.FechaTra = inventario != null ? inventario.FechaTransferencia : "";
            int rol = (int)user.Rol;
            if (rol == 15)
            {
                var catPuesto = await _expedienteRepository.GetPuestosLista();
                ViewBag.AreaCatalogo = (new SelectList(catPuesto, "IdPuesto", "Puesto", idUserPuesto));
                //ViewData["btnShowValidacion"] = true;
            }
            else
            {
                var catPuesto = await _expedienteRepository.GetPuestoUser(idUserPuesto);
                ViewBag.AreaCatalogo = new SelectList(catPuesto, "IdPuesto", "Puesto", idUserPuesto);
                //ViewData["btnShowValidacion"] = false;
            }
            if (TempData.ContainsKey("Alert"))
                ViewBag.Alert = TempData["Alert"].ToString();
            return View("../Expedientes/Inventario");
        }
        [HttpPost]
        public async Task<IActionResult> InsertInventarioTPrimaria(Inventario inventario)
        {
            //var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            //inventario.IdUser = user.Id;
            var success = await _expedienteRepository.InsertInventarioTP(inventario);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar el inventario");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        //[HttpPost]
        //public async Task<IActionResult> InsertExpedienteInventarioTPrimaria(Expediente expediente)
        //{
        //    var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
        //    expediente.IdUser = user.Id;

        //    var success = await _expedienteRepository.InsertExpedienteInventarioTP(expediente);
        //    if (!success)
        //    {
        //        TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar el expediente");
        //        return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public async Task<IActionResult> ExpedientesTP()
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            var inventario = await _expedienteRepository.GetInventarioControl(user.Cargo);

            IEnumerable<Expediente> expedientes = new List<Expediente>();
            expedientes = await _expedienteRepository.GetExpedientesInventarioTP(user.Id, inventario!=null ? inventario.Id: 0);
            if (expedientes == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = expedientes });
        }
        [HttpPost]
        public async Task<IActionResult> GetExpedientesTPByIdInv([FromForm] int id)
        {
            IEnumerable<Expediente> expedientes = new List<Expediente>();
            expedientes = await _expedienteRepository.GetExpedientesTPByIdInv(id);
            if (expedientes == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = expedientes });
        }
        //[HttpPost]
        //public async Task<IActionResult> GetExpedienteTP([FromForm] int id)
        //{
        //    Expediente expediente = new();
        //    var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
        //    expediente = await _expedienteRepository.GetExpedienteTP(id);
            
        //    if (expediente == null)
        //    {
        //        var alert = AlertService.ShowAlert(Alerts.Danger, "Id de expediente no encontrado");
        //        return Ok(alert);
        //    }
        //    expediente.UserName = user.Name;
        //    return Ok(expediente);
        //}
        [HttpPost]
        public async Task<IActionResult> GetCaratulaExpedienteTP([FromForm] int id, int legajo)
        {
            Caratula caratula = new();
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            caratula = await _expedienteRepository.GetCaratulaExpedienteTP(id,legajo);
            
            if (caratula == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Id de expediente no encontrado");
                return Ok(alert);
            }
            caratula.UserName = user.Name;
            return Ok(caratula);
        }
        //[HttpPost]
        //public async Task<IActionResult> DropExpediente(Expediente expediente)
        //{
        //    var success = await _expedienteRepository.DropExpediente(expediente.Id);
        //    if (!success)
        //    {
        //        TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al eliminar el expediente");
        //        return RedirectToAction("Index");
        //    }
        //    TempData["Alert"] = AlertService.ShowAlert(Alerts.Success, "Se eliminó el expediente con exito");
        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public async Task<IActionResult> SendValExpediente(Expediente expediente)
        //{
        //    var success = await _expedienteRepository.SendValExpediente(expediente.Id);
        //    if (!success)
        //    {
        //        TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al enviar el expediente");
        //        return RedirectToAction("Index");
        //    }
        //    TempData["Alert"] = AlertService.ShowAlert(Alerts.Success, "Se envió el expediente a revisión con exito");
        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public async Task<IActionResult> VoBoExpediente(Expediente expediente)
        //{
        //    var success = await _expedienteRepository.VoBoExpediente(expediente.Id);
        //    if (!success)
        //    {
        //        TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al enviar el VoBo del expediente");
        //        return RedirectToAction("Index");
        //    }
        //    TempData["Alert"] = AlertService.ShowAlert(Alerts.Success, "Se dió el VoBo al expediente con exito");
        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public async Task<IActionResult> RevalidacionExpediente(Expediente expediente)
        //{
        //    var success = await _expedienteRepository.RevalidacionExpediente(expediente.Id);
        //    if (!success)
        //    {
        //        TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al enviar a revalidación el expediente");
        //        return RedirectToAction("Index");
        //    }
        //    TempData["Alert"] = AlertService.ShowAlert(Alerts.Success, "Se envió a revalidación el expediente con exito");
        //    return RedirectToAction("Index");
        //}
    }
}