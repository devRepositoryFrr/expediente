using ConaviWeb.Commons;
using ConaviWeb.Data.Levantamiento;
using ConaviWeb.Data.Proyecto;
using ConaviWeb.Model;
using ConaviWeb.Model.Levantamiento;
using ConaviWeb.Model.Proyecto;
using ConaviWeb.Model.Response;
using ConaviWeb.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using static ConaviWeb.Models.AlertsViewModel;

namespace ConaviWeb.Controllers.Proyecto
{
    public class ProyectoController : Controller
    {
        private readonly IProyectoRepository _proyectoRepository;
        private readonly IWebHostEnvironment _environment;
        public ProyectoController(IProyectoRepository proyectoRepository, IWebHostEnvironment environment)
        {
            _proyectoRepository = proyectoRepository;
            _environment = environment;
        }
        // GET: ProyectoController
        public IActionResult Index()
        {
            ViewBag.Alert = TempData["Alert"];
            return View("../Proyecto/Ejecutivo");
        }

        [HttpPost]
        public async Task<IActionResult> AddBuildingAsync([FromForm] int etapa, int manzana, string nomenclatura, int viviendasEdificio, int idPredio)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");

            var edificio = new Edificio()
            {
                Etapa = etapa,
                Manzana = manzana,
                Nomenclatura = nomenclatura,
                Viviendas = viviendasEdificio,
                IdPredio = idPredio,
                IdUser = user.Id
            };

            var success = false;
            success = await _proyectoRepository.InsertEdificioProyecto(edificio);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar el edificio");
                return Redirect("../Proyecto?id=" + idPredio);
            }
            return Redirect("../Proyecto?id=" + idPredio);
        }
        [HttpPost]
        public async Task<IActionResult> GetEdificiosAsync(int idPredio)
        {
            IEnumerable<Edificio> edificios = new List<Edificio>();
            edificios = await _proyectoRepository.GetEdificios(idPredio);

            if (edificios == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = edificios });
        }
        [HttpPost]
        public async Task<IActionResult> DropEdificioAsync([FromForm] int id, int idPredio)
        {
            var success = await _proyectoRepository.DropEdificio(id);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al eliminar el registro");
                return Redirect("../Proyecto?id=" + idPredio);
            }
            TempData["Alert"] = AlertService.ShowAlert(Alerts.Success, "Se eliminó el registro con éxito");
            return Redirect("../Proyecto?id=" + idPredio);
        }
        [HttpPost]
        public async Task<IActionResult> InsertPropuestaConceptualAsync(PropuestaConceptual propuesta)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            propuesta.IdUser = user.Id;

            var success = false;
            success = await _proyectoRepository.InsertPropuestaConceptual(propuesta);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar!");
                return Redirect("~/Proyecto?id=" + propuesta.IdPredio);
            }
            TempData["Alert"] = AlertService.ShowAlert(Alerts.Success, "Registro realizado!");
            return Redirect("~/Proyecto?id=" + propuesta.IdPredio);
        }
        private void DicCreate(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        [HttpPost]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public IActionResult UploadArchivoEjecutivoAsync()
        {
            var json = Request.Form["json"].ToString();
            var data = JObject.Parse(json);
            var idPredio = data.GetValue("idPredio").ToString();
            var nameFile = data.GetValue("nameFile").ToString();
            string alert;
            var files = Request.Form.Files;
            try
            {
                foreach (var file in files)
                {
                    string[] arrpath = file.FileName.Split(@"/");
                    string dirpath = "";//Directory where the file is located (including one or two levels of directories)
                    string fulldir = Path.Combine(arrpath[0]);
                    string filename = arrpath[arrpath.Length - 1].ToString();//The file name
                    var extension = Path.GetExtension(filename);
                    if (!extension.Contains(".pdf") && !extension.Contains(".docx") && !extension.Contains(".xlsx") && !extension.Contains(".rar"))
                    {
                        alert = AlertService.ShowAlert(Alerts.Danger, "Solo están permitidos archivos con extensiones pdf, docx, xlsx y rar");
                        return Ok(new
                        {
                            success = false,
                            message = alert
                        });
                    }
                    string rootpath = Path.Combine(_environment.WebRootPath, "doc", "PrediosAdquisicion", idPredio, nameFile);
                    for (int i = 1; i < arrpath.Length; i++)
                    {
                        if (i == arrpath.Length - 1)
                        {
                            break;
                        }
                        dirpath += arrpath[i] + @"/";
                    }
                    dirpath = Path.Combine(rootpath, dirpath);
                    DicCreate(dirpath);//Create the directory if it does not exist

                    string filepath = Path.Combine(rootpath, fulldir);
                    using (var addFile = new FileStream(filepath, FileMode.OpenOrCreate))
                    {
                        if (file != null)
                        {
                            file.CopyTo(addFile);
                        }
                        else
                        {
                            Request.Body.CopyTo(addFile);
                        }
                        addFile.Close();
                    }
                    _proyectoRepository.InsertFileEjecutivo(idPredio, nameFile, filename, extension);
                }
                alert = AlertService.ShowAlert(Alerts.Success, "Se cargaron " + files.Count + " archivos");

                return Ok(new
                {
                    success = true,
                    message = alert
                });
            }
            catch (Exception ex)
            {
                alert = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al cargar los archivos");
                return Ok(new
                {
                    success = false,
                    message = alert
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetFileAsync(int idPredio, string nameFile)
        {
            Catalogo file = new Catalogo();
            file = await _proyectoRepository.GetFile(idPredio, nameFile);
            return Json(new { data = file });
        }
        [HttpPost]
        public async Task<IActionResult> SendStatusAsync(int idPredio, string nameFile, int estatus, string observaciones)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            //predio.IdUser = user.Id;
            //IdUser = 212;

            var success = false;
            success = await _proyectoRepository.UpdateEstatusFile(idPredio, nameFile, estatus, observaciones, user.Id);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar!");
                return Redirect("~/Proyecto?id=" + idPredio);
            }
            TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Actualización realizada!");
            return Redirect("~/Proyecto?id=" + idPredio);
        }
        [HttpPost]
        public async Task<IActionResult> SendStatusSectionAsync(int idPredio, string nameFile, int estatus, string observaciones, string section)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            //predio.IdUser = user.Id;
            //IdUser = 212;

            var success = false;
            success = await _proyectoRepository.UpdateEstatusSection(idPredio, estatus, observaciones, section, user.Id);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar!");
                return Redirect("~/Proyecto?id=" + idPredio);
            }
            TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Actualización realizada!");
            return Redirect("~/Proyecto?id=" + idPredio);
        }
        [HttpPost]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public IActionResult SendEstatusWFileAsync()
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            var idPredio = Request.Form["idPredio"].ToString();
            //var data = JObject.Parse(json);
            var nameFile = Request.Form["nameFile"].ToString();
            var estatus = Request.Form["estatus"].ToString();
            string alert;
            var files = Request.Form.Files;
            try
            {
                foreach (var file in files)
                {
                    string[] arrpath = file.FileName.Split(@"/");
                    string dirpath = "";//Directory where the file is located (including one or two levels of directories)
                    string fulldir = "Obs" + Path.Combine(arrpath[0]);
                    string filename = "Obs" + arrpath[arrpath.Length - 1].ToString();//The file name
                    var extension = Path.GetExtension(filename);
                    if (!extension.Contains(".pdf"))
                    {
                        alert = AlertService.ShowAlert(Alerts.Danger, "Solo están permitidos archivos con extensiones pdf");
                        return Ok(new
                        {
                            success = false,
                            message = alert
                        });
                    }
                    string rootpath = Path.Combine(_environment.WebRootPath, "doc", "PrediosAdquisicion", idPredio, nameFile);
                    for (int i = 1; i < arrpath.Length; i++)
                    {
                        if (i == arrpath.Length - 1)
                        {
                            break;
                        }
                        dirpath += arrpath[i] + @"/";
                    }
                    dirpath = Path.Combine(rootpath, dirpath);
                    DicCreate(dirpath);//Create the directory if it does not exist

                    string filepath = Path.Combine(rootpath, fulldir);
                    using (var addFile = new FileStream(filepath, FileMode.OpenOrCreate))
                    {
                        if (file != null)
                        {
                            file.CopyTo(addFile);
                        }
                        else
                        {
                            Request.Body.CopyTo(addFile);
                        }
                        addFile.Close();
                    }
                    _proyectoRepository.UpdateEstatusWFile(idPredio, estatus, nameFile, filename, user.Id);
                }
                alert = AlertService.ShowAlert(Alerts.Success, "Se cargaron " + files.Count + " archivos");

                return Ok(new
                {
                    success = true,
                    message = alert
                });
            }
            catch (Exception ex)
            {
                alert = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al cargar el archivo");
                return Ok(new
                {
                    success = false,
                    message = alert
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendMedidasPresupuestoAsync(string json, int idPredio)
        {
            string alert;
            Lineas ecos = JsonConvert.DeserializeObject<Lineas>(json);
            ecos.idPredio = idPredio;
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            ecos.idUser = user.Id;
            var success = false;
            success = await _proyectoRepository.UpdateMedidasPresupuesto(ecos);
            if (!success)
            {
                alert = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar!");
                return Ok(new
                {
                    success = true,
                    message = alert
                });
            }
            alert = AlertService.ShowAlert(Alerts.Success, "Actualización realizada!");
            return Ok(new
            {
                success = true,
                message = alert
            });
        }
        [HttpPost]
        public async Task<IActionResult> GetMedidasPresupuestoAsync(int idPredio)
        {
            Lineas ecos = new Lineas();
            ecos = await _proyectoRepository.GetMedidasPresupuesto(idPredio);
            return Json(new { data = ecos });
        }
        [HttpPost]
        public async Task<IActionResult> SendStatusProyEjeAsync(int idPredio, int estatus, string observaciones)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            //predio.IdUser = user.Id;
            //IdUser = 212;

            var success = false;
            success = await _proyectoRepository.UpdateEstatusProyEje(idPredio, estatus, observaciones, user.Id);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar!");
                return Redirect("~/Proyecto?id=" + idPredio);
            }
            TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Actualización realizada!");
            return Redirect("~/Proyecto?id=" + idPredio);
        }
    }
}
