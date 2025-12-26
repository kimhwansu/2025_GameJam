using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonUI : MonoBehaviour
{
    public Button upgradeButton;

    void Start()
    {
        upgradeButton.onClick.AddListener(OnClickUpgrade);
    }

    void OnClickUpgrade()
    {
        UpgradeManager.Instance.TryUpgrade();
    }
}
