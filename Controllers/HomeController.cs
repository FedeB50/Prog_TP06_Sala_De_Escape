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
        db.InsertarUsuario(nombre);
        int idUsuario = db.GetIdUsuario();
        HttpContext.Session.SetString("nombreUsuario", nombre);
        return RedirectToAction("CrearPartida", new { idUsuario = idUsuario });
    }
    
    public IActionResult CrearPartida(int idUsuario)
    {
        HttpContext.Session.SetString("intentosIncorrectos", "0");
        HttpContext.Session.SetString("pista", "");
        DB db = new DB();
        int idPartida = db.InsertarPartida(idUsuario);
        HttpContext.Session.SetString("idPartida", idPartida.ToString());
        HttpContext.Session.SetString("idSalaActual", db.GetPartida(idPartida).idSalaActual.ToString());
        return RedirectToAction("Sala");
    }

    [HttpPost]
    public IActionResult Intento(string respuesta)
    {
        int idPartida = int.Parse(HttpContext.Session.GetString("idPartida"));
        int idSalaActual = int.Parse(HttpContext.Session.GetString("idSalaActual"));
        DB db = new DB();
        if (db.VerificarRespuesta(idSalaActual, respuesta))
        {
            HttpContext.Session.SetString("intentosIncorrectos", "0");
            HttpContext.Session.SetString("pista", "");
            Partidas partida = db.GetPartida(idPartida);
            partida.idSalaActual++;
            db.ActualizarSala(partida.id, partida.idSalaActual);
            db.ActualizarEstado(partida.id, "En progreso");
            HttpContext.Session.SetString("idSalaActual", partida.idSalaActual.ToString());
        } else
        {
            int intentosIncorrectos = int.Parse(HttpContext.Session.GetString("intentosIncorrectos"));
            intentosIncorrectos++;
            HttpContext.Session.SetString("intentosIncorrectos", intentosIncorrectos.ToString());
            if (intentosIncorrectos >= 5)
            {
                db.ActualizarEstado(idPartida, "Perdida");
                return RedirectToAction("Derrota");
            }
        }
        return RedirectToAction("Sala");
    }

    public IActionResult Pista()
    {
        DB db = new DB();
        if (HttpContext.Session.GetString("pista") != db.GetPista(int.Parse(HttpContext.Session.GetString("idSalaActual"))))
        {
            HttpContext.Session.SetString("intentosIncorrectos", (int.Parse(HttpContext.Session.GetString("intentosIncorrectos")) + 2).ToString());
            HttpContext.Session.SetString("pista", db.GetPista(int.Parse(HttpContext.Session.GetString("idSalaActual"))));
        } 
        return RedirectToAction("Sala");
    } 

    public IActionResult Sala()
    {
        if (int.Parse(HttpContext.Session.GetString("idSalaActual")) >= 1 && int.Parse(HttpContext.Session.GetString("idSalaActual")) <= 4)
        {
            if (int.Parse(HttpContext.Session.GetString("intentosIncorrectos")) >= 5)
            {
                return RedirectToAction("Derrota");
            }
            if (int.Parse(HttpContext.Session.GetString("idSalaActual")) == 2)
            {
                ViewBag.texto = "△ ☐ ○";
            }
            ViewBag.cantidadIntentos = HttpContext.Session.GetString("intentosIncorrectos");
            ViewBag.pista = HttpContext.Session.GetString("pista");
            DB db = new DB();
            ViewBag.imagen = db.GetImagen(int.Parse(HttpContext.Session.GetString("idSalaActual")));
            return View();
        }
        return RedirectToAction("Victoria");
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
