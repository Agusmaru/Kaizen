namespace Kaizen.Web.ViewModels;

public readonly record struct CalendarRange(DateOnly Start, DateOnly End)
{
    public const int MaximumDays = 42;

    public static bool TryCreate(DateOnly start, DateOnly end, out CalendarRange range)
    {
        range = default;
        if (end <= start || end.DayNumber - start.DayNumber > MaximumDays) return false;
        range = new(start, end);
        return true;
    }

    public bool Contains(DateOnly date) => date >= Start && date < End;
}
