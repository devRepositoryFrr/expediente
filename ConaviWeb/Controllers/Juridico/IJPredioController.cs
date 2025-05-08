using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConaviWeb.Controllers.Juridico
{
    public class IJPredioController : Controller
    {
        public IActionResult Index()
        {
            return View("../Juridico/IJPredio");
        }

        public IActionResult listaPredios(string idPredio) {
            var items = new[] {
                    new {id = "25000012" , status = "exp"},
                    new {id = "25000013" , status = "des"},
                    new {id = "25000014" , status = "adq"}

                };
            var result = items.Where(x => x.id == idPredio).FirstOrDefault();
            return Ok(result);
        }
    }
}
