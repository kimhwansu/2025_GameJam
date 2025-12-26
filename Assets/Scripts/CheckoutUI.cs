using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckoutUI : MonoBehaviour
{
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject linePrefab; // (아이콘, 이름, 가격) UI 프리팹
    [SerializeField] private Text totalText;
    [SerializeField] private Text moneyText;

    private readonly List<CheckoutUILine> lineList = new();

    public void AddLine(string productId, Sprite icon, string name, int price)
    {
        // 같은 제품이 이미 리스트에 있는지 확인
        foreach (var uiLine in lineList)
        {
            if (uiLine != null && uiLine.IsSameProduct(productId))
            {
                uiLine.IncreaseCount();
                return; // 개수만 증가하고 종료
            }
        }

        // 새로운 제품이면 새 라인 추가
        var go = Instantiate(linePrefab, listParent);
        var line = go.GetComponent<CheckoutUILine>();
        line.Set(productId, icon, name, price, RemoveLine);
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
}
