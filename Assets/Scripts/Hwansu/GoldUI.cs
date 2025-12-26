using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public TMP_Text goldText;

    void Start()
    {
        UpdateGoldText();
        GoldManager.Instance.OnGoldChanged += UpdateGoldText;
    }

    void UpdateGoldText()
    {
        goldText.text = GoldManager.Instance.Gold.ToString();
    }
}
