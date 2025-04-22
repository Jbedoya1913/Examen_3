namespace Universidad.Matriculas.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        public string? DocumentoEstudiante { get; set; }
        public int NumeroCreditos { get; set; }
        public decimal ValorCredito { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        public string? Semestre { get; set; }
        public string? Asignaturas { get; set; }
    }
} 