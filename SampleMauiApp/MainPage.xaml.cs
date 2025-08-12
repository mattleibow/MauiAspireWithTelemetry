namespace SampleMauiApp;

public partial class MainPage : ContentPage
{
    private readonly WeatherApiClient _weatherApiClient;

    public MainPage(WeatherApiClient weatherApiClient)
    {
        InitializeComponent();

        _weatherApiClient = weatherApiClient;

        //var weather = await _weatherApiClient.GetWeatherAsync();

        cv.ItemsSource = Enumerable.Range(1, 1_000).Select(i => $"Item {i}").ToArray();
    }
}
