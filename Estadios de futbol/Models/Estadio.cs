using Supabase.Postgrest.Attributes;  // ← falta este using
using Supabase.Postgrest.Models;

namespace Estadios_de_futbol.Models
{
    [Table("estadios")]  
    public class Estadio : BaseModel
    {
        [PrimaryKey("id", false)]       
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("latitud")]
        public double Latitud { get; set; }

        [Column("longitud")]
        public double Longitud { get; set; }

        [Column("fechaRegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public override string ToString() => Nombre;
    }
}