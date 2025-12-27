using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementChecker : Singleton<AchievementChecker>
{
    [Header("StatBar References")]
    [SerializeField] private List<StatBar> statBars = new List<StatBar>(); // 여러 StatBar를 추가 가능

    [Header("Ending UI References")]
    [SerializeField] private GameObject successEndingImage; // 성공 엔딩 이미지 (18개 달성)
    [SerializeField] private GameObject failEndingImage;    // 실패 엔딩 이미지 (18개 미달)

    [Header("Settings")]
    [SerializeField] private int targetCount = 18; // 목표 카운트

    private bool isChecked = false; // 한 번만 체크하도록

    void Start()
    {
        // UI 초기 상태 설정
        if (successEndingImage != null) successEndingImage.SetActive(false);
        if (failEndingImage != null) failEndingImage.SetActive(false);
    }

    // 7일차 종료 시 엔딩 체크 (DateManager에서 호출)
    public void CheckEnding()
    {
        if (isChecked)
            return;

        int totalCount = GetTotalCount();

        if (totalCount >= targetCount)
        {
            ShowSuccessEnding();
        }
        else
        {
            ShowFailEnding();
        }

        isChecked = true;
    }

    private int GetTotalCount()
    {
        int statBarCount = 0;

        // 모든 StatBar의 maxCount 합산
        foreach (var statBar in statBars)
        {
            if (statBar != null)
            {
                statBarCount += statBar.GetMaxCount();
            }
        }

        // PurchaseButton의 구매 횟수
        int purchaseCount = PurchaseButton.GetTotalPurchaseCount();

        return statBarCount + purchaseCount;
    }

    private void ShowSuccessEnding()
    {
        if (successEndingImage != null)
            successEndingImage.SetActive(true);

        if (failEndingImage != null)
            failEndingImage.SetActive(false);
    }

    private void ShowFailEnding()
    {
        if (successEndingImage != null)
            successEndingImage.SetActive(false);

        if (failEndingImage != null)
            failEndingImage.SetActive(true);
    }

    // 현재 총 카운트 확인용 (디버그/UI 표시용)
    public int GetCurrentTotalCount()
    {
        return GetTotalCount();
    }

    // 목표까지 남은 카운트
    public int GetRemainingCount()
    {
        return Mathf.Max(0, targetCount - GetTotalCount());
    }

    // 리셋 (재도전 시 사용)
    public void ResetChecker()
    {
        isChecked = false;
        PurchaseButton.ResetTotalPurchaseCount();

        if (successEndingImage != null)
            successEndingImage.SetActive(false);

        if (failEndingImage != null)
            failEndingImage.SetActive(false);
    }
}