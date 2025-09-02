using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{
    public class ContratosController : Controller
    {
        private readonly InquilinoService inquilinoService;

        public ContratosController(InquilinoService inquilinoService)
        {
            this.inquilinoService = inquilinoService;
        }

        // GET: ContratosController
        [Route("[controller]/Index")]
        public ActionResult Index()
        {
            try
            {
                var lista = inquilinoService.ListarInquilinosSinContrato();
                return View(lista);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}