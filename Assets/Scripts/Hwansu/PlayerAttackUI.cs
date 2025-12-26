using TMPro;
using UnityEngine;

public class PlayerAttackUI : MonoBehaviour
{
    public CharacterData characterData;
    public TMP_Text attackText;

    void OnEnable()
    {
        characterData.OnStatChanged += UpdateUI;
        UpdateUI();
    }

    void OnDisable()
    {
        characterData.OnStatChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        attackText.text = $"ATTACK: {characterData.attack}";
    }
}
