using ConaviWeb.Commons;
using ConaviWeb.Model.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConaviWeb.Controllers.Expedientes
{
    public class BibliohemerograficoFtoController : Controller
    {
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetObject<UserResponse>("ComplexObject");
            if (user == null)
            {
                return RedirectToAction("Index", "LoginSedatu");
            }
            return View("../Expedientes/BibliohemerograficoFto");
        }
    }
}
