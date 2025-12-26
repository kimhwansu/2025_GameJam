using UnityEngine;

[CreateAssetMenu(menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int gold;

    [Header("ÃÊ±â °ñµå °ª")]
    public int startGold = 1000;

    public System.Action OnGoldChanged;

    public void ResetData()
    {
        gold = startGold;
        OnGoldChanged?.Invoke();
    }

    public bool SpendGold(int cost)
    {
        if (gold < cost) return false;

        gold -= cost;
        OnGoldChanged?.Invoke();
        return true;
    }
}
