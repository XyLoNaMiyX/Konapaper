// Made by Lonami Exo (C) LonamiWebs
// Creation date: february 2016
// Modifications:
// - No modifications made
using System;

public static class UnixDate
{
    // epoch datetime
    static readonly DateTime epoch = new DateTime(1970, 1, 1);

    // unix time utils
    public static long DateTimeToUnixTime(DateTime dt)
    { return Convert.ToInt64((dt - epoch).TotalMilliseconds); }

    public static DateTime UnixTimeToDateTime(long ut)
    { return epoch.Add(TimeSpan.FromMilliseconds(ut)); }
}