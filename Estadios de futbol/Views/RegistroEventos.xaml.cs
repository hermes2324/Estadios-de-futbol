using Estadios_de_futbol.Controllers;
using Estadios_de_futbol.Models;

namespace Estadios_de_futbol.Views;

public partial class RegistroEventos : ContentPage
{
    private readonly EventoController _controller;
    private readonly EstadioController _Estadio;
    private List<Estadio> _listaEstadios = new();

    private readonly Estadio _opcionAgregar = new()
    {
        Id = "__nuevo__",
        Nombre = "➕  Agregar nuevo estadio"
    };

    public RegistroEventos()
    {
        InitializeComponent();
        _controller = new EventoController();
        _Estadio = new EstadioController();
        FechaPicker.MinimumDate = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarEstadios();
    }

    public async Task CargarEstadios()
    {
        try
        {
            _listaEstadios = await _Estadio.ObtenerEstadios();
            var items = new List<Estadio>(_listaEstadios) { _opcionAgregar };
            sfComboBox.ItemsSource = items;
            sfComboBox.ItemDisplayBinding = new Binding("Nombre");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los estadios: {ex.Message}", "OK");
        }
    }

    private async void onEstadioSeleccion(object sender, EventArgs e)
    {
        if (sfComboBox.SelectedItem is not Estadio seleccionado) return;

        if (seleccionado.Id == "__nuevo__")
        {
            sfComboBox.SelectedItem = null;

            // Busca la TabbedPage y la pestaña Mapa ya existente
            TabbedPage? tabbed = null;

            if (App.Current?.MainPage is NavigationPage nav &&
                nav.CurrentPage is TabbedPage t)
                tabbed = t;
            else if (App.Current?.MainPage is TabbedPage td)
                tabbed = td;

            if (tabbed == null) return;

            var paginaMapa = tabbed.Children.OfType<Mapa>().FirstOrDefault();
            if (paginaMapa == null) return;

            // ✅ Activa modo agregar en la página que ya existe
            paginaMapa.ActivarModoAgregar(onEstadioGuardado: async (estadio) =>
            {
                await CargarEstadios();

                // Regresa a la pestaña de registro y selecciona el nuevo estadio
                tabbed.CurrentPage = this.Parent as Page ?? tabbed.Children[0];

                var nuevo = _listaEstadios.FirstOrDefault(es => es.Id == estadio.Id);
                if (nuevo != null)
                    sfComboBox.SelectedItem = nuevo;
            });

            // Cambia a la pestaña del mapa
            tabbed.CurrentPage = paginaMapa;
        }
    }

    private async void Guardar_Clicked(object sender, EventArgs e)
    {
        if (sfComboBox.SelectedItem is not Estadio estadioSeleccionado
            || estadioSeleccionado.Id == "__nuevo__")
        {
            await DisplayAlert("Error", "Selecciona un estadio válido.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(LocalEntry.Text) ||
            string.IsNullOrWhiteSpace(VisitanteEntry.Text) ||
            string.IsNullOrWhiteSpace(BoletoEntry.Text) ||
            string.IsNullOrWhiteSpace(PrecioEntry.Text))
        {
            await DisplayAlert("Error", "Completa todos los campos.", "OK");
            return;
        }

        if (!int.TryParse(BoletoEntry.Text, out int boletos) ||
            !decimal.TryParse(PrecioEntry.Text, out decimal precio))
        {
            await DisplayAlert("Error", "Boletos y precio deben ser números válidos.", "OK");
            return;
        }

        var evento = new Evento
        {
            NombreJuego = $"{LocalEntry.Text} vs {VisitanteEntry.Text}",
            BoletosTotales = boletos,
            BoletosDisponibles = boletos,
            Precio = precio,
            FechaHora = FechaPicker.Date + HoraPicker.Time,
            EstadioId = estadioSeleccionado.Id,
            EstadioNombre = estadioSeleccionado.Nombre,
            EstadioLatitud = estadioSeleccionado.Latitud,
            EstadioLongitud = estadioSeleccionado.Longitud
        };

        var (exito, mensaje) = await _controller.RegistrarEvento(evento);
        await DisplayAlert(exito ? "Éxito" : "Error", mensaje, "OK");
        if (exito) LimpiarFormulario();
    }

    private void LimpiarFormulario()
    {
        LocalEntry.Text = string.Empty;
        VisitanteEntry.Text = string.Empty;
        BoletoEntry.Text = string.Empty;
        PrecioEntry.Text = string.Empty;
        FechaPicker.Date = DateTime.Today;
        HoraPicker.Time = new TimeSpan(12, 0, 0);
        sfComboBox.SelectedItem = null;
    }
}