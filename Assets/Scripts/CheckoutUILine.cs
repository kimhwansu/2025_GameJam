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

    private string productId;
    private int unitPrice;
    private int count = 1;
    private Action<CheckoutUILine> onRemoveCallback;

    private void Awake()
    {
        // 프리팹 인스턴스화 시 자동으로 버튼 찾기 (Inspector에서 할당 안 했을 경우 대비)
        if (cancelButton == null)
            cancelButton = GetComponentInChildren<Button>();
    }

    public void Set(string id, Sprite icon, string name, int price, Action<CheckoutUILine> onRemove = null)
    {
        productId = id;
        unitPrice = price;
        count = 1;
        onRemoveCallback = onRemove;
        
        iconImage.sprite = icon;
        nameText.text = name;
        UpdateDisplay();
        
        // 취소 버튼 연결 (Awake에서 찾지 못했으면 다시 시도)
        if (cancelButton == null)
            cancelButton = GetComponentInChildren<Button>();
        
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelClicked);
        }
    }

    public void IncreaseCount()
    {
        count++;
        UpdateDisplay();
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
}
