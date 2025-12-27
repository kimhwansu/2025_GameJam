using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    // 각 스탯의 업그레이드 횟수 추적
    private Dictionary<StatType, int> upgradeCount = new Dictionary<StatType, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 초기화
        foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
        {
            upgradeCount[statType] = 0;
        }
    }

    // StatType에 따른 업그레이드 코스트 계산
    public int GetUpgradeCost(StatType statType)
    {
        int n = upgradeCount[statType] + 1;  //코스트 공식 계산
        int multiplier = (int)statType + 1; 
        return 1 + n * multiplier;
    }

    // 현재 업그레이드 횟수 반환
    public int GetUpgradeCount(StatType statType)
    {
        return upgradeCount[statType];
    }

    public void TryUpgrade(StatType statType, StatBar statBar)
    {
        int upgradeCost = GetUpgradeCost(statType);
     
        // 골드 사용 시도
        if (!GoldManager.Instance.UseGold(upgradeCost))
        {
            Debug.Log("골드 부족");
            return;
        }

        // 업그레이드 횟수 증가
        upgradeCount[statType]++;

        // 플레이어 스텟 증가
        CharacterStats.Instance.UpgradeStat(statType, 1);

        // 관련 게이지바 증가
        if (statBar != null)
            statBar.Increase();
    }
}