using Estadios_de_futbol.Controllers;
using Estadios_de_futbol.Models;

namespace Estadios_de_futbol.Views;

public partial class VentaBoletos : ContentPage
{
    private readonly EventoController _eventoController;
    private readonly VentaController _ventaController;

    private List<Estadio> _estadios = new();
    private List<Evento> _eventos = new();
    private Evento? _eventoSeleccionado;

    public VentaBoletos()
    {
        InitializeComponent();
        _eventoController = new EventoController();
        _ventaController = new VentaController();
        LblFechaActual.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarEstadios();
    }

    private async Task CargarEstadios()
    {
        try
        {
            MostrarCargando(true);
            _estadios = await _eventoController.ObtenerEstadios();
            PkrEstadio.Items.Clear();
            foreach (var e in _estadios)
                PkrEstadio.Items.Add(e.Nombre);
        }
        catch (Exception ex)
        {
            MostrarStatus($"Error al cargar estadios: {ex.Message}", esError: true);
        }
        finally { MostrarCargando(false); }
    }

    private async void OnEstadioChanged(object sender, EventArgs e)
    {
        if (PkrEstadio.SelectedIndex < 0) return;

        var estadioId = _estadios[PkrEstadio.SelectedIndex].Id;

        try
        {
            MostrarCargando(true);
            LimpiarFormulario();

            var todos = await _eventoController.ObtenerEventosPorEstadio(estadioId);
            _eventos = todos
                .Where(ev => ev.FechaHora > DateTime.Now.AddDays(2))
                .ToList();

            PkrEvento.Items.Clear();
            foreach (var ev in _eventos)
                PkrEvento.Items.Add(ev.NombreJuego);
        }
        catch (Exception ex)
        {
            MostrarStatus($"Error al cargar eventos: {ex.Message}", esError: true);
        }
        finally { MostrarCargando(false); }
    }

    private void OnEventoChanged(object sender, EventArgs e)
    {
        if (PkrEvento.SelectedIndex < 0) return;

        _eventoSeleccionado = _eventos[PkrEvento.SelectedIndex];

        LblNombreEvento.Text = _eventoSeleccionado.NombreJuego;
        LblFechaEvento.Text = _eventoSeleccionado.FechaHora
                                    .ToString("dddd, dd MMMM yyyy  HH:mm");
        LblTotales.Text = _eventoSeleccionado.BoletosTotales.ToString();
        LblDisponibles.Text = _eventoSeleccionado.BoletosDisponibles.ToString();
        LblRecaudado.Text = _eventoSeleccionado.DineroRecolectado
                                    .ToString("C", new System.Globalization.CultureInfo("es-MX"));
        LblPrecioUnitario.Text = _eventoSeleccionado.Precio
                                    .ToString("C", new System.Globalization.CultureInfo("es-MX"));

        FrameInfoEvento.IsVisible = true;
        FrameVenta.IsVisible = true;
        GridBotones.IsVisible = true;
        FrameStatus.IsVisible = false;
        EntCantidad.Text = string.Empty;
        LblTotal.Text = "$0.00";
    }

    private void OnCantidadChanged(object sender, TextChangedEventArgs e)
    {
        if (_eventoSeleccionado == null) return;

        if (int.TryParse(EntCantidad.Text, out int cantidad) && cantidad > 0)
            LblTotal.Text = (cantidad * _eventoSeleccionado.Precio)
                                .ToString("C", new System.Globalization.CultureInfo("es-MX"));
        else
            LblTotal.Text = "$0.00";
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (_eventoSeleccionado == null) return;

        if (!int.TryParse(EntCantidad.Text, out int cantidad) || cantidad <= 0)
        { MostrarStatus("Ingresa una cantidad válida.", esError: true); return; }

        if (cantidad > _eventoSeleccionado.BoletosDisponibles)
        { MostrarStatus($"Solo hay {_eventoSeleccionado.BoletosDisponibles} boletos disponibles.", esError: true); return; }

        if (string.IsNullOrWhiteSpace(EntCliente.Text))
        { MostrarStatus("Escribe el nombre del cliente.", esError: true); return; }

        if (string.IsNullOrWhiteSpace(EntTelefono.Text) || EntTelefono.Text.Length != 10)
        { MostrarStatus("El teléfono debe tener 10 dígitos.", esError: true); return; }

        var venta = new Venta
        {
            EventoId = _eventoSeleccionado.Id,
            NombreCliente = EntCliente.Text.Trim(),
            Telefono = EntTelefono.Text.Trim(),
            CantidadBoletos = cantidad,
            PrecioUnitario = _eventoSeleccionado.Precio,
            FechaVenta = DateTime.UtcNow
        };

        try
        {
            MostrarCargando(true);
            var (exito, mensaje) = await _ventaController.RegistrarVenta(venta, _eventoSeleccionado);

            if (exito)
            {
                MostrarStatus("✅ Venta registrada correctamente.", esError: false);
                OnEventoChanged(this, EventArgs.Empty);
                LimpiarFormulario();
            }
            else
                MostrarStatus($"❌ {mensaje}", esError: true);
        }
        catch (Exception ex)
        {
            MostrarStatus($"Error: {ex.Message}", esError: true);
        }
        finally { MostrarCargando(false); }
    }

    private void OnCancelarClicked(object sender, EventArgs e) => LimpiarFormulario();

    private async void OnBorrarPasadasClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert(
            "Confirmar",
            "¿Eliminar todas las ventas de eventos ya pasados?",
            "Sí, borrar", "Cancelar");

        if (!confirmar) return;

        try
        {
            MostrarCargando(true);
            var (exito, mensaje) = await _ventaController.BorrarVentasPasadas();
            MostrarStatus(exito ? $"✅ {mensaje}" : $"❌ {mensaje}", esError: !exito);
        }
        catch (Exception ex)
        {
            MostrarStatus($"Error: {ex.Message}", esError: true);
        }
        finally { MostrarCargando(false); }
    }

  
    private void LimpiarFormulario()
    {
        EntCantidad.Text = string.Empty;
        EntCliente.Text = string.Empty;
        EntTelefono.Text = string.Empty;
        LblTotal.Text = "$0.00";
        FrameStatus.IsVisible = false;
    }

    private void MostrarStatus(string mensaje, bool esError)
    {
        LblStatus.Text = mensaje;
        FrameStatus.BackgroundColor = esError
            ? Color.FromArgb("#3D1010")
            : Color.FromArgb("#103D10");
        FrameStatus.BorderColor = esError
            ? Color.FromArgb("#FF4444")
            : Color.FromArgb("#4CAF50");
        FrameStatus.IsVisible = true;
    }

    private void MostrarCargando(bool activo)
    {
        Cargando.IsRunning = activo;
        Cargando.IsVisible = activo;
    }
}