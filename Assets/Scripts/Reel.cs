using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// reel scrolls symbol images down, then stops on the chosen result.
public class Reel : MonoBehaviour
{
    [SerializeField] private Image[] symbolImages;      
    [SerializeField] private float symbolHeight = 150f;
    [SerializeField] private float maxSpeed = 2500f;   
    [SerializeField] private float accelerationTime = 0.25f;
    [SerializeField] private float bounceAmount = 25f;
    [SerializeField] private float bounceTime = 0.15f;

    public bool IsSpinning { get; private set; }

    private List<SymbolData> symbols;
    private RandomNumberGenerator rng;

    // Called once at the start by SlotMachineController
    public void Init(List<SymbolData> symbols, RandomNumberGenerator rng)
    {
        this.symbols = symbols;
        this.rng = rng;

        // Stack images one below center, one in center, two above
        for (int i = 0; i < symbolImages.Length; i++)
        {
            symbolImages[i].rectTransform.anchoredPosition = new Vector2(0, (i - 1) * symbolHeight);
            SetRandomSprite(symbolImages[i]);
        }
    }

    public IEnumerator Spin(SymbolData result, float spinDuration)
    {
        IsSpinning = true;

        // Accelerate
        float t = 0;
        while (t < accelerationTime)
        {
            t += Time.deltaTime;
            float speed = maxSpeed * (t / accelerationTime);
            MoveImages(speed * Time.deltaTime);
            yield return null;  
        }

        // Full speed
        t = 0;
        while (t < spinDuration)
        {
            t += Time.deltaTime;
            MoveImages(maxSpeed * Time.deltaTime);
            yield return null;
        }

        // Slow down then put the result on the top image, bring it to the center
        Image target = GetTopImage();
        target.sprite = result.sprite;

        float distance = target.rectTransform.anchoredPosition.y;
        float stopTime = 3f * distance / maxSpeed;   // matches current speed, so no sudden jerk
        float moved = 0;
        t = 0;
        while (t < stopTime)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / stopTime);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);   // fast at first, slow at the end
            float goal = distance * eased;
            MoveImages(goal - moved);
            moved = goal;
            yield return null;
        }

        //  Snap exactly to center
        MoveImages(target.rectTransform.anchoredPosition.y);

        //  Small bounce: dip down a bit, come back
        t = 0;
        float previous = 0;
        while (t < bounceTime)
        {
            t += Time.deltaTime;
            float offset = bounceAmount * Mathf.Sin(Mathf.Clamp01(t / bounceTime) * Mathf.PI);
            MoveImages(offset - previous);
            previous = offset;
            yield return null;
        }
        MoveImages(target.rectTransform.anchoredPosition.y);   // final snap

        IsSpinning = false;
    }

    // Moves all images down. An image that leaves the bottom jumps to the top.
    private void MoveImages(float amount)
    {
        float bottomLimit = -1.5f * symbolHeight;

        foreach (Image img in symbolImages)
        {
            Vector2 pos = img.rectTransform.anchoredPosition;
            pos.y -= amount;

            if (pos.y < bottomLimit)
            {
                pos.y += symbolImages.Length * symbolHeight;
                SetRandomSprite(img);
            }

            img.rectTransform.anchoredPosition = pos;
        }
    }

    private Image GetTopImage()
    {
        Image top = symbolImages[0];
        foreach (Image img in symbolImages)
        {
            if (img.rectTransform.anchoredPosition.y > top.rectTransform.anchoredPosition.y)
                top = img;
        }
        return top;
    }

    private void SetRandomSprite(Image img)
    {
        img.sprite = rng.PickSymbol(symbols).sprite;
    }
}