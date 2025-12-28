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
    private bool hasReachedMax = false; // ✅ MAX 도달 여부 (한 번만 카운팅)

    // MAX 도달 시 발생하는 이벤트
    public event Action OnMaxReached;

    public void Initialize(int max)
    {
        maxValue = max;
        currentValue = 0;
        hasReachedMax = false;
        UpdateBar();
        Debug.Log($"[StatBar] Initialize - maxValue: {maxValue}");
    }

    public void Increase(int amount = 1)
    {
        // 이미 MAX 도달했으면 더 이상 증가하지 않음
        if (hasReachedMax)
        {
            Debug.Log("[StatBar] 이미 MAX 도달 완료");
            return;
        }

        currentValue += amount;

        // MAX 도달 처리
        if (currentValue >= maxValue)
        {
            currentValue = maxValue;
            hasReachedMax = true; // ✅ MAX 도달 플래그 설정

            Debug.Log($"[StatBar] MAX 도달! (한 번만 카운팅됨)");
            OnMaxReached?.Invoke();
        }

        UpdateBar();
    }

    private void UpdateBar()
    {
        if (fillImage != null)
            fillImage.fillAmount = (float)currentValue / maxValue;

        if (gaugeText == null)
            return;

        if (currentValue >= maxValue)
        {
            gaugeText.text = "MAX";
        }
        else
        {
            gaugeText.text = $"Lv.UP";
        }
    }

    public int GetValue()
    {
        return currentValue;
    }

    // ✅ MAX 도달 여부 반환 (AchievementChecker에서 사용)
    public int GetMaxCount()
    {
        return hasReachedMax ? 1 : 0;
    }

    // 완전 초기화 (게임 재시작 시)
    public void ResetMax()
    {
        currentValue = 0;
        hasReachedMax = false;
        UpdateBar();
        Debug.Log("[StatBar] 완전 리셋 완료");
    }

    public bool IsMax()
    {
        return hasReachedMax;
    }
}