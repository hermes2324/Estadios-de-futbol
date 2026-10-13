using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase;

namespace Estadios_de_futbol.Base_de_datos
{
    public class Conexion
    {
        private static string url = Secretos.SupabaseUrl;
        private static string key = Secretos.SupabaseKey;
        private static Supabase.Client? _supabase;

        public static async Task<Supabase.Client> ObtenerClienteAsync()
        {
            if (_supabase == null)
            {
                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = false 
                };
                _supabase = new Supabase.Client(url, key, options);
                await _supabase.InitializeAsync();
            }
            return _supabase;
        }
    }
}
