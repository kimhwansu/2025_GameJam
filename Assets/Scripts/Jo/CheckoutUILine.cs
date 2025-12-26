using UnityEngine;
using UnityEngine.UI;
using System;

public class CheckoutUILine : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text priceText;
    [SerializeField] private Text countText;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button increaseButton; // 수량 증가 버튼
    [SerializeField] private Button decreaseButton; // 수량 감소 버튼

    private string productId;
    private int unitPrice;
    private int count = 1;
    private Action<CheckoutUILine> onRemoveCallback;
    private Action onCountChangedCallback; // 수량 변경 시 호출할 콜백

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
        
        // 라인 제거 버튼 연결
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelClicked);
        }
        
        // 수량 증가 버튼 연결
        if (increaseButton != null)
        {
            increaseButton.onClick.RemoveAllListeners();
            increaseButton.onClick.AddListener(OnIncreaseClicked);
        }
        
        // 수량 감소 버튼 연결
        if (decreaseButton != null)
        {
            decreaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.AddListener(OnDecreaseClicked);
        }
    }

    public void IncreaseCount()
    {
        count++;
        UpdateDisplay();
        onCountChangedCallback?.Invoke();
    }
    
    public void DecreaseCount()
    {
        if (count > 1) // 최소 1개는 유지
        {
            count--;
            UpdateDisplay();
            onCountChangedCallback?.Invoke();
        }
    }
    
    private void OnIncreaseClicked()
    {
        IncreaseCount();
    }
    
    private void OnDecreaseClicked()
    {
        DecreaseCount();
    }

    public bool IsSameProduct(string id)
    {
        return productId == id;
    }

    public int GetTotalPrice()
    {
        return unitPrice * count;
    }

    private void UpdateDisplay()
    {
        priceText.text = $"{GetTotalPrice():N0}원";
        if (countText != null)
            countText.text = $"{count}개";
    }

    public void OnCancelClicked()
    {
        onRemoveCallback?.Invoke(this);
    }

    public string GetProductId() => productId;
    public int GetCount() => count;
}
