using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 1. 전반적인 손님 처리와 타이머 & 휴식시간 관리
/// 2. 계산 결과에 따른 보유 금액 갱신
/// </summary>


public class CheckoutManager : Singleton<CheckoutManager>
{
    [Header("UI")]
    [SerializeField] private CheckoutUI checkoutUi;
    [SerializeField] private MoneyPresenter MoneyPresenter; // 실제 MoneyData 접근 시 사용

    [Header("Money")]
    [SerializeField] private int playerMoney; // 보유 금액
    [SerializeField] private int partTimeMoney = 2000; // 고정 획득 금액
    [SerializeField] private int penaltyMoney = 1000; // 패널티 금액

    [Header("GoalMoney")] // 목표 금액 (배치된 아이템들 총합)
    [SerializeField] private int goalMoney; 

    private List<ScannableItem> spawnedItems = new();
    [SerializeField] private int total;   // 현재 총합
 
    private void Start()
    {
            // 초기 보유 금액 설정
            MoneyPresenter.Init(new MoneyData());
            playerMoney = MoneyPresenter.Model.Money;

        // 결제 버튼 연결
        if (checkoutUi != null)
            checkoutUi.SetCheckoutButton(OnCheckoutButtonClicked);
        
        StartCustomer();
    }

    public void StartCustomer()
    {
        //Debug.Log("CheckoutManager: StartCustomer 호출됨");
        
        // 손님 수 증가 (휴식 체크용)
        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.IncrementCustomerCount();
            
            // 5명마다 휴식 시간 체크
            if (CustomerManager.Instance.NeedsRest())
            {
                StartRestTime();
                return; // 휴식 시간이면 여기서 종료
            }
        }
        
        // 대화 데이터가 있으면 대화 먼저 시작, 없으면 바로 라운드 시작
        List<DialogueData> dialogues = DialogueDataLoader.GetAllDialogues();
        if (dialogues != null && dialogues.Count > 0 && DialogueManager.Instance != null)
        {
            StartDialogue();
        }
        else
        {
            // 일반 손님 처리
            StartCustomerRound();
        }
    }
    
    // 랜덤 대화 시작
    private void StartDialogue()
    {
        // 랜덤으로 대화 데이터 선택
        DialogueData randomDialogue = DialogueDataLoader.GetRandomDialogue();
        
        if (randomDialogue != null && DialogueManager.Instance != null)
        {
            // 대화의 defaultPortraitId에 맞는 손님 이미지 설정
            if (CustomerManager.Instance != null && !string.IsNullOrEmpty(randomDialogue.defaultPortraitId))
            {
                CustomerManager.Instance.SetCustomerByPortraitId(randomDialogue.defaultPortraitId);
                // 대화 시작 시 손님 이미지 활성화
                CustomerManager.Instance.ShowCustomerImage();
            }
            
            // 대화 종료 콜백 설정
            DialogueManager.Instance.OnDialogueEnd = OnDialogueEnd;
            
            // 대화 시작
            DialogueManager.Instance.StartDialogue(randomDialogue);
        }
        else
        {
            // 대화 데이터가 없으면 바로 라운드 시작
            StartCustomerRound();
        }
    }
    
    // 대화 종료 시 호출
    private void OnDialogueEnd()
    {
        // 대화 종료 후 라운드 시작 (초상화는 유지됨)
        StartCustomerRound();
    }
    
    // 일반 손님 처리 시작
    private void StartCustomerRound()
    {
        ClearRound(); // 라운드 초기화
        
        // 손님 이미지 활성화
        if (CustomerManager.Instance != null) { CustomerManager.Instance.ShowCustomerImage();}
        
        // 아이템 스폰
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
        
        // 라운드 초기화 (아이템 제거)
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

    // 현재 총합 텍스트 갱신
    public void UpdateTotal(int newTotal = -1)
    {
        if (newTotal < 0)
            total = checkoutUi.GetTotalPrice(); // UI에서 총합 계산
        else
            total = newTotal; // 직접 지정
        
        checkoutUi.SetTotal(total);
    }

    // Payment 버튼 클릭 시 동작할 버튼 이벤트
    private void OnCheckoutButtonClicked()
    {
        // 모든 아이템이 올바르게 스캔되었는지 확인
        bool isValid = checkoutUi.ValidateAllItemsScanned(spawnedItems);
        
        // True/False에 따른 결과 처리
        if (isValid) { FinishCheckout(true); } else { FinishCheckout(false); }
    }


    // 결과 처리 (성공/실패 - 금액 추가/감소)
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
            //playerMoney += partTimeMoney; // 고정 금액 추가 (총합x)
            MoneyPresenter.AddMoney(partTimeMoney);

            playerMoney = MoneyPresenter.Model.Money;

            checkoutUi.SetPlayerMoney(playerMoney);
        }
        else
        {
            //playerMoney -= penaltyMoney; // 패널티 금액 차감
            MoneyPresenter.DeleteMoney(penaltyMoney);

            playerMoney = MoneyPresenter.Model.Money;

            checkoutUi.SetPlayerMoney(playerMoney);
        }

        // 테이블 비우기 (아이템 제거)
        ClearRound();
        
        // UI 초기화
        checkoutUi.ClearList();
        checkoutUi.SetTotal(0);

        // 다음 손님으로 넘어가기 (대화 시작)
        StartCustomer();
    }

    // 라운드 초기화
    // 총합 초기화 및 ItemManager에 아이템 정리 요청
    private void ClearRound()
    {
        total = 0; // 총합 초기화
        
        // ItemManager에 아이템 정리 및 sortingOrder 초기화 요청
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.ClearItems();
        }
        
        spawnedItems.Clear();
    }

}
