<h1 align="center">🏟️ Estadios de Fútbol · .NET MAUI</h1>

<p align="center">
  App móvil para administrar estadios, partidos y <b>venta de boletos</b>,<br />
  con los estadios ubicados en un <b>mapa interactivo</b> y datos en Supabase.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/C%23-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/.NET%20MAUI%209-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET MAUI 9" />
  <img src="https://img.shields.io/badge/Supabase-3ECF8E?style=for-the-badge&logo=supabase&logoColor=white" alt="Supabase" />
  <img src="https://img.shields.io/badge/Mapsui%20%2F%20Syncfusion%20Maps-1E88E5?style=for-the-badge&logo=openstreetmap&logoColor=white" alt="Mapas" />
</p>

---

## ✨ Funciones

| 🏟️ Estadios | ⚽ Eventos | 🎟️ Boletos |
|---|---|---|
| Registro de estadios con latitud y longitud | Partido con equipo local y visitante | Venta con nombre y teléfono del cliente |
| Mapa interactivo con cada estadio | Fecha, hora y estadio anfitrión | Cantidad de boletos y precio unitario |
| | Boletos totales, disponibles y precio | Dinero recolectado por evento |

## 🧱 Arquitectura

```text
Estadios de futbol/
├── Views/          ← pantallas con pestañas: estadios, eventos, mapa y venta
├── Controllers/    ← lógica de estadios y eventos
├── Servicios/      ← acceso a las tablas de Supabase
├── Models/         ← Estadio, Evento, Venta
└── Base de datos/  ← conexión a Supabase
```

## 🚀 Cómo correrlo

1. Instala **Visual Studio 2022** con la carga de trabajo **.NET MAUI**.
2. Crea tu archivo de claves (no se sube a GitHub):

   ```bat
   copy "Estadios de futbol\Base de datos\Secretos.cs.ejemplo" "Estadios de futbol\Base de datos\Secretos.cs"
   ```

   y llena la URL y la clave **anon** de tu proyecto de Supabase y tu licencia de Syncfusion (la Community es gratuita).
3. Abre `Estadios de futbol.sln` y ejecuta en el emulador de Android o en tu teléfono.

---

<p align="center">Hecho por <a href="https://github.com/hermes2324">Hermes Verdugo</a> · TecNM Campus Guasave</p>
