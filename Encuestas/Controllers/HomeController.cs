using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Encuestas.Models;
using Encuestas.DAL;
using System.Collections.Generic;

namespace Encuestas.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Registro()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public JsonResult Login(string user, string pass)
    {
        string respuesta= string.Empty;
        bool flag = false;
        try
        {
            ConexionQuerys cq = new();

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            {
                flag = cq.UserLogin(user, pass);
                if (flag)
                {
                    HttpContext.Session.SetString("UsuarioGlobal", user);
                }
                respuesta = flag ? "Bienvenido" : "Hubo un problema al iniciar sesion";
            }
            else
            {
                respuesta = "Usuario o Contraseña vacios";
            }
        }
        catch (Exception)
        {
            throw;
        }

        return Json(new { success = flag, res = respuesta });
    }
    [HttpGet]
    public JsonResult Register(string user, string pass)
    {
        string respuesta= string.Empty;
        bool flag = false;
        try
        {
            ConexionQuerys cq = new ();
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            {
                flag = cq.UserRegister(user,BCrypt.Net.BCrypt.HashPassword(pass));
                respuesta = flag ? "Registro exitoso" : "Hubo un problema al registrar";
            }
            else
            {
                respuesta = "Usuario o Contraseña vacios";
            }
        }
        catch (Exception)
        {
            throw;
        }

        return Json(new { success = flag, res = respuesta }) ;
    }
    [HttpGet]
    public IActionResult Principal()
    {
        List<Encuesta> encuestas = new List<Encuesta>();
        try
        {
            ConexionQuerys cq = new();
            string usuarioGlobal = HttpContext.Session.GetString("UsuarioGlobal");
            encuestas = cq.ObtenerEncuestas(usuarioGlobal);
        }
        catch (Exception)
        {
            throw;
        }

        return View(encuestas);
    }
    [HttpPost]
    public IActionResult GuardarEncuesta(string descripcion, string nombre, List<CampoEncuesta> campos)
    {
        string usuarioGlobal = HttpContext.Session.GetString("UsuarioGlobal");
        bool resp = false;
        try
        {
            ConexionQuerys cq = new();
             resp = cq.GuardarEncuesta(descripcion, nombre, usuarioGlobal, campos);  
        }
        catch
        {

        }
        return Ok(new { Message = resp?"Encuesta creada exitosamente":"No se pudo generar la encuesta." });
    }

    [HttpGet]
    public JsonResult ObtenerTipoDatos(string user, string pass)
    {
        string respuesta = string.Empty;
        List<TipoDatos> datos = new List<TipoDatos>();
        try
        {
            ConexionQuerys cq = new();
            datos = cq.ObtenerTipoDatos();
        }
        catch (Exception)
        {
            throw;
        }

        return Json(new { success = true, res = datos });
    }

    
    [HttpGet]
    public JsonResult EliminiarEncuesta(int IdEncuesta)
    {
        
        try
        {
            ConexionQuerys cq = new();
            bool datos = cq.EliminiarEncuesta(IdEncuesta);
        }
        catch (Exception)
        {
            throw;
        }

        return Json(new { success = "Success" });
    }

    [HttpPost]
    public JsonResult GuardarEncuestaActualizada(string Nombre, string Descripcion, int IDEncuesta)
    {

        try
        {
            ConexionQuerys cq = new();
            bool datos = cq.GuardarEncuestaActualizada(Nombre, Descripcion, IDEncuesta);
        }
        catch (Exception)
        {
            throw;
        }

        return Json(new { success = "Success" });
    }

    
}
