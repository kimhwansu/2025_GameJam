using System.Collections.Generic;
using UnityEngine;

public class CheckoutManager : Singleton<CheckoutManager>
{
    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints = new Transform[6]; // 6개의 스폰 위치
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private int spawnCount = 3; // 랜덤으로 선택할 위치 개수
    
    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero; // 스폰 영역 중심
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 10f); // 스폰 영역 크기 (너비, 높이)

    [Header("UI")]
    [SerializeField] private CheckoutUI checkoutUi;

    [Header("Money")]
    public int playerMoney;
    [SerializeField] private int penaltyMoney = 1000;

    [Header("GoalMoney")] // 목표 금액
    [SerializeField] private int goalMoney; 

    private readonly List<ScannableItem> spawnedItems = new();
    private int total;   // 현재 총합
    private int currentMaxSortingOrder = 0; // 현재 가장 높은 sortingOrder
 
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
        RandomizeSpawnPoints(); // 스폰포인트를 랜덤 위치로 이동
        SpawnGoodsRandom();
        checkoutUi.ClearList();
        checkoutUi.SetTotal(0);
        
        // 시간 제한 시작
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StartTimer();
        }
    }
    
    /// 드래그 시작 시 호출하여 새로운 최대 sortingOrder를 반환
    public int GetNextSortingOrder()
    {
        currentMaxSortingOrder++;
        return currentMaxSortingOrder;
    }
    
    /// 스폰 포인트들을 지정된 범위 내의 랜덤 위치로 이동
    private void RandomizeSpawnPoints()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                // 네모 모양 범위 내의 랜덤 위치 계산
                float randomX = Random.Range(
                    spawnAreaCenter.x - spawnAreaSize.x / 2f,
                    spawnAreaCenter.x + spawnAreaSize.x / 2f
                );
                float randomY = Random.Range(
                    spawnAreaCenter.y - spawnAreaSize.y / 2f,
                    spawnAreaCenter.y + spawnAreaSize.y / 2f
                );
                
                // Z 좌표는 기존 값 유지
                Vector3 newPosition = new Vector3(randomX, randomY, spawnPoint.position.z);
                spawnPoint.position = newPosition;
            }
        }
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

    /// 결제 버튼 클릭 시 호출되는 메서드
    private void OnCheckoutButtonClicked()
    {
        // 모든 아이템이 올바르게 스캔되었는지 확인
        bool isValid = checkoutUi.ValidateAllItemsScanned(spawnedItems);
        
        // 결과 텍스트 표시
        
        if (isValid)
        {
            FinishCheckout(true);
        }
        else
        {
            // 검증 실패 시 처리 (예: 경고 메시지 표시 등)
            FinishCheckout(false);
        }
    }

    public void FinishCheckout(bool isSuccess)
    {
        // 문구 출력
        checkoutUi.SetResult(isSuccess);
        
        // 타이머 중지
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StopTimer();
        }
        
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
        
        // sortingOrder 초기화
        currentMaxSortingOrder = 0;

        // 스폰된 물건 제거
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] != null)
                Destroy(spawnedItems[i].transform.root.gameObject); 
            // 바코드가 자식이면 root로 제거
        }
        spawnedItems.Clear();
    }
    
    // 에디터에서 스폰 영역을 시각적으로 표시하는 Gizmo
    private void OnDrawGizmos()
    {
        // 스폰 영역 중심점 표시
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(spawnAreaCenter.x, spawnAreaCenter.y, 0);
        Gizmos.DrawWireSphere(center, 0.2f);
        
        // 스폰 영역 사각형 표시
        Gizmos.color = Color.green;
        Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0);
        Gizmos.DrawWireCube(center, size);
    }

}
