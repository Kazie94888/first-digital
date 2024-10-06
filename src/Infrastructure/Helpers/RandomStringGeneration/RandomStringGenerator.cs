using System.Security.Cryptography;
using System.Text;

namespace SmartCoinOS.Infrastructure.Helpers.RandomStringGeneration;

public static class RandomGenerator
{
    private static readonly char[][] _alphabet =
    [
        Enumerable.Range('a', 'z' - 'a' + 1).Select(x => (char)x).ToArray(),
        Enumerable.Range('A', 'Z' - 'A' + 1).Select(x => (char)x).ToArray(),
        Enumerable.Range('0', '9' - '0' + 1).Select(x => (char)x).ToArray(),
        [
            '@',
            '#',
            '$',
            '%',
            '^',
            '&',
            '*',
            '-',
            '_',
            '+',
            '=',
            '[',
            ']',
            '{',
            '}',
            '|',
            '.',
            ',',
            '!',
            '?'
        ]
    ];
    public static string GenerateString(GeneratorConfig config = default)
    {
        if (config == default)
            config = new GeneratorConfig();

        var builder = new StringBuilder();
        var availableGroups = new List<char[]>();
        if (config.LowerCase)
            availableGroups.Add(_alphabet[0]);
        if (config.UpperCase)
            availableGroups.Add(_alphabet[1]);
        if (config.Digits)
            availableGroups.Add(_alphabet[2]);
        if (config.Characters)
            availableGroups.Add(_alphabet[3]);

        foreach (var group in availableGroups)
            builder.Append(group[RandomNumberGenerator.GetInt32(group.Length)]);

        while (builder.Length != config.Length)
        {
            var group = availableGroups[RandomNumberGenerator.GetInt32(availableGroups.Count)];
            builder.Append(group[RandomNumberGenerator.GetInt32(group.Length)]);
        }

        var array = builder.ToString().ToCharArray();
        Shuffle(array);
        return new string(array);

        static void Shuffle<T>(T[] arr)
        {
            var n = arr.Length;
            while (n > 1)
            {
                var k = RandomNumberGenerator.GetInt32(n--);
                (arr[n], arr[k]) = (arr[k], arr[n]);
            }
        }
    }
}