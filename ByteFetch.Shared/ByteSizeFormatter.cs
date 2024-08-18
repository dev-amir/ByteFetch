namespace ByteFetch.Shared;

public static class ByteSizeFormatter
{
    public static string GetReadableByteSize(long size)
    {
        const int scale = 1024;
        double unitValue;
        string suffix;

        if (size >= Power(scale, 4)) // Terabytes
        {
            unitValue = (double)size / Math.Pow(scale, 4);
            suffix = "TB";
        }
        else if (size >= Power(scale, 3)) // Gigabytes
        {
            unitValue = (double)size / Math.Pow(scale, 3);
            suffix = "GB";
        }
        else if (size >= Power(scale, 2)) // Megabytes
        {
            unitValue = (double)size / Math.Pow(scale, 2);
            suffix = "MB";
        }
        else if (size >= scale) // Kilobytes
        {
            unitValue = (double)size / scale;
            suffix = "KB";
        }
        else
        {
            unitValue = size;
            suffix = "B";
        }

        return $"{unitValue:0.##} {suffix}";

    }

    private static long Power(long baseNumber, int power)
    {
        var number = baseNumber;
        for (var i = 1; i < power; i++)
            number *= baseNumber;
        return number;
    }
}
