using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConaviWeb.Data.Levantamiento;
using ConaviWeb.Model;
using ConaviWeb.Model.Response;
using ConaviWeb.Model.Levantamiento;
using ConaviWeb.Commons;
using ConaviWeb.Services;
using static ConaviWeb.Models.AlertsViewModel;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace ConaviWeb.Controllers.Levantamiento
{
    [Authorize]
    public class LevantamientoController : Controller
    {
        private readonly ILevantamientoRepository _levantamientoRepository;
        private readonly IWebHostEnvironment _environment;
        public LevantamientoController(ILevantamientoRepository levantamientoRepository, IWebHostEnvironment environment)
        {
            _levantamientoRepository = levantamientoRepository;
            _environment = environment;
        }
        public IActionResult IndexAsync()
        {
            return View("../Levantamiento/ListaPredios");
        }
        public async Task<IActionResult> HomologacionFilesAsync(int id)
        {
            var seccion = await _levantamientoRepository.GetSeccionxPredio(id);
            var archivo = await _levantamientoRepository.GetArchivoxPredio(id);
            ViewData["Seccion"] = seccion;
            ViewData["Archivo"] = archivo;
            return View("../Levantamiento/ArchivosHomologacion");
        }
        public async Task<IActionResult> Predios()
        {
            var catEstados = await _levantamientoRepository.GetEstados();
            ViewBag.EstadoCatalogo = (new SelectList(catEstados, "Clave", "Descripcion"));
            return View("../Levantamiento/FormatoHomologacion");
        }
        [HttpGet]
        public IActionResult PropuestaConceptual(int? id)
        {
            //var catEstados = await _levantamientoRepository.GetEstados();
            //ViewBag.EstadoCatalogo = (new SelectList(catEstados, "Clave", "Descripcion"));
            return View("../Levantamiento/PropuestaConceptual");
        }
        public async Task<IActionResult> SeleccionArchivos(int id)
        {
            var seccion = await _levantamientoRepository.GetSeccion();
            var archivo = await _levantamientoRepository.GetArchivo();
            var proyecto = await _levantamientoRepository.GetFormatoLevantamiento(id);
            ViewData["Seccion"] = seccion;
            ViewData["Archivo"] = archivo;
            ViewData["NombreProyecto"] = proyecto.NombrePredio;
            return View("../Levantamiento/SeleccionArchivos");
        }
        [HttpPost]
        public async Task<IActionResult> GetMunicipios(string cveedo)
        {
            IEnumerable<Catalogo> municipios = new List<Catalogo>();
            municipios = await _levantamientoRepository.GetMunicipios(cveedo);
            return Json(new { data = municipios });
        }
        [HttpPost]
        public async Task<IActionResult> GetLocalidades(string cveedo, string cvemun)
        {
            IEnumerable<Catalogo> localidades = new List<Catalogo>();
            localidades = await _levantamientoRepository.GetLocalidades(cveedo, cvemun);
            return Json(new { data = localidades });
        }
        [HttpPost]
        public async Task<IActionResult> InsertFormatoLevantamiento(Predio predio)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            //predio.IdUser = user.Id;
            predio.IdUser = 21;

            var success = false;
            success = await _levantamientoRepository.InsertFormatoLevantamiento(predio);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar el formato");
                return RedirectToAction("Predios");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> GetFormatoLevantamiento([FromForm] int idPredio)
        {
            Predio predio = new();
            predio = await _levantamientoRepository.GetFormatoLevantamiento(idPredio);
            if (predio == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Id de Predio no existente");
                return Ok(alert);
            }
            return Ok(predio);
        }
        [HttpGet]
        public async Task<IActionResult> PrediosAdquisicion()
        {
            IEnumerable<Predio> predios = new List<Predio>();
            predios = await _levantamientoRepository.GetPrediosAdquisicion();

            if (predios == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = predios });
        }
        [HttpPost]
        public async Task<IActionResult> FullPrediosAdquisicion()
        {
            //var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            //var inventario = await _expedienteRepository.GetInventarioControl(user.Cargo);

            IEnumerable<Predio> predios = new List<Predio>();
            predios = await _levantamientoRepository.GetFullPrediosAdquisicion();

            if (predios == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Sin registros");
                return Ok(alert);
            }
            return Json(new { data = predios });
        }
        [HttpPost]
        public async Task<IActionResult> DropPredio(Predio predio)
        {
            var success = await _levantamientoRepository.DropPredio(predio.Id);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al eliminar el registro");
                return RedirectToAction("Index");
            }
            TempData["Alert"] = AlertService.ShowAlert(Alerts.Success, "Se eliminó el registro con éxito");
            return RedirectToAction("Index");
        }
        [HttpPost]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public IActionResult UpLoad()
        {
            var json = Request.Form["json"].ToString();
            var data = JObject.Parse(json);
            var idPredio = data.GetValue("idPredio").ToString();
            var idFile = data.GetValue("idFile").ToString();
            string alert;
            //string[] arrperiodo = periodo.Split(@"-");
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
                    if (!extension.Contains(".pdf") && !extension.Contains(".docx") && !extension.Contains(".xlsx"))
                    {
                        alert = AlertService.ShowAlert(Alerts.Danger, "Solo están permitidos archivos con extensión .pdf");
                        return Ok(new
                        {
                            success = false,
                            message = alert
                        });
                    }
                    string rootpath = Path.Combine(_environment.WebRootPath, "doc", "PrediosAdquisicion", idPredio, idFile);
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
                    _levantamientoRepository.InsertFilePredio(idPredio, idFile, filename, extension);
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
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public IActionResult UploadRepFoto()
        {
            var json = Request.Form["json"].ToString();
            var data = JObject.Parse(json);
            var idPredio = data.GetValue("idPredio").ToString();
            var idSedatu = data.GetValue("idSedatu").ToString();
            //var idFile = data.GetValue("idFile").ToString();
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
                    if (!extension.Contains(".pdf"))
                    {
                        alert = AlertService.ShowAlert(Alerts.Danger, "Solo están permitidos archivos con extensión .pdf");
                        return Ok(new
                        {
                            success = false,
                            message = alert
                        });
                    }
                    string rootpath = Path.Combine(_environment.WebRootPath, "doc", "PrediosAdquisicion", idPredio, "RepFoto");
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
                    _levantamientoRepository.InsertRepFoto(idPredio, filename);
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
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public IActionResult UploadCriteriosTecnicos()
        {
            var json = Request.Form["json"].ToString();
            var data = JObject.Parse(json);
            var idPredio = data.GetValue("idPredio").ToString();
            //var idSedatu = data.GetValue("idSedatu").ToString();
            //var idFile = data.GetValue("idFile").ToString();
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
                    if (!extension.Contains(".pdf"))
                    {
                        alert = AlertService.ShowAlert(Alerts.Danger, "Solo están permitidos archivos con extensión .pdf");
                        return Ok(new
                        {
                            success = false,
                            message = alert
                        });
                    }
                    string rootpath = Path.Combine(_environment.WebRootPath, "doc", "PrediosAdquisicion", idPredio, "CriteriosTecnicos");
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
                    _levantamientoRepository.InsertCriteriosTecnicos(idPredio, filename);
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
        public async Task<IActionResult> InsertPropuestaConceptual(PropuestaConceptual propuesta)
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            //predio.IdUser = user.Id;
            propuesta.IdUser = 212;

            var success = false;
            success = await _levantamientoRepository.InsertPropuestaConceptual(propuesta);
            if (!success)
            {
                TempData["Alert"] = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al registrar!");
                return Redirect("~/Levantamiento/PropuestaConceptual?id=" + propuesta.IdPredio);
            }
            return Redirect("~/Levantamiento/PropuestaConceptual?id=" + propuesta.IdPredio);
        }
        [HttpPost]
        public async Task<IActionResult> GetPropuestaConceptual([FromForm] int idPredio)
        {
            PropuestaConceptual propuesta = new();
            propuesta = await _levantamientoRepository.GetPropuestaConceptual(idPredio);
            if (propuesta == null)
            {
                var alert = AlertService.ShowAlert(Alerts.Danger, "Id de Predio no encontrado");
                return Ok(alert);
            }
            return Ok(propuesta);
        }
        [HttpPost]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public IActionResult UploadPropuesta()
        {
            var json = Request.Form["json"].ToString();
            var data = JObject.Parse(json);
            var idPredio = data.GetValue("idPredio").ToString();
            var observacion = data.GetValue("observacion").ToString();
            var estatus = data.GetValue("estatus").ToString();
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
                    if (!extension.Contains(".pdf"))
                    {
                        alert = AlertService.ShowAlert(Alerts.Danger, "Solo están permitidos archivos con extensión .pdf");
                        return Ok(new
                        {
                            success = false,
                            message = alert
                        });
                    }
                    string rootpath = Path.Combine(_environment.WebRootPath, "doc", "PrediosAdquisicion", idPredio, "PropuestaConceptual");
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
                    _levantamientoRepository.InsertPropuestaFile(idPredio, filename, estatus, observacion);
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
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public IActionResult UploadConciliacionFile()
        {
            var json = Request.Form["json"].ToString();
            var data = JObject.Parse(json);
            var idPredio = data.GetValue("idPredio").ToString();
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
                    if (!extension.Contains(".pdf"))
                    {
                        alert = AlertService.ShowAlert(Alerts.Danger, "Solo están permitidos archivos con extensión .pdf");
                        return Ok(new
                        {
                            success = false,
                            message = alert
                        });
                    }
                    string rootpath = Path.Combine(_environment.WebRootPath, "doc", "PrediosAdquisicion", idPredio, "Conciliacion");
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
                    _levantamientoRepository.InsertConciliacionFile(idPredio, filename);
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
        public IActionResult UploadTotalPropuesta()
        {
            var json = Request.Form["json"].ToString();
            var data = JObject.Parse(json);
            var idPredio = data.GetValue("idPredio").ToString();
            var viviendas = data.GetValue("viviendas").ToString();
            var cajones = data.GetValue("cajones").ToString();
            var niveles = data.GetValue("niveles").ToString();
            string alert;
            //var files = Request.Form.Files;
            try
            {
                _levantamientoRepository.InsertFinalPropuesta(idPredio, viviendas, cajones, niveles);

                alert = AlertService.ShowAlert(Alerts.Success, "Se guardaron los cambios");

                return Ok(new
                {
                    success = true,
                    message = alert
                });
            }
            catch (Exception ex)
            {
                alert = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al guardar los cambios");
                return Ok(new
                {
                    success = false,
                    message = alert
                });
            }
        }
        private void DicCreate(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetFile(int idPredio, int idFile)
        {
            Catalogo file = new Catalogo();
            file = await _levantamientoRepository.GetFile(idPredio, idFile);
            return Json(new { data = file });
        }
        [HttpPost]
        public async Task<IActionResult> GetRepFoto(int idPredio)
        {
            Catalogo file = new Catalogo();
            file = await _levantamientoRepository.GetRepFoto(idPredio);
            return Json(new { data = file });
        }
        [HttpPost]
        public async Task<IActionResult> GetCriteriosTecnicos(int idPredio)
        {
            Catalogo file = new Catalogo();
            file = await _levantamientoRepository.GetCriteriosTecnicos(idPredio);
            return Json(new { data = file });
        }
        //[HttpPost]
        //public async Task<IActionResult> GetPropuestaFile(int idPredio)
        //{
        //    PropuestaConceptual file = new PropuestaConceptual();
        //    file = await _levantamientoRepository.GetPropuestaFile(idPredio);
        //    return Json(new { data = file });
        //}
        [HttpPost]
        public async Task<IActionResult> ValidarArchivo(string idPredio, int idFile)
        {
            var success = await _levantamientoRepository.ValidarArchivo(idPredio, idFile);
            string alert;
            if (!success)
            {
                alert = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al validar el archivo");
                return Ok(new
                {
                    success = false,
                    message = alert
                });
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RechazarArchivo(int idPredio, int idFile)
        {
            var success = await _levantamientoRepository.RechazarArchivo(idPredio, idFile);
            string alert;
            if (!success)
            {
                alert = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al rechazar el archivo");
                return Ok(new
                {
                    success = false,
                    message = alert
                });
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RequerirArchivo(string idPredio, int idFile, int estatus)
        {
            string alert;
            var success = await _levantamientoRepository.RequerirArchivo(idPredio, idFile, estatus);
            if (!success)
            {
                alert = AlertService.ShowAlert(Alerts.Danger, "Ocurrio un error al realizar la operación");
                return Ok(new
                {
                    success = false,
                    message = alert
                });
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> GetEstatusReqFile(int idPredio, int idFile)
        {
            Catalogo file = new Catalogo();
            file = await _levantamientoRepository.GetEstatusReqFile(idPredio, idFile);
            return Json(new { data = file });
        }
        [HttpPost]
        public async Task<IActionResult> GetEdosPrediosList(int ejFiscal)
        {
            IEnumerable<Catalogo> estados = new List<Catalogo>();
            estados = await _levantamientoRepository.GetEdosPrediosList(ejFiscal);
            return Json(new { data = estados });
        }
    }
}
