using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text gaugeText;

    private int currentValue = 0;
    private int maxValue = 100;

    public void Initialize(int max)
    {
        maxValue = max;
        currentValue = 0;
        UpdateBar();
    }

    public void Increase(int amount = 1)
    {
        currentValue += amount;
        if (currentValue > maxValue) currentValue = maxValue;
        UpdateBar();
    }

    private void UpdateBar()
    {
        fillImage.fillAmount = (float)currentValue / maxValue;

        if (currentValue >= maxValue)
        {
            gaugeText.gameObject.SetActive(true);
            gaugeText.text = "MAX";
        }
        else
        {
            gaugeText.gameObject.SetActive(false);
        }
    }

    public int GetValue()
    {
        return currentValue;
    }
}
