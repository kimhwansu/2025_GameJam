using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    public UserData userData;
    public TMP_Text goldText;

    void OnEnable()
    {
        userData.OnGoldChanged += UpdateUI;
        UpdateUI();
    }

    void OnDisable()
    {
        userData.OnGoldChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        goldText.text = $"Gold: {userData.gold}";
    }
}
