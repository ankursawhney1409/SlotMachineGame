using System.Collections.Generic;

// Picks a random symbol for one reel, based on each symbol's weight.
public class RandomNumberGenerator
{
    private readonly System.Random random = new System.Random();

    public SymbolData PickSymbol(List<SymbolData> symbols)
    {
        // Add up all weights
        int totalWeight = 0;
        foreach (SymbolData s in symbols)
            totalWeight += s.weight;

        // Random number from 0 to 99
        int roll = random.Next(totalWeight);

        // Find which symbol's range the roll falls in
        // Seven: 0-4, Bar: 5-19, Bell: 20-49, Cherry: 50-99
        int rangeEnd = 0;
        foreach (SymbolData s in symbols)
        {
            rangeEnd += s.weight;
            if (roll < rangeEnd)
                return s;
        }

        // Safety fallback (never reached)
        return symbols[symbols.Count - 1];
    }
}