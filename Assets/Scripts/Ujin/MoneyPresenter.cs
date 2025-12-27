using UnityEngine;

public class MoneyPresenter : MonoBehaviour
{
    [SerializeField] private MoneyView view;
    public MoneyData Model { get; private set; }

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

    // 돈 감소 (데이터 & UI)
    public void DeleteMoney(int amount)
    {
        Model.Delete(amount);
        Refresh();
    }

    public void Refresh()
    {
        view.SetMoney(Model.Money);
    }
}