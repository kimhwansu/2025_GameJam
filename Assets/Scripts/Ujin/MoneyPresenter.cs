using UnityEngine;

public class MoneyPresenter : MonoBehaviour
{
    [SerializeField] private MoneyView view;
    public MoneyData Model { get; private set; }

    private void Awake()
    {
        view  = GetComponent<MoneyView>();
    }
    public void Init(MoneyData model)
    {
        Model = model;
        Refresh();
    }

    // 돈 추가 (데이터 & UI)
    public void AddMoney(int amount)
    {
        Model.Add(amount);
        Refresh();
        Debug.Log("현재 금액 : " + Model.Money);
    }

    public void Refresh()
    {
        view.SetMoney(Model.Money);
    }
}