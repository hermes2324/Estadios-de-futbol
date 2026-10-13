using Estadios_de_futbol.Models;
using Estadios_de_futbol.Servicios;

namespace Estadios_de_futbol.Controllers
{
    public class EventoController
    {
        private readonly Servicio_Estadio _servicioEstadio;
        private readonly Servicio_Evento _servicioEvento;

        public EventoController()
        {
            _servicioEstadio = new Servicio_Estadio();
            _servicioEvento = new Servicio_Evento();
        }

        
        public async Task<List<Estadio>> ObtenerEstadios()
        {
            return await _servicioEstadio.ObtenerEstadios();
        }

       
        public async Task<(bool exito, string mensaje)> RegistrarEvento(Evento evento)
        {
            
            if (!evento.EsValidoParaVenta)
                return (false, "El evento debe registrarse con al menos 2 días de anticipación.");

            if (evento.BoletosTotales <= 0)
                return (false, "El número de boletos debe ser mayor a 0.");

            if (evento.Precio <= 0)
                return (false, "El precio debe ser mayor a 0.");

            return await _servicioEvento.GuardarEvento(evento);
        }
        public async Task<List<Evento>> ObtenerEventosPorEstadio(string estadioId)
        {
            return await _servicioEstadio.ObtenerEventosPorEstadio(estadioId);
        }
    }
}