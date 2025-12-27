using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;

    [System.Serializable]
    public class BackgroundSet
    {
        public GameObject backgroundObject1; // 첫 번째 배경 GameObject (Image 포함)
        public GameObject backgroundObject2; // 두 번째 배경 GameObject (Image 포함)
    }

    [Header("Background Sets")]
    [SerializeField] private BackgroundSet bg1; // 1번 배경
    [SerializeField] private BackgroundSet bg2; // 2번 배경
    [SerializeField] private BackgroundSet bg3; // 3번 배경
    [SerializeField] private BackgroundSet bg4; // 4번 배경

    private BackgroundSet[] allBackgrounds;
    private int currentBackgroundIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // 배열로 관리
        allBackgrounds = new BackgroundSet[] { bg1, bg2, bg3, bg4 };
    }

    void Start()
    {
        // 모든 배경 비활성화
        HideAllBackgrounds();

        // 기본 배경 활성화 (bg1)
        if (bg1.backgroundObject1 != null)
            bg1.backgroundObject1.SetActive(true);
        if (bg1.backgroundObject2 != null)
            bg1.backgroundObject2.SetActive(true);
    }

    // 모든 배경 비활성화
    private void HideAllBackgrounds()
    {
        foreach (var bgSet in allBackgrounds)
        {
            if (bgSet.backgroundObject1 != null)
                bgSet.backgroundObject1.SetActive(false);

            if (bgSet.backgroundObject2 != null)
                bgSet.backgroundObject2.SetActive(false);
        }
    }

    // 배경 번호로 변경
    public void ChangeBackground(int bgNumber)
    {
        // 1번 구매 → bg2 (index 1)
        // 2번 구매 → bg3 (index 2)
        // 3번 구매 → bg4 (index 3)
        int actualIndex = bgNumber; // 1 → index 1 (bg2)

        if (actualIndex < 1 || actualIndex >= allBackgrounds.Length)
        {
            Debug.LogWarning($"잘못된 배경 번호: {bgNumber}");
            return;
        }

        // 모든 배경 비활성화
        HideAllBackgrounds();

        var bgSet = allBackgrounds[actualIndex];

        // 선택한 배경만 활성화
        if (bgSet.backgroundObject1 != null)
        {
            bgSet.backgroundObject1.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"배경 {bgNumber}번의 첫 번째 오브젝트가 할당되지 않았습니다!");
        }

        if (bgSet.backgroundObject2 != null)
        {
            bgSet.backgroundObject2.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"배경 {bgNumber}번의 두 번째 오브젝트가 할당되지 않았습니다!");
        }

        currentBackgroundIndex = actualIndex;

        Debug.Log($"배경 구매 {bgNumber}번 → bg{actualIndex + 1} 활성화");
    }

    // 현재 배경 번호 반환
    public int GetCurrentBackgroundNumber()
    {
        return currentBackgroundIndex + 1;
    }
}