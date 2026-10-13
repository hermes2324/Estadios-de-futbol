using Estadios_de_futbol.Models;
using Estadios_de_futbol.Servicios;

namespace Estadios_de_futbol.Controllers
{
    public class VentaController
    {
        private readonly VentaService _servicioVenta;
        private readonly Servicio_Estadio _servicioEstadio;
        private readonly Servicio_Evento _servicioEvento;

        public VentaController()
        {
            _servicioVenta = new VentaService();
            _servicioEstadio = new Servicio_Estadio();
            _servicioEvento = new Servicio_Evento();
        }

        public async Task<(bool exito, string mensaje)> RegistrarVenta(Venta venta, Evento evento)
        {
            if (venta.CantidadBoletos > evento.BoletosDisponibles)
                return (false, "No hay suficientes boletos disponibles.");

            var resultado = await _servicioVenta.GuardarVenta(venta);
            if (!resultado.exito) return resultado;

            evento.BoletosDisponibles -= venta.CantidadBoletos;
            evento.DineroRecolectado = venta.CalcularTotal();
            await _servicioEvento.ActualizarEvento(evento);

            return (true, "Venta registrada correctamente.");
        }

        public async Task<(bool exito, string mensaje)> BorrarVentasPasadas()
        {
            return await _servicioVenta.EliminarVentasPasadas();
        }
    }
}