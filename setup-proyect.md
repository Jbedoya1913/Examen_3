


# 🎓 Servicio de Matrículas - Universidad (con SQL Server y .NET 8)

Este proyecto permite registrar las matrículas de los estudiantes. Usa autenticación con Bearer Token y guarda los datos en **SQL Server**. Aquí está todo el paso a paso para crearlo desde cero con **Cursor**.



🗂️ 2. Crear las carpetas del proyecto

Crea estas carpetas dentro del proyecto:
	•	Models
	•	Data
	•	Controllers

⸻

📄 3. Crear el modelo Matricula

Dentro de Models/Matricula.cs:

public class Matricula
{
    public int Id { get; set; }
    public string DocumentoEstudiante { get; set; }
    public int NumeroCreditos { get; set; }
    public decimal ValorCredito { get; set; }
    public decimal Total { get; set; }
    public DateTime Fecha { get; set; }
    public string Semestre { get; set; }
    public string Asignaturas { get; set; }
}



⸻

👤 4. Crear el modelo Estudiante

En Models/Estudiante.cs:

public class Estudiante
{
    public int Id { get; set; }
    public string Documento { get; set; }
    public string Usuario { get; set; }
    public string Clave { get; set; }
}



⸻

🧠 5. Crear el DbContext

En Data/AppDbContext.cs:

using Microsoft.EntityFrameworkCore;
using Universidad.Matriculas.Models;

namespace Universidad.Matriculas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
    }
}



⸻

⚙️ 6. Configurar la conexión SQL Server

En appsettings.json, cambia la sección de conexión:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=UniversidadDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

Asegúrate de que tu SQL Server esté corriendo.

⸻

🔗 7. Configurar EF Core y la conexión

En Program.cs:

using Microsoft.EntityFrameworkCore;
using Universidad.Matriculas.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



⸻

🔒 8. Configurar JWT

A continuación de la conexión en Program.cs, agrega:

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var key = Encoding.UTF8.GetBytes("CLAVE_SECRETA_SEGURA_123456");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

Y en la sección final:

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();



⸻

🪪 9. Crear LoginController

En Controllers/LoginController.cs:

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Universidad.Matriculas.Data;
using Universidad.Matriculas.Models;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoginController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Estudiantes
            .FirstOrDefault(u => u.Usuario == request.Usuario && u.Clave == request.Clave);

        if (user == null) return Unauthorized();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("CLAVE_SECRETA_SEGURA_123456");

        var token = new JwtSecurityToken(
            claims: new[] { new Claim(ClaimTypes.Name, user.Documento) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return Ok(new { token = tokenHandler.WriteToken(token) });
    }
}

public class LoginRequest
{
    public string Usuario { get; set; }
    public string Clave { get; set; }
}



⸻

📥 10. Crear el CRUD de Matrículas

En Controllers/MatriculaController.cs:

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universidad.Matriculas.Data;
using Universidad.Matriculas.Models;

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



⸻

🧪 11. Crear la base de datos

Desde la terminal:

dotnet ef migrations add Init
dotnet ef database update




