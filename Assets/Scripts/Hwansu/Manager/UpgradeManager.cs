using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TryUpgrade(StatType statType, int upgradeCost, StatBar statBar)
    {
        // 골드 사용 시도
        if (!GoldManager.Instance.UseGold(upgradeCost))
        {
            Debug.Log("골드 부족");
            return;
        }

        // 플레이어 스텟 증가
        CharacterStats.Instance.UpgradeStat(statType, 1);

        // 관련 게이지바 증가
        if (statBar != null)
            statBar.Increase();
    }
}
