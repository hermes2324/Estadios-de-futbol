using Estadios_de_futbol.Base_de_datos;
using Estadios_de_futbol.Models;

namespace Estadios_de_futbol.Servicios
{
    public class VentaService
    {
        public async Task<(bool exito, string mensaje)> GuardarVenta(Venta venta)
        {
            try
            {
                var supabase = await Conexion.ObtenerClienteAsync();
                await supabase.From<Venta>().Insert(venta);
                return (true, "Venta guardada correctamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al guardar venta: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje)> EliminarVentasPasadas()
        {
            try
            {
                var supabase = await Conexion.ObtenerClienteAsync();

                var eventosPasados = await supabase.From<Evento>()
                    .Where(e => e.FechaHora < DateTime.UtcNow)
                    .Get();

                if (!eventosPasados.Models.Any())
                    return (true, "No hay ventas pasadas que eliminar.");

                foreach (var ev in eventosPasados.Models)
                {
                    await supabase.From<Venta>()
                        .Where(v => v.EventoId == ev.Id)
                        .Delete();
                }

                return (true, $"Ventas de {eventosPasados.Models.Count} eventos eliminadas.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al eliminar ventas: {ex.Message}");
            }
        }
    }
}