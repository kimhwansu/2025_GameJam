using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonUI : MonoBehaviour
{
    public Button upgradeButton;
    public StatType statType;      // 어떤 스텟 버튼인지
    public int upgradeCost = 100;  // 버튼별 강화 비용
    public StatBar statBar;        // 연결된 게이지바

    void Start()
    {
        upgradeButton.onClick.AddListener(OnClickUpgrade);
    }

    void OnClickUpgrade()
    {
        UpgradeManager.Instance.TryUpgrade(statType, upgradeCost, statBar);
    }
}
