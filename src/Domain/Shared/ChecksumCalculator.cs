using System.Text;

namespace SmartCoinOS.Domain.Shared;
public static class ChecksumCalculator
{
    /// <summary>
    /// Produces a single digit to act as a check on any given string of length more then 2.
    /// 
    /// Based on the Luhn algorithm https://en.wikipedia.org/wiki/Luhn_algorithm.
    /// 
    /// Algorithm:
    /// 1. Lay the given string into an array of numbers by converting the characters to their respective UTF-8 index.
    /// 2. From right to the left, double the value of every 2nd cell.
    /// 3. Results from the steps above will always be more then single digit. Add digits together until the result is single digit (0-9).
    /// 4. Sum up all values of the array (original given numbers, and replacements at the 3rd step.)
    /// 5. Modulus by 10 the result above. Use this value as the check digit.
    /// </summary>
    /// <param name="draftNumber">A random string, without previously calculated check digit.</param>
    /// <returns>Number to be used as check digit.</returns>
    public static int CalculateCheckDigit(string draftNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(draftNumber);

        if (draftNumber.Length < 2)
            throw new ArgumentOutOfRangeException(nameof(draftNumber));

        int[] utf8Number = Encoding.UTF8.GetBytes(draftNumber)
                                    .Select(x => (int)x)
                                    .ToArray();

        for (int i = utf8Number.Length - 2; i >= 0; i -= 2)
        {
            var choosenCell = utf8Number[i];

            choosenCell *= 2;

            var singleDigitSum = SumOfDigitUntilSingle(choosenCell);

            utf8Number[i] = singleDigitSum;
        }

        var sumOfGivenNumber = utf8Number.Sum();

        var checkDigit = sumOfGivenNumber % 10;
        return checkDigit;
    }

    private static int SumOfDigitUntilSingle(int givenNumber)
    {
        if (givenNumber == 0)
            return 0;

        if (givenNumber % 9 == 0)
            return 9;
        else
            return givenNumber % 9;
    }
}
