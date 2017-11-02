using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public static class ExtensionMethods
{
    #region Indices of

    #region Indices of character

    /// <summary>
    /// Reports all the zero-based indices of all the occurrences of the specified character
    /// in the current System.String object using an ordinal comparision type.
    /// </summary>
    /// <param name="value">The character to seek.</param>
    /// <returns>The zero-based indices positions of the value parameter if that string is found,
    /// or none if it is not.</returns>
    public static IEnumerable<int> IndicesOf(this string str, char value)
    {
        for (int i = 0; i < str.Length; i++)
            if (str[i] == value)
                yield return i;
    }

    #endregion

    #region Indices of string

    /// <summary>
    /// Reports all the zero-based indices of all the occurrences of the specified string
    /// in the current System.String object using an ordinal comparision type.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <returns>The zero-based indices positions of the value parameter if that string is found,
    /// or none if it is not.</returns>
    public static IEnumerable<int> IndicesOf(this string str, string value)
        => str.IndicesOf(value, StringComparison.Ordinal);

    /// <summary>
    /// Reports all the zero-based indices of all the occurrences of the specified string
    /// in the current System.String object. Parameters specify the type of search to use for the specified string.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="comparisionType">One of the enumeration values that specifies the rules for the search.</param>
    /// <returns>The zero-based indices positions of the value parameter if that string is found,
    /// or none if it is not.</returns>
    public static IEnumerable<int> IndicesOf(this string str, string value, StringComparison comparisionType)
    {
        if (!string.IsNullOrEmpty(value))
        {
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, comparisionType);
                if (index == -1)
                    break;
                yield return index;
            }
        }
    }

    #endregion

    #endregion

    #region Split

    /// <summary>
    /// Returns a string array that contains the substrings in this string that are delimited
    /// by a specified string
    /// </summary>
    /// <param name="separator">An array of single-character strings that delimit the substrings in this string,
    /// an empty array that contains no delimiters, or null.</param>
    /// <returns>An array whose elements contain the substrings in this string that are delimited
    /// by the string separator.</returns>
    public static string[] Split(this string str, string separator)
    {
        return str.Split(separator, StringSplitOptions.None);
    }

    /// <summary>
    /// Returns a string array that contains the substrings in this string that are delimited
    /// by a specified string. A parameter specifies whether to return empty array elements.
    /// </summary>
    /// <param name="separator">An array of single-character strings that delimit the substrings in this string,
    /// an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from
    /// the array returned; or System.StringSplitOptions.None to include empty array
    /// elements in the array returned.</param>
    /// <returns>An array whose elements contain the substrings in this string that are delimited
    /// by the string separator.</returns>
    public static string[] Split(this string str, string separator, StringSplitOptions options)
    {
        return str.Split(new string[] { separator }, options);
    }

    #endregion

    #region Sanitize

    public static string SanitizeUrl(this string url)
    {
        while (url.Contains("%"))
        {
            var hex = url.Substring(url.IndexOf('%') + 1, 2);
            var bhx = byte.Parse(hex, NumberStyles.HexNumber);
            var chr = (char)bhx;
            var str = chr.ToString();
            url = url.Replace("%" + hex, str);
        }
        return url;
    }

    public static string SanitizeFileName(this string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var invalid in invalidChars)
            if (fileName.Contains(invalid))
                fileName = fileName.Replace(invalid, '_');

        return fileName;
    }

    #endregion
}
