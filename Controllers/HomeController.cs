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
        ViewBag.cantidadIntentos = 0;
        DB db = new DB();
        db.InsertarUsuario(nombre);
        int idUsuario = db.GetIdUsuario();
        HttpContext.Session.SetString("nombreUsuario", nombre);
        return RedirectToAction("CrearPartida", new { idUsuario = idUsuario });
    }
    
    public IActionResult CrearPartida(int idUsuario)
    {
        DB db = new DB();
        int idPartida = db.InsertarPartida(idUsuario);
        HttpContext.Session.SetString("idPartida", idPartida.ToString());
        HttpContext.Session.SetString("idSalaActual", db.GetPartida(idPartida).idSalaActual.ToString());

        return RedirectToAction("Sala", new { idSala = int.Parse(HttpContext.Session.GetString("idSalaActual")) });
    }

    [HttpPost]
    public IActionResult Intento(string respuesta)
    {
        int idPartida = int.Parse(HttpContext.Session.GetString("idPartida"));
        int idSalaActual = int.Parse(HttpContext.Session.GetString("idSalaActual"));
        DB db = new DB();
        bool esCorrecta = db.VerificarRespuesta(idSalaActual, respuesta);
        if (esCorrecta)
        {
            Partidas partida = db.GetPartida(idPartida);
            partida.idSalaActual++;
            db.ActualizarSala(partida.id, partida.idSalaActual);
            db.ActualizarEstado(partida.id, "En progreso");
            HttpContext.Session.SetString("idSalaActual", partida.idSalaActual.ToString());
        }
        else
        {
            ViewBag.cantidadIntentos ++;
            if (ViewBag.cantidadIntentos >= 5)
            {
                db.ActualizarEstado(idPartida, "Perdida");
                return RedirectToAction("Derrota");
            }
        }
        return RedirectToAction("Sala", new { idSala = int.Parse(HttpContext.Session.GetString("idSalaActual")) });
    }

    public IActionResult Sala(int idSala)
    {
        ViewBag.qa = idSala;
        if (idSala >= 1 && idSala <= 4)
        {
            string sala = "Sala" + idSala;
            return RedirectToAction(sala);
        }
        return RedirectToAction("Victoria");
    }

    public IActionResult Sala1()
    {
        return View();
    }

    public IActionResult Sala2()
    {
        return View();
    }

    public IActionResult Sala3()
    {
        return View();
    }

    public IActionResult Sala4()
    {
        return View();
    }

    public IActionResult Victoria()
    {
        return View();
    }
    
    public IActionResult Derrota()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
