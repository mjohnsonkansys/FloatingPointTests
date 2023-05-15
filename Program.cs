using RandN.Distributions;
using RandN.Rngs;
using System.Diagnostics;

internal class Program
{
    // 10 digits after the decimal point, 6 before
    private static readonly Uniform.Decimal UniformDist = Uniform.Decimal.Create(100_000m, 100_000_000m);

    private static void Main()
    {
        decimal decimalMinuend = 10_000m;
        decimal decimalSubtrahend = 0m;
        double doubleMinuend = 10_000d;
        double doubleSubtrahend = 0d;
        for (int i = 0; i < 30_690_000; i++)
        {
            decimalMinuend += .93m;
            doubleMinuend += .93d;
            if (i % 33 != 0 && i % 32 != 0)
            {
                decimalSubtrahend += .99m;
                doubleSubtrahend += .99d;
            }
        }

        var decimalDiff = decimalMinuend - decimalSubtrahend;
        var doubleDiff = doubleMinuend - doubleSubtrahend;

        Console.WriteLine($"Expected Difference: 10000.0");
        Console.WriteLine($"Decimal Difference:  {decimalDiff}");
        Console.WriteLine($"Double Difference:   {doubleDiff}");

        var interest = .0375m;
        var decimalPrincipal = decimalDiff;
        var doublePrincipal = doubleDiff;
        for (int i = 0; i < 12 * 5; i++)
        {
            decimalPrincipal *= 1m + interest / 12;
            doublePrincipal *= 1d + (double)interest / 12;
        }

        Console.WriteLine($"Decimal Principal:  {decimalPrincipal}");
        Console.WriteLine($"Double Principal:   {doublePrincipal}");

        var rng = ChaCha.Create(new ChaCha.Seed(new[] { 1u, 2u, 3u, 4u, 5u, 6u, 7u, 9u }, 1234567891));
        

        const int meanCount = 100_000_000;
        var quantities = new List<decimal>(meanCount);
        Console.WriteLine($"Creating {meanCount} random numbers in [100000, 100000000)...");
        for (int i = 0; i < meanCount; i++)
            quantities.Add(UniformDist.Sample(rng));

        Console.WriteLine($"Sums:");
        Output(Sums(quantities));

        var sw = Stopwatch.StartNew();

        var cumulativeMeans = CumulativeMean(quantities);
        Console.WriteLine("Cumulative Means:");
        Output(cumulativeMeans, sw.Elapsed);
        sw.Restart();

        Console.WriteLine("Partial Means:");
        Output(PartialMean(quantities), sw.Elapsed);
        sw.Restart();

        Console.WriteLine("Simple Means:");
        Output(SimpleMean(quantities), sw.Elapsed);
        sw.Stop();
    }

    private static Result Sums(IEnumerable<decimal> quantities)
    {
        // Full precision decimal
        var fullSum = 0m;
        // Decimal with inputs rounded to 10 digits
        var tenDigitSum = 0m;
        // Full precision double
        var doubleSum = 0d;

        foreach (var full in quantities)
        {
            var dec = decimal.Round(full, 10);
            var doub = (double)full;

            // Cumulative mean
            fullSum += full;
            tenDigitSum += dec;
            doubleSum += doub;
        }

        return new Result(fullSum, tenDigitSum, doubleSum);
    }

    private static Result CumulativeMean(IEnumerable<decimal> quantities)
    {
        // Full precision decimal
        var fullMean = 0m;
        // Decimal with inputs rounded to 10 digits
        var tenDigitMean = 0m;
        // Full precision double
        var doubleMean = 0d;

        int i = 1;
        foreach(var full in quantities)
        {
            var dec = decimal.Round(full, 10);
            var doub = (double)dec;

            // Cumulative mean
            fullMean = i == 1 ? full : fullMean * (i - 1) / i + full / i;
            tenDigitMean = i == 1 ? dec : tenDigitMean * (i - 1) / i + dec / i;
            doubleMean = i == 1 ? doub : doubleMean * (i - 1) / i + doub / i;
            i++;
        }

        return new Result(fullMean, tenDigitMean, doubleMean);
    }

    private static Result PartialMean(IList<decimal> quantities)
    {
        // Full precision decimal
        var fullMean = 0m;
        // Decimal with inputs rounded to 10 digits
        var tenDigitMean = 0m;
        // Full precision double
        var doubleMean = 0d;

        foreach (var full in quantities)
        {
            var dec = decimal.Round(full, 10);
            var doub = (double)dec;

            fullMean += full / quantities.Count;
            tenDigitMean += dec / quantities.Count;
            doubleMean += doub / quantities.Count;
        }

        return new Result(fullMean, tenDigitMean, doubleMean);
    }

    private static Result SimpleMean(IList<decimal> quantities)
    {
        // Full precision decimal
        var fullSum = 0m;
        // Decimal with inputs rounded to 10 digits
        var tenDigitSum = 0m;
        // Full precision double
        var doubleSum = 0d;

        foreach (var full in quantities)
        {
            var dec = decimal.Round(full, 10);
            var doub = (double)dec;

            fullSum += full;
            tenDigitSum += dec;
            doubleSum += doub;
        }

        return new Result(fullSum / quantities.Count, tenDigitSum / quantities.Count, doubleSum / quantities.Count);
    }

    private static void Output(Result result, TimeSpan? elapsed = null)
    {
        // Round to 10 digits to match the storage datatype
        Console.WriteLine($"Full:     {decimal.Round(result.Full, 10):F10}");
        Console.WriteLine($"10-digit: {decimal.Round(result.TenDigit, 10):F10}");
        var binary = decimal.Parse(result.Binary.ToString("G17"));
        Console.WriteLine($"double:   {decimal.Round(binary, 10):F10}");
    }

    private readonly record struct Result(decimal Full, decimal TenDigit, double Binary)
    {
        /// <summary>
        /// The result of 96-bit decimal floating point calculations with full-precision inputs.
        /// This is the 
        /// </summary>
        public decimal Full { get; } = Full;

        /// <summary>
        /// The result of 96-bit decimal floating point calculations with inputs limited to 10 digits.
        /// </summary>
        public decimal TenDigit { get; } = TenDigit;

        /// <summary>
        /// The result of 64-bit binary floating point calculations with full-precision inputs.
        /// </summary>
        public double Binary { get; } = Binary;
    }
}
