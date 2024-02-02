using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Sync.Domain.Services;

public class HashGenerator
{
    public static IEnumerable<string> CreateHash(string title, string author, IEnumerable<string> extensions)
    {
        var fileName = GetValidFileName(author + " - " + title);
        return extensions.Select(ext => CreateMD5(fileName + "." + ext)).ToList();
    }

    private static string GetValidFileName(string input)
    {
        if (input.Last() == '.')
            input = input[..^1] + '_';
        input = input.Replace('/', '_').Replace(':', '_').Trim('\0');

        // todo: check config_unicode_filename feature in Calibre-web configuration and unidecode if needed
        // config_unicode_filename is off by default in Calibre-web, 
        // so non unicode won't be replaced.

        input = Regex.Replace(input, @"[^\u0000-\u007F]+", string.Empty).Trim();

        if (string.IsNullOrEmpty(input))
            throw new ArgumentOutOfRangeException(nameof(input), "Input contains no valid characters");

        return input;
    }

    private static string CreateMD5(string input)
    {
        var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}