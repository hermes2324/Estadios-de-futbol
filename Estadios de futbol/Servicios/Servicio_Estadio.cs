using Estadios_de_futbol.Base_de_datos;
using Estadios_de_futbol.Models;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estadios_de_futbol.Servicios
{
    public class Servicio_Estadio
    {
        public async Task<List<Estadio>> ObtenerEstadios()
        {
            try
            {
                var supabase = await Conexion.ObtenerClienteAsync();
                var estadios = await supabase.From<Estadio>().Get();
                return estadios.Models;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener estadios: {ex.Message}");
                return new List<Estadio>();
            }
        }
        public async Task<List<Evento>> ObtenerEventosPorEstadio(string estadioId)
        {
            var supabase = await Conexion.ObtenerClienteAsync();
            var result = await supabase.From<Evento>()
                .Where(e => e.EstadioId == estadioId)
                .Get();
            return result.Models;
        }
        public async Task<(bool exito, string mensaje)> GuardarEstadio(Estadio estadio)
        {
            try
            {
                var supabase = await Conexion.ObtenerClienteAsync();
                await supabase.From<Estadio>().Insert(estadio);
                return (true, "Estadio guardado.");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

    }
}
