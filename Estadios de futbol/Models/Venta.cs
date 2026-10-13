using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace Estadios_de_futbol.Models
{
    [Table("ventas")]
    public class Venta : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("eventoId")]
        public string EventoId { get; set; } = string.Empty;

        [Column("nombreCliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Column("cantidadBoletos")]
        public int CantidadBoletos { get; set; }

        [Column("precioUnitario")]
        public decimal PrecioUnitario { get; set; }

        [Column("fechaVenta")]
        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

      
        public decimal CalcularTotal()
        {
            return CantidadBoletos * PrecioUnitario;
        }
    }
}