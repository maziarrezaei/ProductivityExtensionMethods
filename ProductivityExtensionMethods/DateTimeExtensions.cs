#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System;

public static partial class ProductivityExtensions
{
    public static DateTime SetTime(this DateTime date, TimeSpan time)
    {
        return date.Date.Add(time);
    }

    public static DateTime SetTime(this DateTime date, int? hours = null, int? minutes = null, int? seconds = null, int? milliseconds = null, int? subMsTicks = null)
    {
        TimeSpan newTime = date.TimeOfDay.SetTime(0, hours, minutes, seconds, milliseconds, subMsTicks);

        return date.Date.Add(newTime);
    }

    public static TimeSpan SetTime(this TimeSpan ts, int? days, int? hours = null, int? minutes = null, int? seconds = null, int? milliseconds = null, int? subMsTicks = null)
    {
        TimeSpan ticks = subMsTicks.HasValue ? TimeSpan.FromTicks(subMsTicks.Value) : ts - TimeSpan.FromMilliseconds(ts.TotalMilliseconds);

        return new TimeSpan(days ?? ts.Days, hours ?? ts.Hours, minutes ?? ts.Minutes, seconds ?? ts.Seconds, milliseconds ?? ts.Milliseconds) + ticks;
    }

    public static DateTime SetDate(this DateTime date, int? year = null, int? month = null, int? day = null)
    {
        return new DateTime(year ?? date.Year, month ?? date.Month, day ?? date.Day).Add(date.TimeOfDay);
    }

    /// <summary>
    ///     Gets the first kind of day based on the passed day time.
    /// </summary>
    /// <param name="date">The date</param>
    /// <param name="dayKind">The kind of day</param>
    /// <returns>The DateTime associated with the first kind of day passed.</returns>
    public static DateTime GetFirst(this DateTime date, DayKind dayKind)
    {
        date = date.Date;

        if (dayKind <= DayKind.DayOfMonth)
        {
            DateTime firstDayOfMonth = date.AddDays(1 - date.Day);

            if (dayKind == DayKind.DayOfMonth)
                return firstDayOfMonth;

            if (firstDayOfMonth.DayOfWeek != (DayOfWeek)dayKind)
                return firstDayOfMonth.GetNext((DayOfWeek)dayKind);

            return firstDayOfMonth;
        }

        if (dayKind <= DayKind.DayOfYear)
        {
            DateTime firstDayOfYear = date.SetDate(month: 1, day: 1);

            if (dayKind == DayKind.DayOfYear)
                return firstDayOfYear;

            if (dayKind >= DayKind.SundayOfYear && dayKind <= DayKind.SaturdayOfYear)
            {
                var inMonthDayKind = (DayKind)((int)dayKind & ~0b01000);

                return date.GetFirst(inMonthDayKind);
            }
        }

        if (dayKind == DayKind.DayOfWeek)
            return date.DayOfWeek == DayOfWeek.Sunday ? date : date.GetPrevious(DayOfWeek.Sunday);

        throw new ArgumentOutOfRangeException(nameof(dayKind));
    }

    /// <summary>
    ///     Gets the last kind of day based on the passed day time.
    /// </summary>
    /// <param name="date">The  date</param>
    /// <returns>The DateTime associated with the last kind of day passed.</returns>
    public static DateTime GetLast(this DateTime date, DayKind dayKind)
    {
        if (dayKind <= DayKind.DayOfMonth)
        {
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            DateTime lastDayOfMonth = date.GetFirst(DayKind.DayOfMonth).AddDays(daysInMonth - 1);

            if (dayKind == DayKind.DayOfMonth)
                return lastDayOfMonth;

            int diff = (int)dayKind - (int)lastDayOfMonth.DayOfWeek;

            if (diff > 0)
                diff -= 7;

            return lastDayOfMonth.AddDays(diff);
        }

        if (dayKind <= DayKind.DayOfYear)
        {
            DateTime lastDayOfYear = date.Date.SetDate(month: 12, day: 31);

            if (dayKind == DayKind.DayOfYear)
                return lastDayOfYear;

            if (dayKind >= DayKind.SundayOfYear && dayKind <= DayKind.SaturdayOfYear)
            {
                var inMonthDayKind = (DayKind)((int)dayKind & ~0b01000);

                return date.GetLast(inMonthDayKind);
            }
        }

        if (dayKind == DayKind.DayOfWeek)
            return date.DayOfWeek == DayOfWeek.Saturday ? date.Date : date.GetNext(DayOfWeek.Saturday);

        throw new ArgumentOutOfRangeException(nameof(dayKind));
    }

    /// <summary>
    ///     Gets the DateTime for the first following date that is the given day of the week
    /// </summary>
    /// <param name="date">The date</param>
    /// <param name="dayOfWeek">The following day of the specified DayOfWeek</param>
    public static DateTime GetNext(this DateTime date, DayOfWeek dayOfWeek)
    {
        int diff = dayOfWeek - date.DayOfWeek;

        if (diff <= 0)
            diff += 7;

        return date.Date.AddDays(diff);
    }

    /// <summary>
    ///     Gets a DateTime representing the first date following the current date which falls on the given day of the week
    /// </summary>
    /// <param name="date">The date</param>
    /// <param name="dayOfWeek">The following day of the specified DayOfWeek</param>
    public static DateTime GetPrevious(this DateTime date, DayOfWeek dayOfWeek)
    {
        int diff = dayOfWeek - date.DayOfWeek;

        if (diff >= 0)
            diff -= 7;

        return date.Date.AddDays(diff);
    }

    public enum DayKind
    {
        SundayOfMonth,
        MondayOfMonth,
        TuesdayOfMonth,
        WednesdayOfMonth,
        ThursdayOfMonth,
        FridayOfMonth,
        SaturdayOfMonth,
        DayOfMonth,
        SundayOfYear = 0b01000,
        MondayOfYear,
        TuesdayOfYear,
        WednesdayOfYear,
        ThursdayOfYear,
        FridayOfYear,
        SaturdayOfYear,
        DayOfYear,
        DayOfWeek
    }
}
