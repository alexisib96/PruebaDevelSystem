using Microsoft.AspNetCore.Mvc;
using Encuestas.Models;
using Encuestas.DAL;

namespace Encuestas.Controllers
{
    public class EncuestaController : Controller
    {
        public IActionResult Index(string guid)
        {
            Encuesta encuestas = new Encuesta();
            ConexionQuerys con = new ConexionQuerys();
            encuestas = con.ObtenerEnlace(guid);

            return View(encuestas);
        }

        public IActionResult Resultados(string guid)
        {
            Encuesta encuestas = new Encuesta();
            ConexionQuerys con = new ConexionQuerys();
            encuestas = con.VerRespuestaEnlace(guid);

            return View(encuestas);
        }

        [HttpPost]
        public JsonResult GuardarRespuesta(List<CampoEncuesta> encuesta)
        {
            ConexionQuerys con = new ConexionQuerys();
            bool respuesta = con.GuardarRespuesta(encuesta);
            return Json(encuesta);
        }

     }
}
