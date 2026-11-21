using System.Collections.Generic;

// Picks a symbol at random, using each symbol's weight.
public class RandomNumberGenerator
{
    private readonly System.Random random = new System.Random();

    public SymbolData PickSymbol(List<SymbolData> symbols)
    {
        // Add up all weights of the symbols to make a 100
        int totalWeight = 0;
        foreach (SymbolData s in symbols)
            totalWeight += s.weight;

        int roll = random.Next(totalWeight);

        // Walk through symbols until the roll falls in one's piece
        foreach (SymbolData s in symbols)
        {
            if (roll < s.weight)
                return s;
            roll -= s.weight;
        }

        return symbols[symbols.Count - 1]; // safety fallback, never reached
    }
}