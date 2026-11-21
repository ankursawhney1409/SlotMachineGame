using System.Collections.Generic;

// Checks the spin result and calculates the win.
// Rule: all reels show the same symbol = bet x payoutMultiplier.
public static class WinEvaluator
{
    public static int CalculateWin(List<SymbolData> results, int bet)
    {
        SymbolData first = results[0];

        // If any reel is different from the first one, no win
        foreach (SymbolData s in results)
        {
            if (s != first)
                return 0;
        }

        // All the same
        return bet * first.payoutMultiplier;
    }
}