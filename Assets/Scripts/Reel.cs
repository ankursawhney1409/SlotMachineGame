using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reel : MonoBehaviour
{
    [SerializeField] private Image[] symbolImages;
    [SerializeField] private float symbolHeight = 100f;
    [SerializeField] private float maxSpeed = 2500f;
    [SerializeField] private float accelerationTime = 0.25f;

    [Header("Landing bounce (damped spring)")]
    [SerializeField] private float overshoot = 25f;        // how far it slides past the center
    [SerializeField] private float bounceSpeed = 25f;      // how fast it wobbles
    [SerializeField] private float bounceDamping = 7f;     // how fast the wobble dies out
    [SerializeField] private float bounceTime = 0.6f;

    public bool IsSpinning { get; private set; }

    private List<SymbolData> symbols;
    private RandomNumberGenerator rng;

    public void Init(List<SymbolData> symbols, RandomNumberGenerator rng)
    {
        this.symbols = symbols;
        this.rng = rng;

        for (int i = 0; i < symbolImages.Length; i++)
        {
            symbolImages[i].rectTransform.anchoredPosition = new Vector2(0, (i - 1) * symbolHeight);
            SetRandomSprite(symbolImages[i]);
        }
    }

    public IEnumerator Spin(SymbolData result, float spinDuration)
    {
        IsSpinning = true;

        //Accelerate
        float t = 0;
        while (t < accelerationTime)
        {
            t += Time.deltaTime;
            MoveImages(maxSpeed * (t / accelerationTime) * Time.deltaTime);
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

        // Slow down, sliding slightly PAST the center
        Image target = GetTopImage();
        target.sprite = result.sprite;

        float distance = target.rectTransform.anchoredPosition.y + overshoot;
        float stopTime = 3f * distance / maxSpeed;
        float moved = 0;
        t = 0;
        while (t < stopTime)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / stopTime);
            float goal = distance * (1f - Mathf.Pow(1f - progress, 3f));
            MoveImages(goal - moved);
            moved = goal;
            yield return null;
        }

        // Spring back: each bounce smaller than the last
        t = 0;
        while (t < bounceTime)
        {
            t += Time.deltaTime;
            float wantedY = -overshoot * Mathf.Exp(-bounceDamping * t) * Mathf.Cos(bounceSpeed * t);
            MoveImages(target.rectTransform.anchoredPosition.y - wantedY);
            yield return null;
        }

        // Final snap exactly to center
        MoveImages(target.rectTransform.anchoredPosition.y);

        IsSpinning = false;
    }

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