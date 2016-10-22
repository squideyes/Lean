using System;

namespace Legomatic.Trading
{
    public static class DecimalsExtenders
    {
        internal static decimal RateToPips(this decimal rate, Decimals decimals)
        {
            if (decimals == Decimals.Five)
                return Math.Round(rate * 10000.0m, 5);
            else
                return Math.Round(rate * 100.0m, 3);
        }

        internal static decimal PipsToRate(this decimal pips, Decimals decimals)
        {
            if (decimals == Decimals.Five)
                return Math.Round(pips / 10000.0m, 5);
            else
                return Math.Round(pips / 100.0m, 3);
        }
    }
}
