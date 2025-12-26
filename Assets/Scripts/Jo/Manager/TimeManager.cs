using UnityEngine;
using UnityEngine.UI;

public class TimeManager : Singleton<TimeManager>
{
    [Header("Time Settings")]
    [SerializeField] private float timeLimit = 20f; // 시간 제한 (초)
    
    [Header("UI")]
    [SerializeField] private Scrollbar timeScrollbar; // 시간 표시용 스크롤바
    
    private float currentTime;
    private bool isTimerRunning;
    
    private void Update()
    {
        if (!isTimerRunning) return;
        
        currentTime -= Time.deltaTime;
        
        // 스크롤바 업데이트 (100% ~ 0%)
        if (timeScrollbar != null)
        {
            timeScrollbar.size = Mathf.Clamp01(currentTime / timeLimit);
        }
        
        // 시간 제한 초과
        if (currentTime <= 0f)
        {
            OnTimeExpired();
        }
    }
    
    /// <summary>
    /// 새로운 손님 시간 제한 시작
    /// </summary>
    public void StartTimer()
    {
        currentTime = timeLimit;
        isTimerRunning = true;
        
        // 스크롤바 초기화 (100%)
        if (timeScrollbar != null)
        {
            timeScrollbar.size = 1f;
        }
    }
    
    /// <summary>
    /// 타이머 중지
    /// </summary>
    public void StopTimer()
    {
        isTimerRunning = false;
    }
    
    /// <summary>
    /// 시간 제한 초과 시 호출
    /// </summary>
    private void OnTimeExpired()
    {
        isTimerRunning = false;
        
        if (CheckoutManager.Instance != null)
        {
            CheckoutManager.Instance.FinishCheckout(false);
        }
    }
    
    /// <summary>
    /// 남은 시간 반환 (0 ~ timeLimit)
    /// </summary>
    public float GetRemainingTime()
    {
        return Mathf.Max(0f, currentTime);
    }
    
    /// <summary>
    /// 남은 시간 비율 반환 (0 ~ 1)
    /// </summary>
    public float GetRemainingTimeRatio()
    {
        return Mathf.Clamp01(currentTime / timeLimit);
    }
}

