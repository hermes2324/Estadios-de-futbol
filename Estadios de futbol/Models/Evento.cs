using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace Estadios_de_futbol.Models
{
    [Table("eventos")]
    public class Evento : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("nombreJuego")]
        public string NombreJuego { get; set; } = string.Empty;

        [Column("boletosDisponibles")]
        public int BoletosDisponibles { get; set; }

        [Column("boletosTotales")]
        public int BoletosTotales { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("fechaHora")]
        public DateTime FechaHora { get; set; }
        [Column("estadioNombre")]          
        public string EstadioNombre { get; set; } = string.Empty;

        [Column("estadioId")]
        public string EstadioId { get; set; } = string.Empty;

        [Column("dineroRecolectado")]
        public decimal DineroRecolectado { get; set; }
        [Column("estadioLatitud")]         
        public double EstadioLatitud { get; set; }

        [Column("estadioLongitud")]        
        public double EstadioLongitud { get; set; }

        [JsonIgnore]
        public bool EsValidoParaVenta =>
            FechaHora.Date > DateTime.Today.AddDays(2);

        public override string ToString() => NombreJuego;
    }
}