using System.Collections.Generic;
using UnityEngine;

public class CheckoutManager : Singleton<CheckoutManager>
{
    [Header("UI")]
    [SerializeField] private CheckoutUI checkoutUi;

    [Header("Money")]
    public int playerMoney;
    [SerializeField] private int partTimeMoney = 2000;
    [SerializeField] private int penaltyMoney = 1000;

    [Header("GoalMoney")] // 목표 금액
    [SerializeField] private int goalMoney; 

    private List<ScannableItem> spawnedItems = new();
    private int total;   // 현재 총합
 
    private void Start()
    {
        // 결제 버튼 연결
        if (checkoutUi != null)
            checkoutUi.SetCheckoutButton(OnCheckoutButtonClicked);
        
        StartCustomer();
    }

    public void StartCustomer()
    {
        Debug.Log("CheckoutManager: StartCustomer 호출됨");
        
        // CustomerManager를 통한 손님 이미지 변경
        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.ChangeCustomer();
            
            // 5명마다 휴식 시간 체크
            if (CustomerManager.Instance.NeedsRest())
            {
                StartRestTime();
                return; // 휴식 시간이면 여기서 종료
            }
        }
        
        // 일반 손님 처리
        StartCustomerRound();
    }
    
    // 일반 손님 라운드 시작
    private void StartCustomerRound()
    {
        ClearRound(); // 이전 라운드 정리
        
        // 손님 이미지 활성화
        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.ShowCustomerImage();
        }
        
        // ItemManager를 통한 아이템 스폰
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.RandomizeSpawnPoints(); // 스폰포인트를 랜덤 위치로 이동
            spawnedItems = ItemManager.Instance.SpawnItems(); // 랜덤 아이템 생성
        }
        
        checkoutUi.ClearList(); // UI 리스트 초기화
        checkoutUi.SetTotal(0); // 총합 초기화
        
        // 시간 제한 시작
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StartTimer();
        }
    }
    
    // 휴식 시간 시작
    private void StartRestTime()
    {
        Debug.Log($"휴식 시간 시작! (손님 {CustomerManager.Instance.GetCustomerCount()}명 처리 완료)");
        
        // 이전 라운드 정리 (아이템 제거)
        ClearRound();
        
        // 손님 이미지 비활성화
        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.HideCustomerImage();
        }
        
        // UI 초기화
        checkoutUi.ClearList();
        checkoutUi.SetTotal(0);
        
        // 휴식 시간 타이머 시작
        if (TimeManager.Instance != null && CustomerManager.Instance != null)
        {
            TimeManager.Instance.StartRestTimer(CustomerManager.Instance.GetRestDuration());
        }
    }

    public void Scan(ScannableItem item)
    {
        checkoutUi.AddLine(item.data.productId, item.data.icon, item.data.displayName, item.data.price);
        UpdateTotal();
    }

    public void UpdateTotal(int newTotal = -1)
    {
        if (newTotal < 0)
            total = checkoutUi.GetTotalPrice(); // UI에서 총합 계산
        else
            total = newTotal; // 직접 지정
        
        checkoutUi.SetTotal(total);
    }

    private void OnCheckoutButtonClicked()
    {
        // 모든 아이템이 올바르게 스캔되었는지 확인
        bool isValid = checkoutUi.ValidateAllItemsScanned(spawnedItems);
        
        if (isValid)
        {
            FinishCheckout(true); // 성공
        }
        else
        {
            FinishCheckout(false); // 실패
        }
    }

    public void FinishCheckout(bool isSuccess)
    {
        // 결과 문구 출력 (성공/실패)
        checkoutUi.SetResult(isSuccess);
        
        // 타이머 중지
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StopTimer();
        }
        
        // 성공/실패에 따른 돈 처리
        if (isSuccess)
        {
            playerMoney += partTimeMoney; // 총합만큼 돈 추가
            checkoutUi.SetPlayerMoney(playerMoney);
        }
        else
        {
            playerMoney -= penaltyMoney; // 패널티 금액 차감
            checkoutUi.SetPlayerMoney(playerMoney);
        }

        StartCustomer(); // 다음 손님으로 바로 넘어가기
    }

    // 라운드 초기화
    // 총합 초기화 및 ItemManager에 아이템 정리 요청
    private void ClearRound()
    {
        total = 0;
        
        // ItemManager에 아이템 정리 및 sortingOrder 초기화 요청
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.ClearItems();
        }
        
        spawnedItems.Clear();
    }

}
