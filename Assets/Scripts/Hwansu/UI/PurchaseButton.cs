using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchaseButton : MonoBehaviour
{
    public enum ItemCategory
    {
        Cloth,
        BG,
        Effect
    }

    [Header("Item Settings")]
    [SerializeField] private ItemCategory itemCategory = ItemCategory.Cloth;
    [SerializeField] private int itemNumber = 1; // 1, 2, 3...

    [Header("UI References")]
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI purchaseButtonText;
    [SerializeField] private Button priceButton;
    [SerializeField] private TextMeshProUGUI priceButtonText;

    private int itemPrice;
    private bool isPurchased = false;
    private string itemID; // 고유 아이템 ID (예: "Cloth_1", "BG_2")

    void Start()
    {
        // 아이템 ID 생성
        itemID = $"{itemCategory}_{itemNumber}";

        // 타입에 따른 가격 설정
        SetPriceByType();

        // 가격 버튼 설정
        if (priceButton != null)
        {
            priceButton.interactable = false;
            ColorBlock colors = priceButton.colors;
            colors.disabledColor = colors.normalColor;
            priceButton.colors = colors;
        }

        // 구매 버튼 클릭 이벤트
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        }

        // 초기 UI 업데이트
        UpdateUI();

        // 골드 변경 이벤트 구독
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged += CheckAffordable;
        }
    }

    void OnDestroy()
    {
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged -= CheckAffordable;
        }
    }

    private void SetPriceByType()
    {
        // 카테고리별 기본 가격
        int basePrice = itemCategory switch
        {
            ItemCategory.Cloth => 10000,
            ItemCategory.BG => 15000,
            ItemCategory.Effect => 20000,
            _ => 10000
        };

        // 번호에 따른 추가 가격
        itemPrice = basePrice + (itemNumber - 1) * 2000;
    }

    private void OnPurchaseButtonClicked()
    {
        if (isPurchased)
            return;

        if (GoldManager.Instance.UseGold(itemPrice))
        {
            isPurchased = true;
            UpdateUI();

            Debug.Log($"{itemCategory} {itemNumber}번 아이템 구매 완료! (가격: {itemPrice})");

            // 구매 완료 후 추가 로직
            OnItemPurchased();
        }
        else
        {
            ShowInsufficientGoldFeedback();
        }
    }

    private void UpdateUI()
    {
        if (priceButtonText != null)
        {
            priceButtonText.text = $"{itemPrice:N0} Gold";
        }

        if (isPurchased)
        {
            if (purchaseButtonText != null)
                purchaseButtonText.text = "구매완료";

            if (purchaseButton != null)
                purchaseButton.interactable = false;
        }
        else
        {
            if (purchaseButtonText != null)
                purchaseButtonText.text = "구매하기";

            CheckAffordable();
        }
    }

    private void CheckAffordable()
    {
        if (isPurchased || purchaseButton == null)
            return;

        bool canAfford = GoldManager.Instance.Gold >= itemPrice;
        purchaseButton.interactable = canAfford;
    }

    private void ShowInsufficientGoldFeedback()
    {
        Debug.Log("골드가 부족합니다!");
    }

    private void OnItemPurchased()
    {
        // 구매된 아이템 처리
        switch (itemCategory)
        {
            case ItemCategory.Cloth:
                // 옷 아이템 지급 - 스킨 변경
                if (CharacterSkinManager.Instance != null)
                {
                    CharacterSkinManager.Instance.ChangeClothSkin(itemNumber);
                }
                Debug.Log($"옷 {itemNumber}번 지급 및 스킨 변경");
                break;
            case ItemCategory.BG:
                // 배경 아이템 지급 로직
                Debug.Log($"배경 {itemNumber}번 지급");
                break;
            case ItemCategory.Effect:
                // 이펙트 아이템 지급 로직
                Debug.Log($"이펙트 {itemNumber}번 지급");
                break;
        }
    }

    // 외부에서 구매 상태 확인용
    public bool IsPurchased()
    {
        return isPurchased;
    }

    public string GetItemID()
    {
        return itemID;
    }

    public ItemCategory GetCategory()
    {
        return itemCategory;
    }

    public int GetItemNumber()
    {
        return itemNumber;
    }
}