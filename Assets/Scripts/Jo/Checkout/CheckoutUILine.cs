using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 포스기 한칸의 프리팹에 할당하는 UI 갱신만 담당
/// </summary>

public class CheckoutUILine : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text priceText;
    [SerializeField] private Text countText;
    [SerializeField] private Button cancelButton;   // 삭제 버튼 
    [SerializeField] private Button increaseButton; // 수량 증가 버튼
    [SerializeField] private Button decreaseButton; // 수량 감소 버튼

    private string productId;
    private int unitPrice;
    private int count = 1;

    private Action <CheckoutUILine> onRemoveCallback;
    private Action onCountChangedCallback; // 수량 변경 시 호출할 콜백
    
    public string GetProductId() => productId; // 제품 ID
    public int GetCount() => count; // 제품 수량


    // 스캔한 아이템 정보를 프리팹 UI에 할당
    public void Set(string id, Sprite icon, string name, int price, Action<CheckoutUILine> onRemove = null, Action onCountChanged = null)
    {
        productId = id;
        unitPrice = price;
        count = 1;
        onRemoveCallback = onRemove;
        onCountChangedCallback = onCountChanged;
        
        iconImage.sprite = icon;
        nameText.text = name;
        UpdateDisplay();
    }

    // 수량 증가 (데이터 & UI)
    public void IncreaseCount()
    {
        count++;
        UpdateDisplay();
        onCountChangedCallback?.Invoke();
    }
    
    // 수량 감소 (데이터 & UI)
    public void DecreaseCount()
    {
        if (count > 1) // 최소 1개는 유지
        {
            count--;
            UpdateDisplay();
            onCountChangedCallback?.Invoke();
        }
    }

    // 삭제 버튼 
    public void OnCancelClicked()
    {
        onRemoveCallback?.Invoke(this);
    }
    
    // 동일 제품 여부 확인
    public bool IsSameProduct(string id)
    {
        return productId == id;
    }

    // 통합 금액 내보내기
    public int GetTotalPrice()
    {
        return unitPrice * count;
    }

    // UI 업데이트 (통합 금액, 수량)
    private void UpdateDisplay()
    {
        priceText.text = $"{GetTotalPrice():N0}원";
        if (countText != null)
            countText.text = $"{count}개";
    }


}
