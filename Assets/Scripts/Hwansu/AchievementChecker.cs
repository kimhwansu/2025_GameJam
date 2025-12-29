using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AchievementChecker : Singleton<AchievementChecker>
{
    [Header("StatBar References")]
    [SerializeField] private List<StatBar> statBars = new List<StatBar>();

    [Header("Ending UI References")]
    [SerializeField] private GameObject endingCanvas;
    [SerializeField] private GameObject successEndingImage;
    [SerializeField] private GameObject failEndingImage;

    [Header("Ending Buttons - Fail")]
    [SerializeField] private Button failReturnToTitleButton;
    [SerializeField] private Button failQuitGameButton;

    [Header("Ending Buttons - Success")]
    [SerializeField] private Button successQuitGameButton;

    [Header("Scene Settings")]
    [SerializeField] private string titleSceneName = "TitleScene";

    [Header("Settings")]
    [SerializeField] private int targetCount = 18;

    private bool isChecked = false;

    void Start()
    {
        if (failReturnToTitleButton != null)
        {
            failReturnToTitleButton.onClick.AddListener(OnReturnToTitleClicked);
        }

        if (failQuitGameButton != null)
        {
            failQuitGameButton.onClick.AddListener(OnQuitGameClicked);
        }

        if (successQuitGameButton != null)
        {
            successQuitGameButton.onClick.AddListener(OnQuitGameClicked);
        }

        // UI 초기 상태 설정
        if (endingCanvas != null)
        {
            endingCanvas.SetActive(false);
        }

        if (successEndingImage != null)
        {
            successEndingImage.SetActive(false);
        }

        if (failEndingImage != null)
        {
            failEndingImage.SetActive(false);
        }

        // ✅ 초기 상태 디버깅
        DebugUIState("Start");
    }

    void OnDestroy()
    {
        if (failReturnToTitleButton != null)
        {
            failReturnToTitleButton.onClick.RemoveListener(OnReturnToTitleClicked);
        }

        if (failQuitGameButton != null)
        {
            failQuitGameButton.onClick.RemoveListener(OnQuitGameClicked);
        }

        if (successQuitGameButton != null)
        {
            successQuitGameButton.onClick.RemoveListener(OnQuitGameClicked);
        }
    }

    private void OnReturnToTitleClicked()
    {
        Debug.Log("타이틀로 돌아가기 버튼 클릭!");
        ResetChecker();
        SceneManager.LoadScene(titleSceneName);
    }

    private void OnQuitGameClicked()
    {
        Debug.Log("게임 종료 버튼 클릭!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CheckEnding()
    {
        if (isChecked)
        {
            Debug.Log("이미 엔딩 판정 완료");
            return;
        }

        int totalCount = GetTotalCount();
        Debug.Log($"[Day End Check] total={totalCount}, target={targetCount}");

        if (totalCount >= targetCount)
        {
            ShowSuccessEnding();
        }
        else
        {
            ShowFailEnding();
        }

        isChecked = true;

        // ✅ 엔딩 표시 후 상태 디버깅
        DebugUIState("After CheckEnding");
    }

    private int GetTotalCount()
    {
        int statBarCount = 0;

        foreach (var statBar in statBars)
        {
            if (statBar != null)
            {
                int count = statBar.GetMaxCount();
                statBarCount += count;
                Debug.Log($"StatBar MAX 카운트: {count}");
            }
        }

        int purchaseCount = PurchaseButton.GetTotalPurchaseCount();

        Debug.Log($"=== 카운트 집계 ===");
        Debug.Log($"StatBar 총 MAX 카운트: {statBarCount}");
        Debug.Log($"PurchaseButton 총 구매 카운트: {purchaseCount}");
        Debug.Log($"합계: {statBarCount + purchaseCount}");

        return statBarCount + purchaseCount;
    }

    private void ShowSuccessEnding()
    {
        Debug.Log("★★★ 성공 엔딩! 18개 달성! ★★★");

        // ✅ 1단계: Canvas를 반드시 먼저 활성화
        if (endingCanvas != null)
        {
            endingCanvas.SetActive(true);
            Debug.Log($"[성공] endingCanvas 활성화: {endingCanvas.activeSelf}");
        }
        else
        {
            Debug.LogWarning("[성공] endingCanvas가 설정되지 않았습니다. 이미지가 Canvas 직속 자식인 경우 문제없습니다.");
        }

        // ✅ 2단계: 실패 이미지 비활성화
        if (failEndingImage != null)
        {
            failEndingImage.SetActive(false);
            Debug.Log($"[성공] failEndingImage 비활성화");
        }

        // ✅ 3단계: 성공 이미지 활성화
        if (successEndingImage != null)
        {
            successEndingImage.SetActive(true);
            Debug.Log($"[성공] successEndingImage 활성화!");
            Debug.Log($"[성공] successEndingImage.activeSelf: {successEndingImage.activeSelf}");
            Debug.Log($"[성공] successEndingImage.activeInHierarchy: {successEndingImage.activeInHierarchy}");

            // Canvas 확인
            Canvas canvas = successEndingImage.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"[성공] 부모 Canvas 발견: {canvas.name}, 활성화 상태: {canvas.gameObject.activeSelf}");
            }

            // ✅ Image 컴포넌트 체크
            Image img = successEndingImage.GetComponent<Image>();
            if (img != null)
            {
                Debug.Log($"[성공] Image - enabled: {img.enabled}, sprite: {(img.sprite != null ? img.sprite.name : "null")}");
                if (img.sprite == null)
                {
                    Debug.LogError("[성공] Image에 Sprite가 할당되지 않았습니다!");
                }
            }
            else
            {
                Debug.LogWarning("[성공] Image 컴포넌트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError("[성공] successEndingImage가 null입니다! Inspector에서 설정해주세요.");
        }
    }

    private void ShowFailEnding()
    {
        Debug.Log("실패 엔딩... 18개 미달성");

        // ✅ 1단계: Canvas를 반드시 먼저 활성화
        if (endingCanvas != null)
        {
            endingCanvas.SetActive(true);
            Debug.Log($"[실패] endingCanvas 활성화: {endingCanvas.activeSelf}");
        }
        else
        {
            Debug.LogWarning("[실패] endingCanvas가 설정되지 않았습니다. 이미지가 Canvas 직속 자식인 경우 문제없습니다.");
        }

        // ✅ 2단계: 성공 이미지 비활성화
        if (successEndingImage != null)
        {
            successEndingImage.SetActive(false);
            Debug.Log($"[실패] successEndingImage 비활성화");
        }

        // ✅ 3단계: 실패 이미지 활성화
        if (failEndingImage != null)
        {
            failEndingImage.SetActive(true);
            Debug.Log($"[실패] failEndingImage 활성화!");
            Debug.Log($"[실패] failEndingImage.activeSelf: {failEndingImage.activeSelf}");
            Debug.Log($"[실패] failEndingImage.activeInHierarchy: {failEndingImage.activeInHierarchy}");

            // Canvas 확인
            Canvas canvas = failEndingImage.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"[실패] 부모 Canvas 발견: {canvas.name}, 활성화 상태: {canvas.gameObject.activeSelf}");
            }

            // ✅ Image 컴포넌트 체크
            Image img = failEndingImage.GetComponent<Image>();
            if (img != null)
            {
                Debug.Log($"[실패] Image - enabled: {img.enabled}, sprite: {(img.sprite != null ? img.sprite.name : "null")}");
                if (img.sprite == null)
                {
                    Debug.LogError("[실패] Image에 Sprite가 할당되지 않았습니다!");
                }
            }
            else
            {
                Debug.LogWarning("[실패] Image 컴포넌트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError("[실패] failEndingImage가 null입니다! Inspector에서 설정해주세요.");
        }
    }

    // ✅ UI 상태 디버깅 헬퍼 함수
    private void DebugUIState(string context)
    {
        Debug.Log($"=== UI 상태 체크 ({context}) ===");

        if (endingCanvas != null)
            Debug.Log($"endingCanvas: {endingCanvas.activeSelf}");
        else
            Debug.Log("endingCanvas: null");

        if (successEndingImage != null)
            Debug.Log($"successEndingImage: activeSelf={successEndingImage.activeSelf}, activeInHierarchy={successEndingImage.activeInHierarchy}");
        else
            Debug.Log("successEndingImage: null");

        if (failEndingImage != null)
            Debug.Log($"failEndingImage: activeSelf={failEndingImage.activeSelf}, activeInHierarchy={failEndingImage.activeInHierarchy}");
        else
            Debug.Log("failEndingImage: null");
    }

    public int GetCurrentTotalCount()
    {
        return GetTotalCount();
    }

    public int GetRemainingCount()
    {
        return Mathf.Max(0, targetCount - GetTotalCount());
    }

    public void ResetChecker()
    {
        isChecked = false;
        PurchaseButton.ResetTotalPurchaseCount();

        if (endingCanvas != null)
            endingCanvas.SetActive(false);

        if (successEndingImage != null)
            successEndingImage.SetActive(false);

        if (failEndingImage != null)
            failEndingImage.SetActive(false);

        Debug.Log("AchievementChecker 리셋 완료");
    }
}