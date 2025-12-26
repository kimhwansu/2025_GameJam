using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Data/UserData")]
public class UserData : ScriptableObject
{
    [Header("초기 재화")]
    public int gold = 1000;

    public Action OnGoldChanged;

    public void AddGold(int value)
    {
        gold += value;
        OnGoldChanged?.Invoke();
    }

    public bool SpendGold(int cost)
    {
        if (gold < cost)
            return false;

        gold -= cost;
        OnGoldChanged?.Invoke();
        return true;
    }

    public void ResetData()
    {
        gold = 1000;
        OnGoldChanged?.Invoke();
    }
}
