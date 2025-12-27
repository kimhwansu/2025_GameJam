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
    [SerializeField] private int itemNumber = 1; // 1, 2, 3

    [Header("UI References")]
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI purchaseButtonText;

    private int itemPrice;
    private bool isPurchased = false;
    private string itemID; // 고유 아이템 ID 

    void Start()
    {
        // 아이템 ID 생성
        itemID = $"{itemCategory}_{itemNumber}";

        // 번호에 따른 가격 설정
        SetPriceByNumber();

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

    private void SetPriceByNumber()
    {
        // 번호에 따른 가격 설정
        itemPrice = itemNumber switch
        {
            1 => 12000,
            2 => 14000,
            3 => 16000,
            _ => 12000 + (itemNumber - 1) * 2000 // 4번 이상은 2000원씩 증가
        };
    }

    private void OnPurchaseButtonClicked()
    {
        if (isPurchased)
            return;

        if (GoldManager.Instance.UseGold(itemPrice))
        {
            isPurchased = true;
            UpdateUI();

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
        if (isPurchased)
        {
            // 구매 완료 상태
            if (purchaseButtonText != null)
                purchaseButtonText.text = "구매완료";

            if (purchaseButton != null)
                purchaseButton.interactable = false;
        }
        else
        {
            // 구매 전 상태 - 가격 표시
            if (purchaseButtonText != null)
                purchaseButtonText.text = $"{itemPrice:N0} Coin";

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
                break;
            case ItemCategory.BG:
                // 배경 아이템 지급 - 배경 변경
                if (BackgroundManager.Instance != null)
                {
                    BackgroundManager.Instance.ChangeBackground(itemNumber);
                }
                break;
            case ItemCategory.Effect:
                // 이펙트 아이템 지급 - 이펙트 변경
                if (EffectManager.Instance != null)
                {
                    EffectManager.Instance.ChangeEffect(itemNumber);
                }
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