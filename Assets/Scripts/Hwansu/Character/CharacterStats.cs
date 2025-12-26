using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    public static CharacterStats Instance { get; private set; }

    public event Action OnStatsChanged;

    [Header("캐릭터 스텟")]
    public int maxHP = 100;
    public int currentHP;
    public int attack; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        currentHP = maxHP;
        attack = 1;  //초기 공격력 = 1
    }

    public void UpgradeStat(StatType type, int value)
    {
        switch (type)
        {
            case StatType.Attack:
                attack += value;
                break;
        }

        OnStatsChanged?.Invoke();
    }

    public int GetStat(StatType type)
    {
        return type switch
        {
            StatType.Attack => attack,
            _ => 0
        };
    }
}
