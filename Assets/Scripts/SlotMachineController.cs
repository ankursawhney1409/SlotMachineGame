using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script runs one full spin from start to finish.
public class SlotMachineController : MonoBehaviour
{
    [Header("Symbols and reels")]
    [SerializeField] private List<SymbolData> symbols;
    [SerializeField] private Reel[] reels;

    [Header("Money")]
    [SerializeField] private int startingBalance = 1000;
    [SerializeField] private int[] betOptions = { 10, 50, 100 };

    [Header("Timing")]
    [SerializeField] private float baseSpinTime = 1f;   // first reel spins this long
    [SerializeField] private float stopDelay = 0.4f;    // each next reel spins this much longer

    public CreditManager Credits { get; private set; }
    public bool IsSpinning { get; private set; }

    public event Action OnSpinStarted;
    public event Action<int> OnSpinFinished;   // sends the win amount (0 = no win)

    private RandomNumberGenerator rng;

    private void Awake()
    {
        rng = new RandomNumberGenerator();
        Credits = new CreditManager(startingBalance, betOptions);

        foreach (Reel reel in reels)
            reel.Init(symbols, rng);
    }

    // Called by the Spin button
    public void Spin()
    {
        if (IsSpinning)
            return;

        int bet = Credits.CurrentBet;
        if (!Credits.TryPlaceBet())
            return;   // not enough money

        StartCoroutine(SpinRoutine(bet));
    }

    private IEnumerator SpinRoutine(int bet)
    {
        IsSpinning = true;
        OnSpinStarted?.Invoke();

        // Decide the results first
        List<SymbolData> results = new List<SymbolData>();
        for (int i = 0; i < reels.Length; i++)
            results.Add(rng.PickSymbol(symbols));

        // Start all reels
        for (int i = 0; i < reels.Length; i++)
            StartCoroutine(reels[i].Spin(results[i], baseSpinTime + i * stopDelay));

        //Wait until every reel has stopped
        while (AnyReelSpinning())
            yield return null;

        // Check for a win and pay it
        int win = WinEvaluator.CalculateWin(results, bet);
        if (win > 0)
            Credits.AddWin(win);

        IsSpinning = false;
        OnSpinFinished?.Invoke(win);
    }

    private bool AnyReelSpinning()
    {
        foreach (Reel reel in reels)
        {
            if (reel.IsSpinning)
                return true;
        }
        return false;
    }
}