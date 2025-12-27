using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 포스기 전체 UI 갱신 담당
/// </summary>

public class CheckoutUI : MonoBehaviour
{
    [SerializeField] private Transform listParent;  // 프리팹 생성될 위치 
    [SerializeField] private GameObject linePrefab; // (아이콘, 이름, 가격) UI 프리팹
    [SerializeField] private Text totalText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Button checkoutButton; // 결제 버튼

    private readonly List<CheckoutUILine> lineList = new();

    // 바코드 스캔 시 제품 정보 추가
    public void AddLine(string productId, Sprite icon, string name, int price)
    {
        // 선택된 제품들 순회
        foreach (CheckoutUILine uiLine in lineList)
        {
            // 동일한 제품인지 확인 (제품ID 비교)
            if (uiLine != null && uiLine.IsSameProduct(productId))
            {
                uiLine.IncreaseCount();
                UpdateTotal(); // 합계 업데이트
                return; // 개수만 증가하고 종료
            }
        }

        // 새로운 제품 - 새 라인 추가
        var go = Instantiate(linePrefab, listParent);
        var line = go.GetComponent<CheckoutUILine>();
        line.Set(productId, icon, name, price, RemoveLine, UpdateTotal); // 라인 ui 지정
        lineList.Add(line); 
    }

    // 제품 정보 삭제 (라인 삭제)
    public void RemoveLine(CheckoutUILine line)
    {
        if (line == null) return;

        // 리스트에서 제거
        lineList.Remove(line);
        
        // 오브젝트 제거
        if (line.gameObject != null)
            Destroy(line.gameObject);

        // 합계 재계산 및 업데이트
        UpdateTotal();
    }

    public void UpdateTotal()
    {
        int newTotal = GetTotalPrice();
        SetTotal(newTotal);
        
        // CheckoutManager의 total도 업데이트
        if (CheckoutManager.Instance != null)
            CheckoutManager.Instance.UpdateTotal(newTotal);
    }

    // 전체 통합 금액 계산
    public int GetTotalPrice()
    {
        int total = 0;
        foreach (var line in lineList) // 라인 리스트 순회 후 통합 금액 누적
        {
            if (line != null)
                total += line.GetTotalPrice();
        }
        return total;
    }

    
    public void ClearList()
    {
        for (int i = listParent.childCount - 1; i >= 0; i--)
            Destroy(listParent.GetChild(i).gameObject);
        lineList.Clear();
    }

    // 텍스트 갱신
    public void SetTotal(int total) => totalText.text = $"합계: {total:N0}원";
    public void SetPlayerMoney(int money) => moneyText.text = $"보유: {money:N0}원";
    
    // 검증 결과 표시 (ResultTextUIManager로 위임)
    public void SetResult(bool isSuccess)
    {
        if (ResultTextUIManager.Instance != null)
        {
            ResultTextUIManager.Instance.ShowResult(isSuccess);
        }
    }
    
    // 결과 텍스트 초기화 (더 이상 필요 없음, ResultTextUIManager가 관리)
    public void ClearResult()
    {
        // ResultTextUIManager가 자동으로 처리
    }

    /// <summary>
    /// 스폰된 모든 아이템이 UI에 올바른 개수로 포함되어 있는지 확인
    /// </summary>
    /// <param name="spawnedItems">스폰된 아이템 리스트</param>
    /// <returns>모든 아이템이 올바른 개수로 포함되어 있으면 true</returns>
    public bool ValidateAllItemsScanned(List<ScannableItem> spawnedItems)
    {
        if (spawnedItems == null || spawnedItems.Count == 0)
            return true; // 스폰된 아이템이 없으면 검증 통과

        // 스폰된 아이템들을 productId별로 개수 세기
        Dictionary<string, int> spawnedCounts = new Dictionary<string, int>();
        foreach (var item in spawnedItems)
        {
            if (item != null && item.data != null)
            {
                string productId = item.data.productId;
                if (spawnedCounts.ContainsKey(productId))
                    spawnedCounts[productId]++;
                else
                    spawnedCounts[productId] = 1;
            }
        }

        // UI에 있는 아이템들을 productId별로 개수 세기
        Dictionary<string, int> uiCounts = new Dictionary<string, int>();
        foreach (var line in lineList)
        {
            if (line != null)
            {
                string productId = line.GetProductId();
                int count = line.GetCount();
                if (uiCounts.ContainsKey(productId))
                    uiCounts[productId] += count;
                else
                    uiCounts[productId] = count;
            }
        }

        // 모든 스폰된 아이템이 UI에 올바른 개수로 포함되어 있는지 확인
        foreach (var kvp in spawnedCounts)
        {
            string productId = kvp.Key;
            int spawnedCount = kvp.Value;

            if (!uiCounts.ContainsKey(productId) || uiCounts[productId] != spawnedCount)
            {
                Debug.LogWarning($"검증 실패: {productId} - 스폰된 개수: {spawnedCount}, UI 개수: {(uiCounts.ContainsKey(productId) ? uiCounts[productId] : 0)}");
                return false;
            }
        }

        // UI에 스폰되지 않은 아이템이 있는지 확인 (불필요한 아이템)
        foreach (var kvp in uiCounts)
        {
            if (!spawnedCounts.ContainsKey(kvp.Key))
            {
                Debug.LogWarning($"검증 실패: {kvp.Key} - 스폰되지 않은 아이템이 UI에 포함되어 있습니다.");
                return false;
            }
        }

        return true;
    }

    /// 결제 버튼 설정 (CheckoutManager에서 호출)
    public void SetCheckoutButton(System.Action onClick)
    {
        if (checkoutButton != null)
        {
            checkoutButton.onClick.RemoveAllListeners();
            checkoutButton.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
