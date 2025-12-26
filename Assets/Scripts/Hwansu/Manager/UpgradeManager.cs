using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] private AttackBar attackBar;
    private int upgradeCost = 100;  //강화버튼 누르면 줄어드는 골드 값

    void Awake()
    {
        Instance = this;
    }

    public void TryUpgrade()
    {
        if (!GoldManager.Instance.UseGold(upgradeCost))
            return;

        // 공격력 +1
        attackBar.Increase();

        Debug.Log("공격력 강화 +1");
    }
}
