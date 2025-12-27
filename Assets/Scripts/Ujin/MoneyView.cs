using TMPro;
using UnityEngine;


public class MoneyView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    private void Awake()
    {
        moneyText = GetComponent<TextMeshProUGUI>();
    }
    public void SetMoney(int value)
    {
        moneyText.text = "보유 : " + value.ToString() + "원";
    }
}