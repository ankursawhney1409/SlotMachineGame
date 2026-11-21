using System;

public class CreditManager
{
    public int Balance { get; private set; }

    private readonly int[] betOptions;
    private int betIndex = 0;

    public int CurrentBet => betOptions[betIndex];
    public bool CanSpin => Balance >= CurrentBet;
    public event Action<int> OnBalanceChanged;
    public event Action<int> OnBetChanged;

    public CreditManager(int startingBalance, int[] betOptions)
    {
        Balance = startingBalance;
        this.betOptions = betOptions;
    }

    // Taking bet out before spinning. Returns false if therez not enough money.
    public bool TryPlaceBet()
    {
        if (!CanSpin)
            return false;

        Balance -= CurrentBet;
        OnBalanceChanged?.Invoke(Balance);
        return true;
    }

    public void AddWin(int amount)
    {
        Balance += amount;
        OnBalanceChanged?.Invoke(Balance);
    }

    public void IncreaseBet()
    {
        if (betIndex < betOptions.Length - 1)
            betIndex++;
        OnBetChanged?.Invoke(CurrentBet);
    }

    public void DecreaseBet()
    {
        if (betIndex > 0)
            betIndex--;
        OnBetChanged?.Invoke(CurrentBet);
    }
}