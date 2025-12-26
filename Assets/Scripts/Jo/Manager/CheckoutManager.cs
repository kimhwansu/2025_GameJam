using System.Collections.Generic;
using UnityEngine;

public class CheckoutManager : Singleton<CheckoutManager>
{
    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints = new Transform[6]; // 6개의 스폰 위치
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private int spawnCount = 3; // 랜덤으로 선택할 위치 개수

    [Header("UI")]
    [SerializeField] private CheckoutUI checkoutUi;

    [Header("Money")]
    public int playerMoney;
    [SerializeField] private int penaltyMoney = 1000;

    [Header("GoalMoney")] // 목표 금액
    [SerializeField] private int goalMoney; 

    private readonly List<ScannableItem> spawnedItems = new();
    private int total;   // 현재 총합
 
    private void Start()
    {
        // 결제 버튼 연결
        if (checkoutUi != null)
            checkoutUi.SetCheckoutButton(OnCheckoutButtonClicked);
        
        StartCustomer();
        checkoutUi.ClearResult();
    }

    public void StartCustomer()
    {
        Debug.Log("CheckoutManager: StartCustomer 호출됨");
        ClearRound();
        SpawnGoodsRandom();
        checkoutUi.ClearList();
        checkoutUi.SetTotal(0);
    }

    private void SpawnGoodsRandom()
    {

        // 6개 위치 리스트에 새로 복사
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
                availableIndices.Add(i);
        }

        // 랜덤으로 3개 선택 (또는 사용 가능한 개수만큼)
        int countToSpawn = Mathf.Min(spawnCount, availableIndices.Count);
        List<int> selectedIndices = new List<int>();

        // selectedIndices에 랜덤으로 뽑은 3개의 아이템 추가
        for (int i = 0; i < countToSpawn; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            selectedIndices.Add(availableIndices[randomIndex]);
            availableIndices.RemoveAt(randomIndex);
        }

        // 선택된 위치에 아이템 생성 (월드 좌표 기준)
        foreach (int index in selectedIndices)
        {
            var prefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

            var go = Instantiate(prefab, spawnPoints[index].position, spawnPoints[index].rotation);
            go.SetActive(true); // 프리팹이 비활성화되어 있을 수 있으므로 활성화
            
            var item = go.GetComponentInChildren<ScannableItem>(); // 바코드가 자식일 수도 있으니
            if (item != null) spawnedItems.Add(item);
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
            total = checkoutUi.GetTotalPrice();
        else
            total = newTotal;
        
        checkoutUi.SetTotal(total);
    }

    /// <summary>
    /// 결제 버튼 클릭 시 호출되는 메서드
    /// </summary>
    private void OnCheckoutButtonClicked()
    {
        // 모든 아이템이 올바르게 스캔되었는지 확인
        bool isValid = checkoutUi.ValidateAllItemsScanned(spawnedItems);
        
        // 결과 텍스트 표시
        checkoutUi.SetResult(isValid);
        
        if (isValid)
        {
            Debug.Log("검증 성공: 모든 아이템이 올바르게 스캔되었습니다.");
            FinishCheckout(true);
        }
        else
        {
            Debug.LogWarning("검증 실패: 모든 아이템이 올바른 개수로 스캔되지 않았습니다.");
            // 검증 실패 시 처리 (예: 경고 메시지 표시 등)
            FinishCheckout(false);
        }
    }

    public void FinishCheckout(bool isSuccess)
    {
        if (isSuccess)
        {
            playerMoney += total;
            checkoutUi.SetPlayerMoney(playerMoney);
        }
        else
        {
            playerMoney -= penaltyMoney;
            checkoutUi.SetPlayerMoney(playerMoney);
        }

        StartCustomer(); // 다음 손님으로 바로 넘어가고 싶다면
    }

    private void ClearRound()
    {
        total = 0;

        // 스폰된 물건 제거
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] != null)
                Destroy(spawnedItems[i].transform.root.gameObject); 
            // 바코드가 자식이면 root로 제거
        }
        spawnedItems.Clear();
    }
}
