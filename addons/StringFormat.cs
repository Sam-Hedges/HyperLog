using System;
using System.Collections.Generic;
using System.Text;
using Godot;

/// <summary>
/// C# Implementation of GDScript format strings % Operator
/// </summary>
// Based on sprintf() in ustring.cpp 
// https://github.com/godotengine/godot/blob/master/core/string/ustring.cpp#L4905
public static class StringFormat
{
    /// <summary>
    /// Formats strings that contain certain placeholder character-sequences.
    /// These placeholders can then easily be replaced by parameters handed to the format string.
    /// </summary>
    /// <param name="format">A format string with a placeholder '%s'.</param>
    /// <param name="value">The value that the placeholder is replaced with.</param>
    public static string Format(string format, Variant value, out bool error)
    {
        List<Variant> list = new List<Variant> { value };
        return Format(format, list, out error);
    }

    /// <summary>
    /// Formats strings that contain certain placeholder character-sequences.
    /// These placeholders can then easily be replaced by parameters handed to the format string.
    /// </summary>
    /// <param name="format">A format string with multiple placeholders '%s'.</param>
    /// <param name="values">The list of value that placeholders are sequentially replaced with.</param>
    public static string Format(string format, List<Variant> values, out bool error)
    {
        // Initialize a StringBuilder to hold the resulting formatted string.
        StringBuilder formatted = new StringBuilder();

        // Control flags for formatting options.
        bool inFormat = false;
        int valueIndex = 0;
        int minChars = 0;
        int minDecimals = 6; // Default number of decimal places for floats.
        bool inDecimals = false;
        bool padWithZeros = false;
        bool leftJustified = false;
        bool showSign = false;
        bool asUnsigned = false;

        // Assume error until proven otherwise.
        error = true;

        // Loop through each character in the format string.
        for (int i = 0; i < format.Length; i++)
        {
            char c = format[i];

            if (inFormat)
            {
                // We're in a format specification.
                switch (c)
                {
                    case '%':
                        // "%%" should output a literal "%".
                        formatted.Append('%');
                        inFormat = false;
                        break;
                    case 'd': // Integer (signed).
                    case 'o': // Octal.
                    case 'x': // Hexadecimal (lowercase).
                    case 'X': // Hexadecimal (uppercase).
                        if (valueIndex >= values.Count)
                            return "not enough arguments for format string";

                        // The value should be an integer or float.
                        if (!(values[valueIndex].Obj is int || values[valueIndex].Obj is float))
                            return "a number is required";

                        // Convert the value to a long.
                        long value = Convert.ToInt64(values[valueIndex].Obj);
                        int baseValue = 16;
                        bool capitalize = false;
                        switch (c)
                        {
                            case 'd':
                                baseValue = 10;
                                break;
                            case 'o':
                                baseValue = 8;
                                break;
                            case 'x': break;
                            case 'X':
                                capitalize = true;
                                break;
                        }

                        // Format the number based on the specified base and sign options.
                        string str = asUnsigned
                            ? Convert.ToUInt64(value)
                                .ToString(baseValue == 10 ? "D" : baseValue == 8 ? "O" : capitalize ? "X" : "x")
                            : Math.Abs(value)
                                .ToString(baseValue == 10 ? "D" : baseValue == 8 ? "O" : capitalize ? "X" : "x");

                        int numberLen = str.Length;
                        bool negative = value < 0 && !asUnsigned;

                        // Determine padding character and apply padding.
                        int padCharsCount = (negative || showSign) ? minChars - 1 : minChars;
                        string padChar = padWithZeros ? "0" : " ";

                        if (leftJustified)
                            str = str.PadRight(padCharsCount, padChar[0]);
                        else
                            str = str.PadLeft(padCharsCount, padChar[0]);

                        // Add sign if needed.
                        if (showSign || negative)
                        {
                            string signChar = negative ? "-" : "+";
                            if (leftJustified)
                                str = signChar + str;
                            else
                                str = str.Insert(padWithZeros ? 0 : str.Length - numberLen, signChar);
                        }

                        // Append the formatted number.
                        formatted.Append(str);
                        ++valueIndex;
                        inFormat = false;
                        break;

                    case 'v': // Vector.
                        if (valueIndex >= values.Count)
                            return "not enough arguments for format string";

                        int componentCount = 0;
                        string vectorStr = "(";

                        // Determine the type of vector and extract the components.
                        if (values[valueIndex].Obj is Vector2 vec2)
                        {
                            componentCount = 2;
                            vectorStr += FormatVectorComponent(vec2.X, minDecimals, minChars, padWithZeros,
                                leftJustified);
                            vectorStr += ", " + FormatVectorComponent(vec2.Y, minDecimals, minChars, padWithZeros,
                                leftJustified);
                        }
                        else if (values[valueIndex].Obj is Vector3 vec3)
                        {
                            componentCount = 3;
                            vectorStr += FormatVectorComponent(vec3.X, minDecimals, minChars, padWithZeros,
                                leftJustified);
                            vectorStr += ", " + FormatVectorComponent(vec3.Y, minDecimals, minChars, padWithZeros,
                                leftJustified);
                            vectorStr += ", " + FormatVectorComponent(vec3.Z, minDecimals, minChars, padWithZeros,
                                leftJustified);
                        }
                        else if (values[valueIndex].Obj is Vector4 vec4)
                        {
                            componentCount = 4;
                            vectorStr += FormatVectorComponent(vec4.X, minDecimals, minChars, padWithZeros,
                                leftJustified);
                            vectorStr += ", " + FormatVectorComponent(vec4.Y, minDecimals, minChars, padWithZeros,
                                leftJustified);
                            vectorStr += ", " + FormatVectorComponent(vec4.Z, minDecimals, minChars, padWithZeros,
                                leftJustified);
                            vectorStr += ", " + FormatVectorComponent(vec4.W, minDecimals, minChars, padWithZeros,
                                leftJustified);
                        }
                        else
                        {
                            return "%v requires a vector type (Vector2/3/4)";
                        }

                        vectorStr += ")";
                        formatted.Append(vectorStr);
                        ++valueIndex;
                        inFormat = false;
                        break;

                    case 'f': // Float.
                        if (valueIndex >= values.Count)
                            return "not enough arguments for format string";

                        // The value should be a float or double.
                        if (!(values[valueIndex].Obj is float || values[valueIndex].Obj is double))
                            return "a number is required";

                        double doubleValue = Convert.ToDouble(values[valueIndex].Obj);
                        bool isNegative = double.IsNegative(doubleValue);
                        string floatStr = Math.Abs(doubleValue).ToString("F" + minDecimals);
                        bool isFinite = !double.IsInfinity(doubleValue) && !double.IsNaN(doubleValue);

                        // Pad the decimal places.
                        if (isFinite)
                            floatStr = floatStr.PadRight(
                                floatStr.Length + (minDecimals -
                                                   (floatStr.Split('.').Length > 1
                                                       ? floatStr.Split('.')[1].Length
                                                       : 0)), '0');

                        int initialLen = floatStr.Length;

                        // Determine padding character and apply padding.
                        int floatPadCharsCount = (isNegative || showSign) ? minChars - 1 : minChars;
                        string floatPadChar = (padWithZeros && isFinite) ? "0" : " ";

                        if (leftJustified)
                            floatStr = floatStr.PadRight(floatPadCharsCount, floatPadChar[0]);
                        else
                            floatStr = floatStr.PadLeft(floatPadCharsCount, floatPadChar[0]);

                        // Add sign if needed.
                        if (showSign || isNegative)
                        {
                            string signChar = isNegative ? "-" : "+";
                            if (leftJustified)
                                floatStr = signChar + floatStr;
                            else
                                floatStr = floatStr.Insert(padWithZeros ? 0 : floatStr.Length - initialLen, signChar);
                        }

                        // Append the formatted float.
                        formatted.Append(floatStr);
                        ++valueIndex;
                        inFormat = false;
                        break;

                    case 's': // String.
                        if (valueIndex >= values.Count)
                            return "not enough arguments for format string";

                        // Convert the value to a string.
                        string strValue = values[valueIndex].Obj.ToString();
                        // Apply padding.
                        if (leftJustified)
                            strValue = strValue.PadRight(minChars);
                        else
                            strValue = strValue.PadLeft(minChars);

                        // Append the formatted string.
                        formatted.Append(strValue);
                        ++valueIndex;
                        inFormat = false;
                        break;

                    case 'c': // Character.
                        if (valueIndex >= values.Count)
                            return "not enough arguments for format string";

                        string charValue;

                        // Convert the value to a character.
                        if (values[valueIndex].Obj is int || values[valueIndex].Obj is long)
                        {
                            int intValue = Convert.ToInt32(values[valueIndex].Obj);
                            if (intValue < 0 || intValue >= 0xd800 && intValue <= 0xdfff || intValue > 0x10ffff)
                                return "invalid Unicode character";
                            charValue = char.ConvertFromUtf32(intValue);
                        }
                        else if (values[valueIndex].Obj is string && ((string)values[valueIndex].Obj).Length == 1)
                        {
                            charValue = (string)values[valueIndex].Obj;
                        }
                        else
                        {
                            return "%c requires number or single-character string";
                        }

                        // Apply padding.
                        if (leftJustified)
                            charValue = charValue.PadRight(minChars);
                        else
                            charValue = charValue.PadLeft(minChars);

                        // Append the formatted character.
                        formatted.Append(charValue);
                        ++valueIndex;
                        inFormat = false;
                        break;

                    case '-': // Left justify.
                        leftJustified = true;
                        break;
                    case '+': // Show sign.
                        showSign = true;
                        break;
                    case 'u': // Unsigned.
                        asUnsigned = true;
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        // Parse a numeric format specification.
                        int n = c - '0';
                        if (inDecimals)
                        {
                            minDecimals *= 10;
                            minDecimals += n;
                        }
                        else
                        {
                            if (c == '0' && minChars == 0)
                            {
                                if (leftJustified)
                                {
                                    Console.WriteLine("'0' flag ignored with '-' flag in string format");
                                }
                                else
                                {
                                    padWithZeros = true;
                                }
                            }
                            else
                            {
                                minChars *= 10;
                                minChars += n;
                            }
                        }

                        break;

                    case '.': // Decimal point.
                        if (inDecimals)
                            return "too many decimal points in format";
                        inDecimals = true;
                        minDecimals = 0;
                        break;

                    case '*': // Dynamic width or precision.
                        if (valueIndex >= values.Count)
                            return "not enough arguments for format string";

                        if (!(values[valueIndex].Obj is int || values[valueIndex].Obj is long))
                            return "* wants number or vector";

                        int size = Convert.ToInt32(values[valueIndex].Obj);

                        if (inDecimals)
                            minDecimals = size;
                        else
                            minChars = size;

                        ++valueIndex;
                        break;

                    default:
                        return "unsupported format character";
                }
            }
            else
            {
                if (c == '%')
                {
                    // Start of a new format specification.
                    inFormat = true;
                    minChars = 0;
                    minDecimals = 6;
                    padWithZeros = false;
                    leftJustified = false;
                    showSign = false;
                    inDecimals = false;
                }
                else
                {
                    // Regular character, just append it.
                    formatted.Append(c);
                }
            }
        }

        // Check for incomplete format.
        if (inFormat)
            return "incomplete format";

        // Check if all arguments were used.
        if (valueIndex != values.Count)
            return "not all arguments converted during string formatting";

        // No error, return the formatted string.
        error = false;
        return formatted.ToString();
    }
    
    // Helper function for formatting a vector component.
    private static string FormatVectorComponent(double val, int minDecimals, int minChars, bool padWithZeros, bool leftJustified)
    {
        string numberStr = Math.Abs(val).ToString("F" + minDecimals);
        bool isFinite = !double.IsInfinity(val) && !double.IsNaN(val);

        // Pad decimals.
        if (isFinite)
            numberStr = numberStr.PadRight(numberStr.Length + (minDecimals - (numberStr.Contains('.') ? numberStr.Split('.')[1].Length : 0)), '0');

        int initialLen = numberStr.Length;

        // Padding. Leave room for sign later if required.
        int padCharsCount = val < 0 ? minChars - 1 : minChars;
        string padChar = (padWithZeros && isFinite) ? "0" : " ";
        if (leftJustified)
            numberStr = numberStr.PadRight(padCharsCount, padChar[0]);
        else
            numberStr = numberStr.PadLeft(padCharsCount, padChar[0]);

        // Add sign if needed.
        if (val < 0)
        {
            if (leftJustified)
                numberStr = "-" + numberStr;
            else
                numberStr = numberStr.Insert(padWithZeros ? 0 : numberStr.Length - initialLen, "-");
        }

        return numberStr;
    }
}