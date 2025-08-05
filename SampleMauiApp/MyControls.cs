namespace SampleMauiApp;

public class MyScrollView : ScrollView
{
    protected override Size ArrangeOverride(Rect bounds)
    {
        using var activity = this.StartActivity("ArrangeOverride");
        var result = base.ArrangeOverride(bounds);
        activity.Stop();
        this.RecordArrange(activity.Duration);
        return result;
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        using var activity = this.StartActivity("MeasureOverride");
        var result = base.MeasureOverride(widthConstraint, heightConstraint);
        activity.Stop();
        this.RecordMeasure(activity.Duration);
        return result;
    }
}

public class MyButton : Button
{
    protected override Size ArrangeOverride(Rect bounds)
    {
        using var activity = this.StartActivity("ArrangeOverride");
        var result = base.ArrangeOverride(bounds);
        activity.Stop();
        this.RecordArrange(activity.Duration);
        return result;
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        using var activity = this.StartActivity("MeasureOverride");
        var result = base.MeasureOverride(widthConstraint, heightConstraint);
        activity.Stop();
        this.RecordMeasure(activity.Duration);
        return result;
    }
}

public class MyLabel : Label
{
    protected override Size ArrangeOverride(Rect bounds)
    {
        using var activity = this.StartActivity("ArrangeOverride");
        var result = base.ArrangeOverride(bounds);
        activity.Stop();
        this.RecordArrange(activity.Duration);
        return result;
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        using var activity = this.StartActivity("MeasureOverride");
        var result = base.MeasureOverride(widthConstraint, heightConstraint);
        activity.Stop();
        this.RecordMeasure(activity.Duration);
        return result;
    }
}

public class MyImage : Image
{
    protected override Size ArrangeOverride(Rect bounds)
    {
        using var activity = this.StartActivity("ArrangeOverride");
        var result = base.ArrangeOverride(bounds);
        activity.Stop();
        this.RecordArrange(activity.Duration);
        return result;
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        using var activity = this.StartActivity("MeasureOverride");
        var result = base.MeasureOverride(widthConstraint, heightConstraint);
        activity.Stop();
        this.RecordMeasure(activity.Duration);
        return result;
    }
}

public class MyVerticalStackLayout : VerticalStackLayout
{
    protected override Size ArrangeOverride(Rect bounds)
    {
        using var activity = this.StartActivity("ArrangeOverride");
        var result = base.ArrangeOverride(bounds);
        activity.Stop();
        this.RecordArrange(activity.Duration);
        return result;
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        using var activity = this.StartActivity("MeasureOverride");
        var result = base.MeasureOverride(widthConstraint, heightConstraint);
        activity.Stop();
        this.RecordMeasure(activity.Duration);
        return result;
    }
}
