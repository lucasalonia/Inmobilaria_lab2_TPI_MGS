using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;
using System.Security.Claims;
using System.Globalization;

namespace Inmobilaria_lab2_TPI_MGS.Controllers
{

    [Authorize(Policy = "UserOrAdmin")]

    public class PagosController : Controller
    {
        private readonly InquilinoService inquilinoService;
        private readonly ContratoService contratoService;
        private readonly PagoService pagoService;


        public PagosController(ContratoService contratoService, InquilinoService inquilinoService, PagoService pagoService)
        {
            this.contratoService = contratoService;
            this.inquilinoService = inquilinoService;
            this.pagoService = pagoService;
        }


        [HttpGet]
        public IActionResult Index(int pagina = 1)
        {
            int tamPagina = 10;
            var totalRegistros = contratoService.ListaContratosVigentes().Count;
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);
            var listaContratos = contratoService.ListaContratosVigentes(pagina, tamPagina);

            var listaViewModel = new List<ContratosViewModel>();
            foreach (var contrato in listaContratos)
            {
                var inquilino = inquilinoService.ObtenerInquilinoPorContrato(contrato.Id);

                listaViewModel.Add(new ContratosViewModel
                {
                    Inquilino = inquilino,
                    Contrato = contrato
                });

            }

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;

            return View(listaViewModel);
        }


        [HttpGet]
        [Route("[controller]/Lista/{id}")]
        public IActionResult Lista(int id)
        {
            try
            {
                var contrato = contratoService.ObtenerPorId(id);

                if (contrato == null)
                {
                    return NotFound();
                }

                var listaPagos = pagoService.ListarPorContrato(id);
                ViewBag.IdContrato = id;
                ViewBag.Moneda = contrato.Moneda;

                return View(listaPagos);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpGet]
        [Route("[controller]/Modificar/{id}")]
        public IActionResult Modificar(ulong id)
        {
            try
            {
                Pago pago = pagoService.ObtenerPorId(id);

                if (pago == null)
                {
                    return NotFound();
                }

                if (pago.Estado == "PAGADO"  && pago.ImportePagado != null)
                {
                    return NotFound();
                }

                ViewBag.PeriodoMes = DateTimeFormatInfo.CurrentInfo.GetMonthName(pago.PeriodoMes);
                ViewBag.PeriodoAnio = pago.PeriodoAnio;
                ViewBag.Estado = pago.Estado;
                ViewBag.Importe = pago.Importe;
                ViewBag.FechaVencimiento = pago.FechaVencimiento;

                return View();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpPost]
        public IActionResult Editar(Pago pago)
        {
            try
            {
               
                if (ModelState.IsValid)
                {
                    int? idUsuario = int.Parse(User.FindFirstValue("UserId"));
                    pago.FechaPago = DateTime.Now;
                    Pago pagoAux = pagoService.ObtenerPorId(pago.Id);

                    decimal resta =  (decimal)pagoAux.Importe - (decimal)pago.ImportePagado;

                    if (resta == 0)
                    {
                        pago.Estado = "PAGADO";
                        pago.Importe = resta;
                    }
                    else if (resta > 0)
                    {
                        pago.Estado = "PENDIENTE";
                        pago.Importe = resta;
                    }
                    else
                    {
                        throw new Exception("El importe pagado no puede ser mayor al importe del pago.");
                    }

                    pagoService.Modificar(pago, idUsuario);
                    return RedirectToAction("Lista", new { id = pagoAux.ContratoId });
                }
                else
                {
                    // Si el modelo no es válido, vuelve a la vista con los errores de validación
                    ViewBag.PeriodoMes = DateTimeFormatInfo.CurrentInfo.GetMonthName(pago.PeriodoMes);
                    ViewBag.PeriodoAnio = pago.PeriodoAnio;
                    ViewBag.Estado = pago.Estado;
                    ViewBag.FechaVencimiento = pago.FechaVencimiento;

                    return View("Modificar", pago);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}