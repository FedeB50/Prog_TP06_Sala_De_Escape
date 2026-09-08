using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP06.Models;

namespace TP06.Controllers;

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
    
    [HttpPost]
    public IActionResult crearUsuario(string nombre)
    {
        DB db = new DB();
        int idUsuario = db.InsertarUsuario(nombre);
        HttpContext.Session.SetString("nombreUsuario", nombre);
        return RedirectToAction("CrearPartida", new { idUsuario });
    }
    
    public IActionResult CrearPartida(int idUsuario)
    {
        DB db = new DB();
        Partidas partida = db.InsertarPartida(idUsuario);
        HttpContext.Session.SetInt32("idPartida", partida.id);
        HttpContext.Session.SetInt32("idSalaActual", partida.idSalaActual);
        return RedirectToAction("Sala");
    }


    public IActionResult Sala()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
