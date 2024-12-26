using System;
using System.Globalization;

public static class DateKeywordFileParser
{
    public static string Parse(string fileName, string format = "yyyy-MM-dd HH:mm", string locale = "en-US")
    {
        DateTime date = default;
        var culture = new CultureInfo(locale);

        if (fileName.Contains("YESTERDAY", StringComparison.OrdinalIgnoreCase))
        {
            date = DateTime.Now.AddDays(-1);
            fileName =  fileName.Replace("YESTERDAY", "", StringComparison.OrdinalIgnoreCase);
        }
        else if (fileName.Contains("TODAY", StringComparison.OrdinalIgnoreCase))
        {
            date = DateTime.Today;
            fileName = fileName.Replace("TODAY", "", StringComparison.OrdinalIgnoreCase);
        }
        else if (fileName.Contains("NOW", StringComparison.OrdinalIgnoreCase))
        {
            date = DateTime.Now;
            fileName = fileName.Replace("NOW", "", StringComparison.OrdinalIgnoreCase);
        }

        if (fileName.Contains("+") || fileName.Contains("-"))
        {
            var offsetIndex = FindOffsetIndex(fileName);
            if (offsetIndex != -1)
            {
                var dotIndex = fileName.IndexOf('.', offsetIndex);
                var offset = fileName.Substring(offsetIndex, dotIndex - offsetIndex);
                fileName = ApplyOffset(fileName, offset, date, format, culture);
            }
        }

        return fileName;
    }

    private static int FindOffsetIndex(string fileName)
    {
        var plusIndex = fileName.IndexOf('+');
        var minusIndex = fileName.IndexOf('-');
        if (plusIndex == -1 && minusIndex == -1)
            return -1;

        if (plusIndex == -1)
            return minusIndex;

        if (minusIndex == -1)
            return plusIndex;

        return Math.Min(plusIndex, minusIndex);
    }

    private static string ApplyOffset(string fileName, string offset, DateTime previousDate, string format, CultureInfo culture)
    {
        var unit = offset[offset.Length - 1];
        var value = int.Parse(offset.Substring(1, offset.Length - 2));

        if (offset[0] == '-')
        {
            value = -value;
        }

        if (unit == 'd')
        {
            previousDate = previousDate.AddDays(value);
        }
        else if (unit == 'h')
        {
            previousDate = previousDate.AddHours(value);
        }
        else
        {
            throw new ArgumentException($"Unsupported offset unit: {unit}");
        }
        return fileName.Replace($"{offset}", previousDate.ToString(format, culture));
    }
}
