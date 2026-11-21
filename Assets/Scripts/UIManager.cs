using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Handles the game visuals 
public class UIManager : MonoBehaviour
{
    [SerializeField] private SlotMachineController controller;

    [Header("Buttons")]
    [SerializeField] private Button spinButton;
    [SerializeField] private Button betUpButton;
    [SerializeField] private Button betDownButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text messageText;

    [Header("Win popup")]
    [SerializeField] private GameObject winPopup;
    [SerializeField] private TMP_Text winPopupText;
    [SerializeField] private float popupTime = 2f;

    private void Start()
    {
        spinButton.onClick.AddListener(controller.Spin);
        betUpButton.onClick.AddListener(controller.Credits.IncreaseBet);
        betDownButton.onClick.AddListener(controller.Credits.DecreaseBet);

        controller.OnSpinStarted += HandleSpinStarted;
        controller.OnSpinFinished += HandleSpinFinished;
        controller.Credits.OnBalanceChanged += UpdateBalance;
        controller.Credits.OnBetChanged += UpdateBet;

        // Show starting values
        UpdateBalance(controller.Credits.Balance);
        UpdateBet(controller.Credits.CurrentBet);
        messageText.text = "Press SPIN!";
        winPopup.SetActive(false);
    }

    private void OnDestroy()
    {
        controller.OnSpinStarted -= HandleSpinStarted;
        controller.OnSpinFinished -= HandleSpinFinished;
        controller.Credits.OnBalanceChanged -= UpdateBalance;
        controller.Credits.OnBetChanged -= UpdateBet;
    }

    private void HandleSpinStarted()
    {
        SetButtonsInteractable(false);
        messageText.text = "Good luck!";
        winPopup.SetActive(false);
    }

    private void HandleSpinFinished(int win)
    {
        SetButtonsInteractable(true);

        if (win > 0)
        {
            messageText.text = "You won " + win + "!";
            StartCoroutine(ShowWinPopup(win));
        }
        else if (!controller.Credits.CanSpin)
        {
            messageText.text = "Not enough coins. Lower your bet.";
        }
        else
        {
            messageText.text = "Try again!";
        }
    }

    private IEnumerator ShowWinPopup(int win)
    {
        winPopupText.text = "WIN\n" + win;
        winPopup.SetActive(true);
        yield return new WaitForSeconds(popupTime);
        winPopup.SetActive(false);
    }

    private void UpdateBalance(int balance)
    {
        balanceText.text = "Balance: " + balance;
        RefreshSpinButton();
    }

    private void UpdateBet(int bet)
    {
        betText.text = "Bet: " + bet;
        RefreshSpinButton();
    }

    private void SetButtonsInteractable(bool value)
    {
        betUpButton.interactable = value;
        betDownButton.interactable = value;
        spinButton.interactable = value && controller.Credits.CanSpin;
    }

    // Spin is only clickable if not spinning and we can afford the bet
    private void RefreshSpinButton()
    {
        spinButton.interactable = !controller.IsSpinning && controller.Credits.CanSpin;
    }
}