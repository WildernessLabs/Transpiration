namespace Transpiration;

public partial class Clock
{
    bool showCalendar = false;

    public void ToggleCalendar()
    {
        showCalendar = !showCalendar;
    }
}