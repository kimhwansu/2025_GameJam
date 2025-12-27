using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonUI : MonoBehaviour
{
    public Button upgradeButton;
    public StatType statType;
    public int upgradeCost = 100;
    public StatBar statBar;

    private TMP_Text buttonText;

    void Start()
    {
        upgradeButton.onClick.AddListener(OnClickUpgrade);

        // 버튼의 텍스트 컴포넌트 가져오기
        buttonText = upgradeButton.GetComponentInChildren<TMP_Text>();

        // StatBar의 MAX 이벤트 구독
        if (statBar != null)
        {
            statBar.OnMaxReached += UpdateButtonText;
        }

        // 초기 상태 업데이트
        UpdateButtonText();
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (statBar != null)
        {
            statBar.OnMaxReached -= UpdateButtonText;
        }
    }

    void OnClickUpgrade()
    {
        UpgradeManager.Instance.TryUpgrade(statType, upgradeCost, statBar);

        // 업그레이드 후 텍스트 업데이트
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (buttonText == null || statBar == null)
            return;

        // StatBar가 MAX 상태인지 확인
        if (statBar.GetValue() >= 100)
        {
            buttonText.text = "MAX";
            upgradeButton.interactable = false; // 버튼 비활성화
        }
        else
        {
            buttonText.text = "Upgrade";
            upgradeButton.interactable = true;
        }
    }
}