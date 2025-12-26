using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] private AttackBar attackBar;
    private int upgradeCost = 100;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TryUpgrade()
    {
        // 골드 사용 시도
        if (!GoldManager.Instance.UseGold(upgradeCost))
        {
            Debug.Log("골드 부족");
            return;
        }

        // 플레이어 공격력 증가
        CharacterStats.Instance.UpgradeStat(StatType.Attack, 1);

        // 게이지바 1 증가
        if (attackBar != null)
            attackBar.Increase();
    }
}
