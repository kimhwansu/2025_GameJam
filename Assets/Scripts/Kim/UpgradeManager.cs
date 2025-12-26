using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public PlayerData playerData;
    public int upgradeCost = 100;

    void Awake()
    {
        Instance = this;
    }

    public void TryUpgrade()
    {
        bool success = playerData.SpendGold(upgradeCost);

        if (!success)
        {
            Debug.Log("재화 부족");
        }
    }
}
