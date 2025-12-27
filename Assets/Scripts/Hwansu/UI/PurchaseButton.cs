using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchaseButton : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private int itemType = 1; // 1, 2, 3

    [Header("UI References")]
    [SerializeField] private Button purchaseButton; // 구매 버튼 (스크립트 있음)
    [SerializeField] private TextMeshProUGUI purchaseButtonText; // 구매 버튼의 TMP (구매하기/구매완료)
    [SerializeField] private Button priceButton; // 가격 표시 버튼 (스크립트 없음, 클릭 불가)
    [SerializeField] private TextMeshProUGUI priceButtonText; // 가격 버튼의 TMP (가격 표시)

    private int itemPrice;
    private bool isPurchased = false;

    void Start()
    {
        // 타입에 따른 가격 설정
        SetPriceByType();

        // 구매 버튼 클릭 이벤트 연결
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
        // 이벤트 구독 해제
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged -= CheckAffordable;
        }
    }

    private void SetPriceByType()
    {
        switch (itemType)
        {
            case 1:
                itemPrice = 12000;
                break;
            case 2:
                itemPrice = 14000;
                break;
            case 3:
                itemPrice = 16000;
                break;
            default:
                itemPrice = 12000;
                Debug.LogWarning($"잘못된 itemType: {itemType}. 기본값 12000으로 설정됩니다.");
                break;
        }
    }

    private void OnPurchaseButtonClicked()
    {
        if (isPurchased)
            return;

        // 골드 확인 및 차감
        if (GoldManager.Instance.UseGold(itemPrice))
        {
            // 구매 성공
            isPurchased = true;
            UpdateUI();

            Debug.Log($"타입 {itemType} 아이템 구매 완료! (가격: {itemPrice})");

            // 여기에 구매 완료 후 추가 로직 작성
            // 예: 아이템 지급, 사운드 재생 등
        }
        else
        {
            // 골드 부족
            ShowInsufficientGoldFeedback();
        }
    }

    private void UpdateUI()
    {
        // 가격 버튼은 항상 가격 표시
        if (priceButtonText != null)
        {
            priceButtonText.text = $"{itemPrice:N0} Gold";
        }

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
            // 구매 가능 상태
            if (purchaseButtonText != null)
                purchaseButtonText.text = "구매하기";

            CheckAffordable();
        }
    }

    private void CheckAffordable()
    {
        if (isPurchased || purchaseButton == null)
            return;

        // 현재 골드로 구매 가능한지 확인
        bool canAfford = GoldManager.Instance.Gold >= itemPrice;
        purchaseButton.interactable = canAfford;
    }

    private void ShowInsufficientGoldFeedback()
    {
        // 골드 부족 피드백
        Debug.Log("골드가 부족합니다!");
    }
}