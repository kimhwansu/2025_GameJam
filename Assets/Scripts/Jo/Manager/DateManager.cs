using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

public class DateManager : Singleton<DateManager>
{
    [Header("Date Settings")]
    [SerializeField] private int maxDays = 7; // 최대 일차 (7일차)
    [SerializeField] private int customersPerDay = 6; // 하루에 오는 손님 수
    // [SerializeField] private int totalNPCs = 8; // 전체 NPC 수
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dateText; // 일차 표시용 Text (예: "1일차/D-6")
    
    private int currentDay = 1; // 현재 일차 (1일차부터 시작)
    private int currentDayCustomerCount = 0; // 현재 일차에서 처리한 손님 수
    private bool isGameStarted = false; // 게임 시작 여부
    private List<string> currentDayCustomerOrder = new List<string>(); // 현재 일차의 손님 순서 (dialogueId 리스트)
    
    // 일차 변경 시 호출될 이벤트
    public Action<int> OnDayChanged;
    
    private void Start()
    {
        // 초기 일차 텍스트 설정
        UpdateDateText();
        
        // 게임 시작 전에는 시간이 흐르지 않음
        isGameStarted = false;
    }
    
    // 게임 시작 (OpeningManager에서 호출)
    public void StartGame()
    {
        if (!isGameStarted)
        {
            isGameStarted = true;
            
            Debug.Log("게임 시작!");
            
            // 첫 날 손님 순서 생성
            GenerateDayCustomerOrder();
            
            // 게임 시작 (CheckoutManager에 알림)
            if (CheckoutManager.Instance != null)
            {
                CheckoutManager.Instance.StartCustomer();
            }
        }
    }
    
    // 하루의 손님 순서 생성 (8명 중 6명 무작위 선택 및 순서 섞기)
    private void GenerateDayCustomerOrder()
    {
        currentDayCustomerOrder.Clear();
        
        // 모든 대화 데이터 가져오기
        List<DialogueData> allDialogues = DialogueDataLoader.GetAllDialogues();
        if (allDialogues == null || allDialogues.Count == 0)
        {
            Debug.LogWarning("DateManager: 대화 데이터를 찾을 수 없습니다.");
            return;
        }
        
        // 8명 중 6명 무작위 선택
        List<DialogueData> selectedDialogues = new List<DialogueData>();
        List<DialogueData> availableDialogues = new List<DialogueData>(allDialogues);
        
        // 무작위로 6명 선택
        for (int i = 0; i < customersPerDay && availableDialogues.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableDialogues.Count);
            selectedDialogues.Add(availableDialogues[randomIndex]);
            availableDialogues.RemoveAt(randomIndex);
        }
        
        // 선택된 6명의 순서를 무작위로 섞기 (Fisher-Yates 셔플)
        for (int i = selectedDialogues.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            DialogueData temp = selectedDialogues[i];
            selectedDialogues[i] = selectedDialogues[j];
            selectedDialogues[j] = temp;
        }
        
        // dialogueId 리스트로 변환
        foreach (var dialogue in selectedDialogues)
        {
            if (!string.IsNullOrEmpty(dialogue.dialogueId))
            {
                currentDayCustomerOrder.Add(dialogue.dialogueId);
            }
        }
        
        Debug.Log($"{currentDay}일차 손님 순서 생성: {string.Join(", ", currentDayCustomerOrder)}");
    }
    
    // 손님 처리 완료 시 호출 (CustomerManager에서 호출)
    public void OnCustomerProcessed()
    {
        if (!isGameStarted) return; // 게임이 시작되지 않았으면 무시
        
        currentDayCustomerCount++;
        
        // 하루의 손님 수가 다 차면 휴식 시간으로 넘어감
        // (CheckoutManager의 StartRestTime에서 처리됨)
    }
    
    // 휴식 시간 종료 시 호출 (TimeManager에서 호출)
    public void OnRestTimeEnded()
    {
        if (!isGameStarted) return; // 게임이 시작되지 않았으면 무시
        
        // 7일차 휴식 시간이 끝나면 엔딩 호출 (일차 증가 전에 체크)
        if (currentDay >= maxDays)
        {
            OnGameEnd();
            return;
        }
        
        // 일차 증가
        currentDay++;
        currentDayCustomerCount = 0; // 새 일차의 손님 수 초기화
        
        // 새 일차의 손님 순서 생성 (매일 다른 조합)
        GenerateDayCustomerOrder();
        
        // 일차 텍스트 업데이트
        UpdateDateText();
        
        // 일차 변경 이벤트 호출
        OnDayChanged?.Invoke(currentDay);
        
        Debug.Log($"{currentDay}일차 시작!");
        
        // 새 일차 시작 - 다음 손님 시작
        if (CheckoutManager.Instance != null)
        {
            CheckoutManager.Instance.StartCustomer();
        }
    }
    
    // 일차 텍스트 업데이트
    private void UpdateDateText()
    {
        if (dateText == null) return;
        
        string dateString = GetDateString();
        dateText.text = dateString;
    }
    
    // 일차 문자열 반환 (예: "1일차/D-6", "7일차/D-DAY")
    public string GetDateString()
    {
        if (currentDay > maxDays)
        {
            return "7일차 / D-DAY";
        }
        
        int daysRemaining = maxDays - currentDay;
        if (daysRemaining == 0)
        {
            return $"{currentDay}일차 / D-DAY";
        }
        else
        {
            return $"{currentDay}일차 / D-{daysRemaining}";
        }
    }
    
    // 현재 일차 반환
    public int GetCurrentDay()
    {
        return currentDay;
    }
    
    // 하루에 오는 손님 수 반환
    public int GetCustomersPerDay()
    {
        return customersPerDay;
    }
    
    // 현재 일차에서 처리한 손님 수 반환
    public int GetCurrentDayCustomerCount()
    {
        return currentDayCustomerCount;
    }
    
    // 하루의 손님 수가 다 찼는지 확인
    public bool IsDayComplete()
    {
        return currentDayCustomerCount > customersPerDay;
    }
    
    // 게임 시작 여부 반환
    public bool IsGameStarted()
    {
        return isGameStarted;
    }
    
    // 현재 일차의 다음 손님 dialogueId 반환
    public string GetNextCustomerDialogueId()
    {
        if (currentDayCustomerOrder == null || currentDayCustomerOrder.Count == 0)
        {
            return null;
        }
        
        // 현재 처리한 손님 수가 리스트 범위를 벗어나면 null 반환
        if (currentDayCustomerCount >= currentDayCustomerOrder.Count)
        {
            return null;
        }
        
        return currentDayCustomerOrder[currentDayCustomerCount];
    }
    
    // 엔딩 호출
    private void OnGameEnd()
    {
        Debug.Log("=== 엔딩 ===");
        Debug.Log("7일차가 모두 끝났습니다. 게임 종료!");
        // 추후 엔딩 UI 구현 예정
    }
}

