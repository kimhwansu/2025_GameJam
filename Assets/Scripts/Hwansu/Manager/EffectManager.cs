using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    [Header("Effect Prefabs")]
    [SerializeField] private GameObject defaultEffectPrefab; // 기본 이펙트
    [SerializeField] private GameObject effect1Prefab; // 1번 이펙트
    [SerializeField] private GameObject effect2Prefab; // 2번 이펙트
    [SerializeField] private GameObject effect3Prefab; // 3번 이펙트

    private GameObject[] allEffects;
    private GameObject currentEffectPrefab;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // 배열로 관리
        allEffects = new GameObject[] { effect1Prefab, effect2Prefab, effect3Prefab };

        // 기본 이펙트 설정
        currentEffectPrefab = defaultEffectPrefab;
    }

    // 이펙트 번호로 변경
    public void ChangeEffect(int effectNumber)
    {
        if (effectNumber < 1 || effectNumber > allEffects.Length)
        {
            Debug.LogWarning($"잘못된 이펙트 번호: {effectNumber}");
            return;
        }

        int index = effectNumber - 1;

        if (allEffects[index] == null)
        {
            Debug.LogError($"이펙트 {effectNumber}번 프리팹이 할당되지 않았습니다!");
            return;
        }

        currentEffectPrefab = allEffects[index];

        Debug.Log($"이펙트 변경: {effectNumber}번");

        // CharacterAttack에 알림
        NotifyCharacterAttack();
    }

    // 현재 이펙트 프리팹 반환
    public GameObject GetCurrentEffectPrefab()
    {
        return currentEffectPrefab;
    }

    // CharacterAttack에게 이펙트 변경 알림
    private void NotifyCharacterAttack()
    {
        CharacterAttack characterAttack = FindObjectOfType<CharacterAttack>();

        if (characterAttack != null)
        {
            characterAttack.UpdateEffectPrefab(currentEffectPrefab);
        }
    }

    // 현재 이펙트 번호 반환
    public int GetCurrentEffectNumber()
    {
        for (int i = 0; i < allEffects.Length; i++)
        {
            if (allEffects[i] == currentEffectPrefab)
                return i + 1;
        }
        return 0; // 기본 이펙트
    }
}