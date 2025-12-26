using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckoutUI : MonoBehaviour
{
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject linePrefab; // (아이콘, 이름, 가격) UI 프리팹
    [SerializeField] private Text totalText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Button checkoutButton; // 결제 버튼
    [SerializeField] private Text resultText; // 검증 결과 텍스트

    private readonly List<CheckoutUILine> lineList = new();

    public void AddLine(string productId, Sprite icon, string name, int price)
    {
        // 같은 제품이 이미 리스트에 있는지 확인
        foreach (var uiLine in lineList)
        {
            if (uiLine != null && uiLine.IsSameProduct(productId))
            {
                uiLine.IncreaseCount();
                UpdateTotal(); // 합계 업데이트
                return; // 개수만 증가하고 종료
            }
        }

        // 새로운 제품이면 새 라인 추가
        var go = Instantiate(linePrefab, listParent);
        var line = go.GetComponent<CheckoutUILine>();
        line.Set(productId, icon, name, price, RemoveLine, UpdateTotal);
        lineList.Add(line);
    }

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

    public int GetTotalPrice()
    {
        int total = 0;
        foreach (var line in lineList)
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

    public void SetTotal(int total) => totalText.text = $"합계: {total:N0}원";
    public void SetPlayerMoney(int money) => moneyText.text = $"보유: {money:N0}원";
    
    /// <summary>
    /// 검증 결과를 텍스트로 표시
    /// </summary>
    /// <param name="isSuccess">성공 여부</param>
    public void SetResult(bool isSuccess)
    {
        if (resultText != null)
        {
            resultText.text = isSuccess ? "성공!" : "실패! (패널티 1000원)";
        }
    }
    
    /// <summary>
    /// 결과 텍스트 초기화
    /// </summary>
    public void ClearResult()
    {
        if (resultText != null)
        {
            resultText.text = "";
        }
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
