using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MoneyView : MonoBehaviour
{
    [SerializeField] private Text moneyText;
    private void Awake()
    {
        moneyText = GetComponent<Text>();
    }
    public void SetMoney(int value)
    {
        moneyText.text = "보유 : " + value.ToString() + "원";
    }
}