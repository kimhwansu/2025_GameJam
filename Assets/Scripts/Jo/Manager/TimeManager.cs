using UnityEngine;
using UnityEngine.UI;

public class TimeManager : Singleton<TimeManager>
{
    [Header("Time Settings")]
    [SerializeField] private float timeLimit = 20f; // 시간 제한 (초)
    
    [Header("UI")]
    [SerializeField] private Scrollbar timeScrollbar; // 시간 표시용 스크롤바 (100% ~ 0%로 감소)
    
    private float currentTime; // 현재 남은 시간
    private bool isTimerRunning; // 타이머 실행 중 여부
    private bool isRestTime = false; // 휴식 시간 여부
    private float currentTimeLimit; // 현재 사용 중인 시간 제한 (일반 시간 또는 휴식 시간)
    
    private void Update()
    {
        if (!isTimerRunning) return;
        
        // 시간 감소
        currentTime -= Time.deltaTime;
        
        // 스크롤바 업데이트 (100% ~ 0%)
        // currentTime / currentTimeLimit 비율로 스크롤바 크기 설정
        if (timeScrollbar != null)
        {
            timeScrollbar.size = Mathf.Clamp01(currentTime / currentTimeLimit);
        }
        
        // 시간 제한 초과 시 처리
        if (currentTime <= 0f)
        {
            if (isRestTime)
            {
                OnRestTimeExpired();
            }
            else
            {
                OnTimeExpired();
            }
        }
    }
    
    public void StartTimer()
    {
        currentTime = timeLimit;
        currentTimeLimit = timeLimit;
        isTimerRunning = true;
        isRestTime = false;
        
        // 스크롤바 초기화 (100%)
        if (timeScrollbar != null)
        {
            timeScrollbar.size = 1f;
        }
    }
    
    // 휴식 시간 시작
    public void StartRestTimer(float restDuration)
    {
        currentTime = restDuration;
        currentTimeLimit = restDuration; // 휴식 시간을 기준으로 설정
        isTimerRunning = true;
        isRestTime = true;
        
        // 스크롤바 초기화 (100%)
        if (timeScrollbar != null)
        {
            timeScrollbar.size = 1f;
        }
    }
    
    public void StopTimer()
    {
        isTimerRunning = false;
        isRestTime = false;
    }
    
    private void OnTimeExpired()
    {
        isTimerRunning = false;
        
        if (CheckoutManager.Instance != null)
        {
            CheckoutManager.Instance.FinishCheckout(false);
        }
    }
    
    // 휴식 시간 종료 시 호출
    private void OnRestTimeExpired()
    {
        isTimerRunning = false;
        isRestTime = false;
        
        // 휴식 종료 후 다음 손님 시작
        if (CheckoutManager.Instance != null)
        {
            CheckoutManager.Instance.StartCustomer();
        }
    }
    
    // 현재 휴식 시간인지 확인
    public bool IsRestTime()
    {
        return isRestTime;
    }
    
    public float GetRemainingTime()
    {
        return Mathf.Max(0f, currentTime);
    }
    
    public float GetRemainingTimeRatio()
    {
        return Mathf.Clamp01(currentTime / timeLimit);
    }
}

