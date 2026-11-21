using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Lets the player pull the lever to spin.
public class Lever : MonoBehaviour
{
    [SerializeField] private SlotMachineController controller;
    [SerializeField] private Button leverButton;      // invisible click area on the lever
    [SerializeField] private GameObject leverUp;      
    [SerializeField] private GameObject leverDown;    
    [SerializeField] private float pulledTime = 0.4f;

    private void Start()
    {
        leverButton.onClick.AddListener(controller.Spin);
        controller.OnSpinStarted += PlayPull;   
        leverDown.SetActive(false);
    }

    private void OnDestroy()
    {
        controller.OnSpinStarted -= PlayPull;
    }

    private void PlayPull()
    {
        StartCoroutine(PullRoutine());
    }

    private IEnumerator PullRoutine()
    {
        leverUp.SetActive(false);
        leverDown.SetActive(true);
        yield return new WaitForSeconds(pulledTime);
        leverDown.SetActive(false);
        leverUp.SetActive(true);
    }
}