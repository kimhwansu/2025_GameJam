using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    public PlayerData playerData;
    public TMP_Text goldText;

    void OnEnable()
    {
        playerData.OnGoldChanged += UpdateUI;
        UpdateUI();
    }

    void OnDisable()
    {
        playerData.OnGoldChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        goldText.text = $"Gold: {playerData.gold}";
    }

}
