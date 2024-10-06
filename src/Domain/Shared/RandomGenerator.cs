using System.Security.Cryptography;
using System.Text;

namespace SmartCoinOS.Domain.Shared;

internal static class RandomGenerator
{
    private static readonly char[][] _alphabet =
    [
        Enumerable.Range('a', 'z' - 'a' + 1).Select(x => (char)x).ToArray(),
        Enumerable.Range('A', 'Z' - 'A' + 1).Select(x => (char)x).ToArray(),
        Enumerable.Range('0', '9' - '0' + 1).Select(x => (char)x).ToArray()
    ];

    internal static string GenerateRandomString(int length, bool lowerCase, bool upperCase, bool digits, bool requireDiversity)
    {
        var builder = new StringBuilder();
        var availableGroups = new List<char[]>();

        if (lowerCase)
            availableGroups.Add(_alphabet[0]);
        if (upperCase)
            availableGroups.Add(_alphabet[1]);
        if (digits)
            availableGroups.Add(_alphabet[2]);

        if (requireDiversity)
        {
            foreach (var group in availableGroups)
            {
                builder.Append(group[RandomNumberGenerator.GetInt32(group.Length)]);
            }
        }

        while (builder.Length != length)
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