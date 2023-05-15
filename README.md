# Comparison between decimal floating point and binary floating point calculations

In the first test, we start with two different numbers, a minuend and a subtrahend with an initial difference
of 10,000. We then repeatedly add `.93` to the minuend 30.7 million times and `.99` to the subtrahend 28.8
million times, so that both numbers should still have a difference 10,000, and subtract the subtrahend from
the minuend. The result should be equal to the initial difference (10,000),
however the `double` amount is off by a little less than `0.01 `.

```
Expected Difference: 10000.0
Decimal Difference:  10000.00
Double Difference:   10000.008706595749
```

We then treat this quantity as the principal of an account and compound the interest monthly over
a period of 12 months. This results in a final amount with a difference over `0.01`.

```
Decimal Principal:  12062.186320446314059477022641
Double Principal:   12062.196822503693
```

Next, we generate 100,000,000 random decimal numbers between 100,000 and 100,000,000 and sum them.

We have three different results here:
- Full: full-precision decimal calculations with inputs not rounded - this is the "ideal" number.
- 10-Digit: full-precision decimal calculations with inputs rounded to 10 decimal places - this is what KE20 uses.
- double: double-precision binary calculations with inputs first rounded to 10 decimal places and then converted - this is what JavaScript calculations would look like.

We then round the results to 10 decimal places, as that's how we would store them in the database.

The sums of the full-precision decimal and the 10-digit decimal differ after 22 digits, or past 6 digits after the decimal point.
The sums of the double and the 10-digit decimal diverge after just 12 digits, or past 5 digits *before* the decimal point.

```
Creating 10000000 random numbers in [100000, 100000000)...
Sums:
Full:     5004860625374122.6282311184
10-digit: 5004860625374122.6282310953
double:   5004860625372520.0000000000
```

We now use these same generated numbers to calculate the mean with 3 different algorithms:
- Cumulative - adds each number to the mean individually, useful if the total count of numbers is unknown
- Partial - divides each number by the total count first, then adds it to the mean
- Simple - typical averaging method, add up all the numbers then divide by the total count

While both the full precision and 10-digit decimal results are identical across all algorithms, double begins
to diverge after 12 total digits, or 4 digits after the decimal point. Additionally, double is not consistent
between different algorithms, the result varying by up to ~.000045 depending on the algorithm used. Decimal
calculations, however, are consistent no matter which algorithm is used.

```
Cumulative Means:
Full:     50048606.2537412263
10-digit: 50048606.2537412263
double:   50048606.2537163050

Partial Means:
Full:     50048606.2537412263
10-digit: 50048606.2537412263
double:   50048606.2537610530

Simple Means:
Full:     50048606.2537412263
10-digit: 50048606.2537412263
double:   50048606.2537252010
```
