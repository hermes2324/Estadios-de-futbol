using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Estadios_de_futbol.Models;
using Estadios_de_futbol.Servicios;

using MapsuiColor = Mapsui.Styles.Color;
using MapsuiBrush = Mapsui.Styles.Brush;
using MapsuiPen = Mapsui.Styles.Pen;

namespace Estadios_de_futbol.Views;

public partial class Mapa : ContentPage
{
    private MapControl _mapControl = null!;
    private WritableLayer _pinLayer = null!;
    private WritableLayer _eventLayer = null!;

    private double _latitud;
    private double _longitud;
    private bool _pinColocado = false;
    private bool _modoAgregar = false;

    private double _estadioLatitudSeleccionado;
    private double _estadioLongitudSeleccionado;

    private List<Evento> _eventos = new();
    private Func<Estadio, Task>? _onEstadioGuardado;

    public Mapa()
    {
        InitializeComponent();
        InicializarMapa();
        MostrarModoNormal();
    }

    // ── Modo agregar ──────────────────────────────────────────────────────
    public void ActivarModoAgregar(Func<Estadio, Task> onEstadioGuardado)
    {
        _modoAgregar = true;
        _onEstadioGuardado = onEstadioGuardado;
        _pinColocado = false;

        _pinLayer?.Clear();
        _mapControl?.Map?.RefreshGraphics();

        EntNombre.Text = string.Empty;
        LblLat.Text = "—";
        LblLon.Text = "—";
        BtnGuardar.IsEnabled = false;
        BtnGuardar.Text = "GUARDAR ESTADIO";
        LblHint.Text = "📌 Toca el mapa para colocar el pin";
        LblHint.TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#555555");

        MostrarModoAgregar();
    }

    public void DesactivarModoAgregar()
    {
        _modoAgregar = false;
        _onEstadioGuardado = null;
        _pinLayer?.Clear();
        _mapControl?.Map?.RefreshGraphics();
        MostrarModoNormal();
    }

    private void MostrarModoNormal()
    {
        Title = "Mapa";
        PanelNormal.IsVisible = true;
        PanelAgregar.IsVisible = false;
    }

    private void MostrarModoAgregar()
    {
        Title = "Agregar Estadio";
        PanelNormal.IsVisible = false;
        PanelAgregar.IsVisible = true;
    }

    // ── Inicializa el mapa ────────────────────────────────────────────────
    private void InicializarMapa()
    {
        _mapControl = new MapControl
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        _pinLayer = new WritableLayer { Style = null };
        _eventLayer = new WritableLayer { Style = null };

        _mapControl.Map?.Layers.Add(OpenStreetMap.CreateTileLayer());
        _mapControl.Map?.Layers.Add(_eventLayer);
        _mapControl.Map?.Layers.Add(_pinLayer);

        var centro = SphericalMercator.FromLonLat(-102.5528, 23.6345);
        _mapControl.Map?.Navigator.CenterOnAndZoomTo(
            new MPoint(centro.x, centro.y), resolution: 5000);

        _mapControl.Info += OnMapInfo;
        MapContainer.Children.Add(_mapControl);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_modoAgregar)
            await CargarPinsEventos();
    }

    // ── Carga pins de eventos ─────────────────────────────────────────────
    private async Task CargarPinsEventos()
    {
        try
        {
            var servicioEvento = new Servicio_Evento();
            _eventos = await servicioEvento.ObtenerEventos();
            _eventLayer.Clear();

            // Agrupa por estadio para poner UN solo pin por estadio
            var porEstadio = _eventos.GroupBy(ev => ev.EstadioId);

            foreach (var grupo in porEstadio)
            {
                var primero = grupo.First();
                var punto = SphericalMercator.FromLonLat(primero.EstadioLongitud, primero.EstadioLatitud);
                var feature = new PointFeature(new MPoint(punto.x, punto.y));

                // Guarda el estadioId para identificar el grupo al tocar
                feature["estadio_id"] = primero.EstadioId;

                // Pin rojo
                feature.Styles.Add(new SymbolStyle
                {
                    SymbolScale = 1.0,
                    Fill = new MapsuiBrush(MapsuiColor.FromArgb(255, 220, 50, 50)),
                    Outline = new MapsuiPen(MapsuiColor.White, 3),
                    SymbolType = SymbolType.Ellipse,
                    Offset = new Offset(0, 14)
                });

                // Punto blanco interior
                feature.Styles.Add(new SymbolStyle
                {
                    SymbolScale = 0.3,
                    Fill = new MapsuiBrush(MapsuiColor.White),
                    Outline = new MapsuiPen(MapsuiColor.White, 0),
                    SymbolType = SymbolType.Ellipse,
                    Offset = new Offset(0, 14)
                });

                _eventLayer.Add(feature);
            }

            _mapControl.Map?.RefreshGraphics();
        }
        catch { }
    }

    // ── Toque en el mapa ──────────────────────────────────────────────────
    private void OnMapInfo(object? sender, MapInfoEventArgs e)
    {
        var worldPoint = e.WorldPosition;
        if (worldPoint == null) return;

        if (_modoAgregar)
        {
            var (lon, lat) = SphericalMercator.ToLonLat(worldPoint.X, worldPoint.Y);
            _longitud = lon;
            _latitud = lat;

            LblLat.Text = _latitud.ToString("F6");
            LblLon.Text = _longitud.ToString("F6");

            _pinLayer.Clear();
            _pinLayer.Add(new PointFeature(new MPoint(worldPoint.X, worldPoint.Y))
            {
                Styles =
                {
                    new SymbolStyle
                    {
                        SymbolScale = 1.0,
                        Fill        = new MapsuiBrush(MapsuiColor.FromArgb(255, 30, 120, 220)),
                        Outline     = new MapsuiPen(MapsuiColor.White, 3),
                        SymbolType  = SymbolType.Ellipse,
                        Offset      = new Offset(0, 14)
                    },
                    new SymbolStyle
                    {
                        SymbolScale = 0.3,
                        Fill        = new MapsuiBrush(MapsuiColor.White),
                        SymbolType  = SymbolType.Ellipse,
                        Offset      = new Offset(0, 14)
                    }
                }
            });

            _mapControl.Map?.RefreshGraphics();
            _pinColocado = true;
            BtnGuardar.IsEnabled = true;
            LblHint.Text = "✅ Ubicación lista — escribe el nombre y guarda";
            LblHint.TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#4CAF50");
        }
        else
        {
            // Busca el estadio más cercano al toque
            var clickPoint = new MPoint(worldPoint.X, worldPoint.Y);
            const double TOLERANCIA = 25000;

            string? estadioIdCercano = null;
            double menorDist = double.MaxValue;

            foreach (var ev in _eventos)
            {
                var puntoEv = SphericalMercator.FromLonLat(ev.EstadioLongitud, ev.EstadioLatitud);
                double dist = Math.Sqrt(
                    Math.Pow(puntoEv.x - clickPoint.X, 2) +
                    Math.Pow(puntoEv.y - clickPoint.Y, 2));

                if (dist < menorDist && dist < TOLERANCIA)
                {
                    menorDist = dist;
                    estadioIdCercano = ev.EstadioId;
                }
            }

            if (estadioIdCercano == null) return;

            // ✅ TODOS los eventos de ese estadio
            var eventosDelEstadio = _eventos
                .Where(ev => ev.EstadioId == estadioIdCercano)
                .OrderBy(ev => ev.FechaHora)
                .ToList();

            var primero = eventosDelEstadio.First();
            _estadioLatitudSeleccionado = primero.EstadioLatitud;
            _estadioLongitudSeleccionado = primero.EstadioLongitud;

            MainThread.BeginInvokeOnMainThread(() =>
                MostrarEventosDeEstadio(eventosDelEstadio));
        }
    }

    // ── Tarjetas de eventos ───────────────────────────────────────────────
    private void MostrarEventosDeEstadio(List<Evento> eventos)
    {
        StackEventos.Children.Clear();

        StackEventos.Children.Add(new Label
        {
            Text = $"📍 {eventos[0].EstadioNombre.ToUpper()}",
            TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#F5C842"),
            FontAttributes = FontAttributes.Bold,
            FontSize = 13,
            CharacterSpacing = 1
        });

        foreach (var ev in eventos)
        {
            int vendidos = ev.BoletosTotales - ev.BoletosDisponibles;

            var tarjeta = new Frame
            {
                BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#1E1E1E"),
                BorderColor = Microsoft.Maui.Graphics.Color.FromArgb("#333333"),
                CornerRadius = 12,
                Padding = new Thickness(14, 12)
            };

            var stack = new VerticalStackLayout { Spacing = 8 };

            stack.Children.Add(new Label
            {
                Text = ev.NombreJuego,
                TextColor = Microsoft.Maui.Graphics.Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16
            });

            stack.Children.Add(new Label
            {
                Text = $"📅 {ev.FechaHora:dd MMM yyyy, hh:mm tt}",
                TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#8B949E"),
                FontSize = 12
            });

            stack.Children.Add(new BoxView
            {
                HeightRequest = 1,
                Color = Microsoft.Maui.Graphics.Color.FromArgb("#333333")
            });

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star)
                },
                ColumnSpacing = 10
            };

            var colVendidos = new VerticalStackLayout { Spacing = 2 };
            colVendidos.Children.Add(new Label { Text = "🎟 Vendidos", TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#8B949E"), FontSize = 11 });
            colVendidos.Children.Add(new Label { Text = vendidos.ToString(), TextColor = Microsoft.Maui.Graphics.Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 20 });

            var colLibres = new VerticalStackLayout { Spacing = 2 };
            colLibres.Children.Add(new Label { Text = "✅ Libres", TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#8B949E"), FontSize = 11 });
            colLibres.Children.Add(new Label { Text = ev.BoletosDisponibles.ToString(), TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#4CAF50"), FontAttributes = FontAttributes.Bold, FontSize = 20 });

            Grid.SetColumn(colVendidos, 0);
            Grid.SetColumn(colLibres, 1);
            grid.Children.Add(colVendidos);
            grid.Children.Add(colLibres);
            stack.Children.Add(grid);

            stack.Children.Add(new Label
            {
                Text = $"💰 Recaudado: {ev.DineroRecolectado.ToString("C", new System.Globalization.CultureInfo("es-MX"))}",
                TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#4CAF50"),
                FontSize = 13
            });

            double lat = ev.EstadioLatitud;
            double lon = ev.EstadioLongitud;

            var btnRuta = new Button
            {
                Text = "🧭  Trazar Ruta",
                BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#238636"),
                TextColor = Microsoft.Maui.Graphics.Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                HeightRequest = 46,
                CornerRadius = 23
            };
            btnRuta.Clicked += (s, args) =>
            {
                _estadioLatitudSeleccionado = lat;
                _estadioLongitudSeleccionado = lon;
                OnTrazarRutaClicked(s, args);
            };

            stack.Children.Add(btnRuta);
            tarjeta.Content = stack;
            StackEventos.Children.Add(tarjeta);
        }

        LblInfoEvento.IsVisible = false;
        ScrollEventos.IsVisible = true;
    }

    // ── Ruta ──────────────────────────────────────────────────────────────
    private async void OnTrazarRutaClicked(object sender, EventArgs e)
    {
        try
        {
            var destino = SphericalMercator.FromLonLat(
                _estadioLongitudSeleccionado,
                _estadioLatitudSeleccionado);

            _mapControl.Map?.Navigator.CenterOnAndZoomTo(
                new MPoint(destino.x, destino.y), resolution: 500);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // ── Guardar estadio ───────────────────────────────────────────────────
    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (!_pinColocado)
        { await DisplayAlert("Falta", "Toca el mapa primero.", "OK"); return; }

        if (string.IsNullOrWhiteSpace(EntNombre.Text))
        { await DisplayAlert("Falta", "Escribe el nombre del estadio.", "OK"); return; }

        var nuevo = new Estadio
        {
            Nombre = EntNombre.Text.Trim(),
            Latitud = _latitud,
            Longitud = _longitud
        };

        try
        {
            BtnGuardar.IsEnabled = false;
            BtnGuardar.Text = "Guardando...";

            var servicio = new Servicio_Estadio();
            var (exito, mensaje) = await servicio.GuardarEstadio(nuevo);

            if (exito)
            {
                DesactivarModoAgregar();
                if (_onEstadioGuardado != null)
                    await _onEstadioGuardado(nuevo);
            }
            else
            {
                await DisplayAlert("Error", mensaje, "OK");
                BtnGuardar.IsEnabled = true;
                BtnGuardar.Text = "GUARDAR ESTADIO";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
            BtnGuardar.IsEnabled = true;
            BtnGuardar.Text = "GUARDAR ESTADIO";
        }
    }
}