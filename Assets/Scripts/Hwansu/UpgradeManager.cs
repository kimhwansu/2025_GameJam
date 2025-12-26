using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public UserData userData;
    public CharacterData characterData;

    public int upgradeCost = 100;  //강화버튼 누르면 재화 사라지는 값

    void Awake()
    {
        Instance = this;
    }

    //강화버튼 누르면 발생되는 이벤트
    public void TryUpgrade()
    {
        if (!userData.SpendGold(upgradeCost))
        {
            Debug.Log("골드 부족");
            return;
        }

        characterData.IncreaseAttack(1);  //공격력 +1
    }
}
