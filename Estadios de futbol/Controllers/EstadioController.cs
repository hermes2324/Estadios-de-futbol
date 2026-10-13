using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Estadios_de_futbol.Servicios;
using Estadios_de_futbol.Models;
using System.Threading.Tasks;

namespace Estadios_de_futbol.Controllers
{
    public class EstadioController
    {
        private readonly Servicio_Estadio _servicio;

        public EstadioController()
        {
            _servicio = new Servicio_Estadio();
        }

        public async Task<List<Estadio>> ObtenerEstadios()
        {
            return await _servicio.ObtenerEstadios();
        }
    }
}