using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] private AttackBar attackBar;
    private int upgradeCost = 100;

    private void Awake()
    {
        Instance = this;
    }

    public void TryUpgrade()
    {
        if (!GoldManager.Instance.UseGold(upgradeCost))
            return;

        CharacterStats.Instance.UpgradeStat(StatType.Attack, 1);
    }

}
