using Estadios_de_futbol.Base_de_datos;
using Estadios_de_futbol.Models;
using Supabase;

namespace Estadios_de_futbol.Servicios
{
    public class Servicio_Evento
    {
        public async Task<(bool exito, string mensaje)> GuardarEvento(Evento evento)
        {
            try
            {
                var supabase = await Conexion.ObtenerClienteAsync();
                await supabase.From<Evento>().Insert(evento);
                return (true, "Evento registrado correctamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al guardar: {ex.Message}");
            }
        }
        public async Task ActualizarEvento(Evento evento)
        {
            var supabase = await Conexion.ObtenerClienteAsync();
            await supabase.From<Evento>().Update(evento);
        }
        public async Task<List<Evento>> ObtenerEventos()
        {
            var supabase = await Conexion.ObtenerClienteAsync();
            var result = await supabase.From<Evento>().Get();
            return result.Models;
        }
    }
}