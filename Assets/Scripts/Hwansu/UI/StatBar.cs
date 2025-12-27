using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text gaugeText;

    private int currentValue = 0;
    private int maxValue = 100;
    private int maxCount = 0; // MAX 도달 횟수

    // MAX 도달 시 발생하는 이벤트
    public event Action OnMaxReached;

    public void Initialize(int max)
    {
        maxValue = max;
        currentValue = 0;
        maxCount = 0;
        UpdateBar();
    }

    public void Increase(int amount = 1)
    {
        bool wasMax = (currentValue >= maxValue);

        currentValue += amount;

        // MAX 도달 또는 초과 처리
        if (currentValue >= maxValue)
        {
            currentValue = maxValue;

            // 이전에 MAX가 아니었는데 지금 MAX가 된 경우
            if (!wasMax)
            {
                maxCount++;
                OnMaxReached?.Invoke();
            }
        }

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

    public int GetMaxCount()
    {
        return maxCount;
    }

    // MAX 상태 초기화 (게이지는 유지하되 MAX 상태만 해제)
    public void ResetMax()
    {
        // 필요시 사용: 게이지를 0으로 리셋
        currentValue = 0;
        UpdateBar();
    }
}