using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Data/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("캐릭터 상태")]
    public int hp = 100;     // 고정
    public int attack = 1;  // 강화 공격력

    public Action OnStatChanged;

    public void IncreaseAttack(int value)
    {
        attack += value;
        OnStatChanged?.Invoke();
    }

    public void ResetData()
    {
        hp = 100;
        attack = 1;
        OnStatChanged?.Invoke();
    }
}
