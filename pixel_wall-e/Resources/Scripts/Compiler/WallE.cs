using System;
using System.Collections.Generic;

public static class WallE
{
    public static List<string> errors = new List<string>();
    public static void AddError(string message, int line, int column)
    {
        errors.Add($"Error: {message} at Col {column} Line {line}");
    }
    public static void CleanErrors()=>errors.Clear();
    public static bool HasErrors()=>errors.Count!=0;
}
