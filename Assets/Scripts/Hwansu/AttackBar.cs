using UnityEngine;
using UnityEngine.UI;

public class AttackBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private int currentGauge = 0;
    private const int maxGauge = 100;

    void Start()
    {
        UpdateGauge();
    }

    // 강화 성공 시 1 증가
    public void Increase()
    {
        if (currentGauge >= maxGauge)
            return;

        currentGauge++;
        UpdateGauge();
    }

    private void UpdateGauge()
    {
        fillImage.fillAmount = (float)currentGauge / maxGauge;
    }

    // 외부에서 현재 공격력 값 가져오기 (필요없으면 삭제)
    public int GetAttackLevel()
    {
        return currentGauge;
    }
}
