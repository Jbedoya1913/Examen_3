using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universidad.Matriculas.Data;
using Universidad.Matriculas.Models;

namespace Universidad.Matriculas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MatriculaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatriculaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Matricula m)
        {
            m.Total = m.NumeroCreditos * m.ValorCredito;
            _context.Matriculas.Add(m);
            _context.SaveChanges();
            return Ok(m);
        }

        [HttpGet("{documento}/{semestre}")]
        public IActionResult Consultar(string documento, string semestre)
        {
            var m = _context.Matriculas
                .FirstOrDefault(x => x.DocumentoEstudiante == documento && x.Semestre == semestre);

            return m == null ? NotFound() : Ok(m);
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Matricula datos)
        {
            var m = _context.Matriculas.Find(id);
            if (m == null) return NotFound();

            m.NumeroCreditos = datos.NumeroCreditos;
            m.ValorCredito = datos.ValorCredito;
            m.Total = datos.NumeroCreditos * datos.ValorCredito;
            m.Fecha = datos.Fecha;
            m.Semestre = datos.Semestre;
            m.Asignaturas = datos.Asignaturas;

            _context.SaveChanges();
            return Ok(m);
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var m = _context.Matriculas.Find(id);
            if (m == null) return NotFound();

            _context.Matriculas.Remove(m);
            _context.SaveChanges();
            return NoContent();
        }
    }
} 