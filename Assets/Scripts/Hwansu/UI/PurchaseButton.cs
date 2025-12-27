using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
    private string itemID;
    private static int totalPurchaseCount = 0; // 전체 구매 횟수 (static으로 모든 버튼이 공유)

    // 구매 완료 시 발생하는 이벤트
    public static event Action OnPurchaseCompleted;

    void Start()
    {
        itemID = $"{itemCategory}_{itemNumber}";
        SetPriceByNumber();

        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        }

        UpdateUI();

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
        itemPrice = itemNumber switch
        {
            1 => 12000,
            2 => 14000,
            3 => 16000,
            _ => 12000 + (itemNumber - 1) * 2000
        };
    }

    private void OnPurchaseButtonClicked()
    {
        if (isPurchased)
            return;

        if (GoldManager.Instance.UseGold(itemPrice))
        {
            isPurchased = true;
            totalPurchaseCount++; // 구매 카운트 증가
            UpdateUI();
            OnItemPurchased();

            // 구매 완료 이벤트 발생
            OnPurchaseCompleted?.Invoke();
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
            if (purchaseButtonText != null)
                purchaseButtonText.text = "구매완료";

            if (purchaseButton != null)
                purchaseButton.interactable = false;
        }
        else
        {
            if (purchaseButtonText != null)
                purchaseButtonText.text = $"{itemPrice:N0}";

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
        switch (itemCategory)
        {
            case ItemCategory.Cloth:
                if (CharacterSkinManager.Instance != null)
                {
                    CharacterSkinManager.Instance.ChangeClothSkin(itemNumber);
                }
                break;
            case ItemCategory.BG:
                if (BackgroundManager.Instance != null)
                {
                    BackgroundManager.Instance.ChangeBackground(itemNumber);
                }
                break;
            case ItemCategory.Effect:
                if (EffectManager.Instance != null)
                {
                    EffectManager.Instance.ChangeEffect(itemNumber);
                }
                break;
        }
    }

    // 전체 구매 횟수 반환
    public static int GetTotalPurchaseCount()
    {
        return totalPurchaseCount;
    }

    // 전체 구매 횟수 초기화 (게임 시작 시 또는 리셋 시 사용)
    public static void ResetTotalPurchaseCount()
    {
        totalPurchaseCount = 0;
    }

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