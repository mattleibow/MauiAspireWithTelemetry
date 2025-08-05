using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SampleMauiApp;

public class MauiDiagnostics
{
    public MauiDiagnostics(IMeterFactory meterFactory)
    {
        ActivitySource = new ActivitySource("Microsoft.Maui.Diagnostics", "1.0.0");

        Meters = meterFactory.Create("Microsoft.Maui.Diagnostics", "1.0.0");

        MeasureCounter = Meters.CreateCounter<int>("maui.layout.measure_count", "{times}", "The number of times a measure happened.");
        ArrangeCounter = Meters.CreateCounter<int>("maui.layout.arrange_count", "{times}", "The number of times an arrange happened.");

        MeasureHistogram = Meters.CreateHistogram<int>("maui.layout.measure_duration", "ns");
        ArrangeHistogram = Meters.CreateHistogram<int>("maui.layout.arrange_duration", "ns");
    }

    public ActivitySource ActivitySource { get; }

    public Meter Meters { get; }


    public Counter<int> MeasureCounter { get; }

    public Counter<int> ArrangeCounter { get; }


    public Histogram<int> MeasureHistogram { get; }

    public Histogram<int> ArrangeHistogram { get; }
}

public static class MauiDiagnosticsExtensions
{
    public static IServiceCollection AddMauiDiagnostics(this IServiceCollection services)
    {
        services.AddSingleton<MauiDiagnostics>();

        services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddMeter("Microsoft.Maui.Diagnostics");
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource("Microsoft.Maui.Diagnostics");
            });

        return services;
    }

    public static MauiDiagnostics? GetMauiDiagnostics(this IView view)
    {
        return view.Handler?.MauiContext?.Services.GetRequiredService<MauiDiagnostics>();
    }

    public static Activity? StartActivity(this IView view, string name)
    {
        var elementName = view.GetType().Name;

        var activity = view.GetMauiDiagnostics()?.ActivitySource.StartActivity($"{name} {elementName}");

        activity?.SetTag("element.type", view.GetType().FullName);

        if (view is Element element)
        {
            activity?.SetTag("element.id", element.Id);
            activity?.SetTag("element.automation_id", element.AutomationId);
            activity?.SetTag("element.class_id", element.ClassId);
            activity?.SetTag("element.style_id", element.StyleId);
        }

        if (view is VisualElement ve)
        {
            activity?.SetTag("element.class", ve.@class);
            activity?.SetTag("element.frame", ve.Frame);
        }

        return activity;
    }

    public static void RecordMeasure(this IView view, TimeSpan duration)
    {
        var diag = view.GetMauiDiagnostics();
        diag?.MeasureCounter?.Add(1);
        diag?.MeasureHistogram?.Record((int)duration.TotalNanoseconds);
    }

    public static void RecordArrange(this IView view, TimeSpan duration)
    {
        var diag = view.GetMauiDiagnostics();
        diag?.ArrangeCounter?.Add(1);
        diag?.ArrangeHistogram?.Record((int)duration.TotalNanoseconds);
    }
}
