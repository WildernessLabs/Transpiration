using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using System;

namespace Transpiration
{
    public partial class Clock
    {
        string[] days = { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };

        void DrawCalendar(MicroGraphics g)
        {
            var date = DateTime.Now.AddHours(Settings.TimeZoneOffset);

            int top = 28;
            int cellHeight = 30;
            int cellWidth = 34;
            int textOffset = 5;

            int startDay = (int)new DateTime(date.Year, date.Month, 1).DayOfWeek - 1;
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            int x, y, day;

            g.Clear();

            g.DrawText(120, 0, DateTime.Now.ToString("MMMM, yyyy"), Settings.ThemeColor, alignmentH: HorizontalAlignment.Center);

            //loop over every cell in the calendar
            for (int j = 0; j < 8; j++)
            {
                y = top + (j * cellHeight);
                for (int i = 0; i < 7; i++)
                {
                    x = 1 + cellWidth / 2 + cellWidth * i;

                    //Draw week day abbrivations 
                    if(j == 0)
                    {
                        g.DrawText(x, y + textOffset, days[i], Settings.ThemeColor, alignmentH: HorizontalAlignment.Center);
                        continue;
                    }

                    //calculate the day of the month for a given cell .... note the offset for abreviations 
                    day = i + (j - 1) * 7 - startDay;

                    //bounds checking to make sure it's a valid day
                    if (day > 0 && day <= daysInMonth)
                    {
                        //if the cell is today then invert 
                        if (day == date.Day)
                        {
                            g.DrawRectangle(x - cellWidth/2, y, cellWidth - 1, cellHeight - 1, Settings.ThemeColor, true);
                            g.DrawText(x, y + textOffset, $"{day}", Color.Black, alignmentH: HorizontalAlignment.Center);
                        }
                        else
                        {
                            g.DrawText(x, y + textOffset, $"{day}", Settings.ThemeColor, alignmentH: HorizontalAlignment.Center);
                        }
                    }
                }
            }

            g.Show();
        }
    }
}