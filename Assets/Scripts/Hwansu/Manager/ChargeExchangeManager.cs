using UnityEngine;
using UnityEngine.UI;

public class ChargeExchangeManager : MonoBehaviour
{
    [Header("충전 금액 버튼들")]
    [SerializeField] private Button charge1000Button;
    [SerializeField] private Button charge5000Button;
    [SerializeField] private Button charge10000Button;

    [Header("References")]
    [SerializeField] private MoneyPresenter moneyPresenter;

    void Start()
    {
        // 충전 버튼 이벤트 연결
        if (charge1000Button != null)
            charge1000Button.onClick.AddListener(() => OnChargeButtonClicked(1000));

        if (charge5000Button != null)
            charge5000Button.onClick.AddListener(() => OnChargeButtonClicked(5000));

        if (charge10000Button != null)
            charge10000Button.onClick.AddListener(() => OnChargeButtonClicked(10000));
    }

    private void OnChargeButtonClicked(int amount)
    {
        ExchangeMoneyToGold(amount);
    }

    private void ExchangeMoneyToGold(int amount)
    {
        // MoneyPresenter가 없으면 실행 안함
        if (moneyPresenter == null || moneyPresenter.Model == null)
        {
            return;
        }

        // 현재 보유 금액 확인
        if (moneyPresenter.Model.Money < amount)
        {
            // 금액 부족 시 아무 일도 일어나지 않음
            return;
        }

        // 1. 돈 차감
        moneyPresenter.DeleteMoney(amount);

        // 2. 골드 추가 (1:1 비율)
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(amount);
        }
    }

    // 외부에서 호출 가능한 함수
    public void ChargeAmount(int amount)
    {
        OnChargeButtonClicked(amount);
    }
}